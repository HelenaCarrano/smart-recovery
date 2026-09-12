using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Infrastructure.Data.Seed;

/// <summary>
/// Gera clientes, assinaturas e ~9 meses de histórico de cobrança simulando o próprio pipeline de
/// produção (PaymentGatewaySimulator → RecoveryScoreCalculator → RecoveryDecisionEngine →
/// RecoveryActionScheduler) dia a dia, em vez de inserir números fabricados à mão. Random com seed
/// fixa para o resultado ser reprodutível entre execuções. Só roda se o banco já não tiver planos.
/// </summary>
public static class SmartRecoveryDataSeeder
{
    private const int CustomerCount = 80;
    private const int MinHistoryDays = 30;
    private const int MaxHistoryDays = 270;
    private const double ApprovalRate = 0.75;
    private const int MaxAttemptsPerCycle = 6;

    private static readonly TimeSpan RecentWindow = TimeSpan.FromDays(30);
    private static readonly DeclineReason[] DeclineReasons = Enum.GetValues<DeclineReason>();

    private static readonly string[] FirstNames =
        ["Ana", "Bruno", "Carla", "Diego", "Elisa", "Fábio", "Gabriela", "Hugo", "Isabela", "João",
         "Karina", "Lucas", "Marina", "Nicolas", "Otávio", "Patrícia", "Rafael", "Sofia", "Tiago", "Valentina"];

    private static readonly string[] LastNames =
        ["Almeida", "Barbosa", "Costa", "Dias", "Ferreira", "Gomes", "Lima", "Martins", "Nunes", "Oliveira",
         "Pereira", "Ribeiro", "Santos", "Souza", "Teixeira"];

    public static async Task SeedAsync(SmartRecoveryDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Plans.AnyAsync(cancellationToken))
            return;

        var random = new Random(42);
        var now = DateTime.UtcNow;

        var plans = CreatePlans();
        context.Plans.AddRange(plans);

        for (var i = 0; i < CustomerCount; i++)
        {
            var customer = CreateCustomer(i, random);
            context.Customers.Add(customer);

            var customerHistory = new List<Payment>();
            var subscriptionCount = random.NextDouble() < 0.3 ? 2 : 1;

            for (var s = 0; s < subscriptionCount; s++)
                SimulateSubscription(customer, plans[random.Next(plans.Length)], random, now, context, customerHistory);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Plan[] CreatePlans() =>
    [
        new() { Name = "Básico", Description = "Plano de entrada, cobrança mensal.", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly },
        new() { Name = "Profissional", Description = "Plano intermediário, cobrança trimestral.", Price = 129.90m, Periodicity = PlanPeriodicity.Quarterly },
        new() { Name = "Enterprise", Description = "Plano completo, cobrança anual.", Price = 999.90m, Periodicity = PlanPeriodicity.Annual }
    ];

    private static Customer CreateCustomer(int index, Random random) => new()
    {
        Name = $"{Pick(FirstNames, random)} {Pick(LastNames, random)}",
        Email = $"cliente{index + 1}@example.com",
        Document = $"{random.Next(100, 999)}.{random.Next(100, 999)}.{random.Next(100, 999)}-{random.Next(10, 99)}"
    };

    /// <summary>
    /// Simula ciclos de cobrança do início da assinatura até hoje. Cada ciclo repete o mesmo fluxo de
    /// PaymentService.ApplyResultAsync + RecoveryService.AnalyzeAsync: cobra, e se recusar, calcula o
    /// Recovery Score real e aplica a ação recomendada (retry, cancelamento, ou deixa pendente).
    /// </summary>
    private static void SimulateSubscription(
        Customer customer, Plan plan, Random random, DateTime now, SmartRecoveryDbContext context, List<Payment> customerHistory)
    {
        var startDate = now.AddDays(-random.Next(MinHistoryDays, MaxHistoryDays));
        var subscription = new Subscription
        {
            CustomerId = customer.Id,
            PlanId = plan.Id,
            StartDate = startDate,
            NextBillingDate = startDate,
            Status = SubscriptionStatus.Active
        };
        context.Subscriptions.Add(subscription);

        var billingDate = startDate;

        while (billingDate <= now && subscription.Status == SubscriptionStatus.Active)
        {
            var payment = new Payment
            {
                CustomerId = customer.Id,
                SubscriptionId = subscription.Id,
                Amount = plan.Price,
                CreatedAt = billingDate,
                Status = PaymentStatus.Pending
            };
            context.Payments.Add(payment);

            SimulatePaymentCycle(payment, subscription, customerHistory, random, billingDate, now, context);
            customerHistory.Add(payment);

            if (subscription.Status != SubscriptionStatus.Active)
                break;

            billingDate = BillingCycleCalculator.NextBillingDate(billingDate, plan.Periodicity);
        }

        if (subscription.Status == SubscriptionStatus.Active)
            subscription.NextBillingDate = billingDate;
    }

    /// <summary>Cobra (e retenta, se aplicável) um único ciclo até aprovar, esgotar tentativas, ou a assinatura ser cancelada.</summary>
    private static void SimulatePaymentCycle(
        Payment payment, Subscription subscription, List<Payment> customerHistory, Random random,
        DateTime attemptDate, DateTime now, SmartRecoveryDbContext context)
    {
        for (var attemptCount = 1; attemptCount <= MaxAttemptsPerCycle && attemptDate <= now; attemptCount++)
        {
            var (status, declineReason) = SimulateCharge(random);

            context.PaymentAttempts.Add(new PaymentAttempt
            {
                PaymentId = payment.Id,
                AttemptedAt = attemptDate,
                ResultStatus = status,
                DeclineReason = declineReason
            });

            payment.AttemptCount = attemptCount;
            payment.Status = status;
            payment.DeclineReason = declineReason;

            if (status == PaymentStatus.Approved)
            {
                payment.ScheduledRetryAt = null;
                return;
            }

            var input = BuildScoreInput(customerHistory, payment, declineReason!.Value, attemptDate);
            var breakdown = RecoveryScoreCalculator.CalculateBreakdown(input);
            var action = RecoveryDecisionEngine.Decide(breakdown.Total, declineReason.Value);

            var analysis = new RecoveryAnalysis
            {
                PaymentId = payment.Id,
                RecoveryScore = breakdown.Total,
                RecommendedAction = action,
                AnalyzedAt = attemptDate,
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
            context.RecoveryAnalyses.Add(analysis);

            if (RecoveryActionScheduler.TryGetRetryDelay(action, out var delay))
            {
                var nextAttempt = attemptDate.Add(delay);
                analysis.ExecutedAt = attemptDate;
                payment.ScheduledRetryAt = nextAttempt;

                // Retry ainda não venceu dentro da janela histórica simulada — fica Declined com retry
                // agendado no futuro, igual aconteceria de verdade se "hoje" fosse antes desse horário.
                if (nextAttempt > now)
                    return;

                attemptDate = nextAttempt;
                continue;
            }

            if (action == RecoveryAction.CancelSubscription)
            {
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.EndDate = attemptDate;
                analysis.ExecutedAt = attemptDate;
            }

            // RequestPaymentMethodUpdate e ManualReview dependem de ação externa — ExecutedAt fica em aberto.
            return;
        }
    }

    /// <summary>Espelha RecoveryService.BuildScoreInputAsync, usando o histórico simulado até agora em vez de uma consulta ao banco.</summary>
    private static RecoveryScoreInput BuildScoreInput(List<Payment> history, Payment current, DeclineReason declineReason, DateTime asOf)
    {
        var cutoff = asOf - RecentWindow;

        var totalPayments = history.Count;
        var successfulPayments = history.Count(p => p.Status == PaymentStatus.Approved);
        var previouslyRecovered = history.Count(p => p.Status == PaymentStatus.Approved && p.AttemptCount > 1);

        var withCurrent = history.Append(current);
        var recentDeclines = withCurrent.Count(p => p.Status == PaymentStatus.Declined && p.CreatedAt >= cutoff);
        var recentAttempts = withCurrent.Where(p => p.CreatedAt >= cutoff).Sum(p => p.AttemptCount);

        return new RecoveryScoreInput(declineReason, totalPayments, successfulPayments, previouslyRecovered, recentDeclines, recentAttempts);
    }

    /// <summary>Mesma distribuição de PaymentGatewaySimulator (75% aprovação), mas com Random seedável para o seed ser reprodutível.</summary>
    private static (PaymentStatus Status, DeclineReason? DeclineReason) SimulateCharge(Random random)
    {
        if (random.NextDouble() < ApprovalRate)
            return (PaymentStatus.Approved, null);

        return (PaymentStatus.Declined, DeclineReasons[random.Next(DeclineReasons.Length)]);
    }

    private static string Pick(string[] values, Random random) => values[random.Next(values.Length)];
}
