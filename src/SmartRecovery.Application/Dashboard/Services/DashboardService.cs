using SmartRecovery.Application.Dashboard.DTOs;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Dashboard.Services;

public class DashboardService(
    ICustomerRepository customerRepository,
    ISubscriptionRepository subscriptionRepository,
    IPaymentRepository paymentRepository,
    IRecoveryAnalysisRepository recoveryAnalysisRepository) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);
        var activeSubscriptions = await subscriptionRepository.GetByStatusAsync(SubscriptionStatus.Active, cancellationToken);
        var payments = await paymentRepository.GetAllAsync(cancellationToken);
        var pendingRecoveryActions = await recoveryAnalysisRepository.GetPendingExecutionAsync(cancellationToken);

        var approved = payments.Count(p => p.Status == PaymentStatus.Approved);
        var declined = payments.Count(p => p.Status == PaymentStatus.Declined);
        var pending = payments.Count(p => p.Status == PaymentStatus.Pending);

        // Um pagamento aprovado que precisou de mais de uma tentativa foi, por definição, recuperado.
        var recovered = payments.Count(p => p.Status == PaymentStatus.Approved && p.AttemptCount > 1);
        var everDeclined = payments.Count(p => p.Status == PaymentStatus.Declined || p.AttemptCount > 1);
        var recoveryRate = everDeclined > 0 ? recovered / (double)everDeclined : 0;

        var totalRevenue = payments.Where(p => p.Status == PaymentStatus.Approved).Sum(p => p.Amount);

        return new DashboardSummaryDto(
            customers.Count,
            activeSubscriptions.Count,
            payments.Count,
            approved,
            declined,
            pending,
            recovered,
            recoveryRate,
            totalRevenue,
            pendingRecoveryActions.Count);
    }
}
