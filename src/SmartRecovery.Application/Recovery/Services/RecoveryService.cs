using SmartRecovery.Application.Recovery.DTOs;
using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Recovery.Services;

public class RecoveryService(
    IPaymentRepository paymentRepository,
    ISubscriptionRepository subscriptionRepository,
    IRecoveryAnalysisRepository recoveryAnalysisRepository,
    IUnitOfWork unitOfWork) : IRecoveryService
{
    /// <summary>Janela usada para considerar recusas/tentativas "recentes" no cálculo do score.</summary>
    private static readonly TimeSpan RecentWindow = TimeSpan.FromDays(30);

    public async Task<RecoveryAnalysisDto> AnalyzeAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetWithDetailsAsync(paymentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment '{paymentId}' not found.");

        if (payment.Status != PaymentStatus.Declined)
            throw new InvalidOperationException("Recovery analysis can only run for a payment with status Declined.");

        var input = await BuildScoreInputAsync(payment, cancellationToken);
        var breakdown = RecoveryScoreCalculator.CalculateBreakdown(input);
        var action = RecoveryDecisionEngine.Decide(breakdown.Total, input.DeclineReason);

        var analysis = new RecoveryAnalysis
        {
            PaymentId = payment.Id,
            RecoveryScore = breakdown.Total,
            RecommendedAction = action,
            TotalPayments = input.TotalPayments,
            SuccessfulPayments = input.SuccessfulPayments,
            PreviouslyRecoveredPayments = input.PreviouslyRecoveredPayments,
            RecentDeclines = input.RecentDeclines,
            RecentAttempts = input.RecentAttempts,
            BaseScore = breakdown.BaseScore,
            HistoryAdjustment = breakdown.HistoryAdjustment,
            RecoveryTrackRecordBonus = breakdown.RecoveryTrackRecordBonus,
            RecentDeclinesAdjustment = breakdown.RecentDeclinesAdjustment,
            RecentAttemptsAdjustment = breakdown.RecentAttemptsAdjustment
        };

        await ApplyActionAsync(payment, analysis, cancellationToken);

        await recoveryAnalysisRepository.AddAsync(analysis, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(analysis);
    }

    public async Task<RecoveryAnalysisDto?> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var analysis = await recoveryAnalysisRepository.GetByPaymentAsync(paymentId, cancellationToken);
        return analysis is null ? null : ToDto(analysis);
    }

    public async Task<IReadOnlyList<RecoveryAnalysisDto>> GetPendingExecutionAsync(CancellationToken cancellationToken = default)
    {
        var pending = await recoveryAnalysisRepository.GetPendingExecutionAsync(cancellationToken);
        return pending.Select(ToDto).ToList();
    }

    private async Task<RecoveryScoreInput> BuildScoreInputAsync(Payment payment, CancellationToken cancellationToken)
    {
        var history = await paymentRepository.GetHistoryByCustomerAsync(payment.CustomerId, cancellationToken);
        var cutoff = DateTime.UtcNow - RecentWindow;

        // A taxa histórica de sucesso deve refletir apenas cobranças anteriores a esta. Incluir o próprio
        // pagamento recém-recusado sempre zeraria a taxa de um cliente novo, penalizando-o sem necessidade.
        var priorPayments = history.Where(p => p.Id != payment.Id).ToList();

        var totalPayments = priorPayments.Count;
        var successfulPayments = priorPayments.Count(p => p.Status == PaymentStatus.Approved);
        // Um pagamento aprovado que precisou de mais de uma tentativa foi, por definição, recuperado.
        var previouslyRecovered = priorPayments.Count(p => p.Status == PaymentStatus.Approved && p.AttemptCount > 1);
        var recentDeclines = history.Count(p => p.Status == PaymentStatus.Declined && p.CreatedAt >= cutoff);
        var recentAttempts = history.Where(p => p.CreatedAt >= cutoff).Sum(p => p.AttemptCount);

        return new RecoveryScoreInput(
            payment.DeclineReason ?? DeclineReason.Unknown,
            totalPayments,
            successfulPayments,
            previouslyRecovered,
            recentDeclines,
            recentAttempts);
    }

    /// <summary>
    /// Aplica o efeito imediato da ação recomendada:
    /// retries agendam ScheduledRetryAt; cancelamento encerra a assinatura.
    /// RequestPaymentMethodUpdate e ManualReview dependem de ação externa (cliente/humano)
    /// e por isso ficam com ExecutedAt em aberto.
    /// </summary>
    private async Task ApplyActionAsync(Payment payment, RecoveryAnalysis analysis, CancellationToken cancellationToken)
    {
        if (RecoveryActionScheduler.TryGetRetryDelay(analysis.RecommendedAction, out var delay))
        {
            payment.ScheduledRetryAt = DateTime.UtcNow.Add(delay);
            paymentRepository.Update(payment);
            analysis.ExecutedAt = DateTime.UtcNow;
            return;
        }

        if (analysis.RecommendedAction == RecoveryAction.CancelSubscription)
        {
            var subscription = await subscriptionRepository.GetByIdAsync(payment.SubscriptionId, cancellationToken);
            if (subscription is not null && subscription.Status != SubscriptionStatus.Cancelled)
            {
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.EndDate = DateTime.UtcNow;
                subscriptionRepository.Update(subscription);
            }

            analysis.ExecutedAt = DateTime.UtcNow;
        }
    }

    private static RecoveryAnalysisDto ToDto(RecoveryAnalysis a) => new(
        a.Id, a.PaymentId, a.RecoveryScore, a.RecommendedAction, a.AnalyzedAt, a.ExecutedAt,
        a.TotalPayments, a.SuccessfulPayments, a.PreviouslyRecoveredPayments, a.RecentDeclines, a.RecentAttempts,
        a.BaseScore, a.HistoryAdjustment, a.RecoveryTrackRecordBonus, a.RecentDeclinesAdjustment, a.RecentAttemptsAdjustment);
}
