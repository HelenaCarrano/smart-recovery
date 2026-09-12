using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Infrastructure.BackgroundServices;

/// <summary>
/// Worker único responsável por todo o ciclo de vida automático das cobranças:
///
/// 1. Faturamento: assinaturas ativas cuja NextBillingDate já chegou geram um novo Payment
///    e têm sua NextBillingDate avançada para o próximo ciclo.
/// 2. Retry: pagamentos recusados com ScheduledRetryAt vencido são reprocessados,
///    fechando o laço com o RecoveryDecisionEngine (que agendou esse retry).
///
/// Roda em um intervalo configurável (SmartRecovery:PaymentRetryWorker:IntervalSeconds).
/// </summary>
public class PaymentRetryWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<PaymentRetryWorkerOptions> options,
    ILogger<PaymentRetryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(options.Value.IntervalSeconds, 5));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCycleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PaymentRetryWorker cycle failed.");
            }

            await Task.Delay(interval, stoppingToken);
        }
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
        var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;

        await BillDueSubscriptionsAsync(subscriptionRepository, paymentService, unitOfWork, now, cancellationToken);
        await RetryDuePaymentsAsync(paymentRepository, paymentService, now, cancellationToken);
    }

    private async Task BillDueSubscriptionsAsync(
        ISubscriptionRepository subscriptionRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var dueSubscriptions = await subscriptionRepository.GetDueForBillingAsync(now, cancellationToken);
        if (dueSubscriptions.Count == 0)
            return;

        // Avança a data de cobrança antes de criar o Payment para não faturar a mesma assinatura duas vezes
        // caso o processamento do pagamento demore mais que o intervalo do worker.
        foreach (var subscription in dueSubscriptions)
        {
            subscription.NextBillingDate = BillingCycleCalculator.NextBillingDate(subscription.NextBillingDate, subscription.Plan.Periodicity);
            subscriptionRepository.Update(subscription);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var subscription in dueSubscriptions)
        {
            var payment = await paymentService.CreatePendingForSubscriptionAsync(subscription.Id, cancellationToken);
            await paymentService.ProcessAsync(payment.Id, cancellationToken);
        }

        logger.LogInformation("Billed {Count} subscription(s) due for charge.", dueSubscriptions.Count);
    }

    private async Task RetryDuePaymentsAsync(
        IPaymentRepository paymentRepository,
        IPaymentService paymentService,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var duePayments = await paymentRepository.GetDueForRetryAsync(now, cancellationToken);
        if (duePayments.Count == 0)
            return;

        foreach (var payment in duePayments)
            await paymentService.ProcessAsync(payment.Id, cancellationToken);

        logger.LogInformation("Retried {Count} declined payment(s) due for retry.", duePayments.Count);
    }
}
