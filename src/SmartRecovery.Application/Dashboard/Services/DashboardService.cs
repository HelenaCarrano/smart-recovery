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
        var totalCustomers = await customerRepository.CountAsync(cancellationToken);
        var activeSubscriptions = await subscriptionRepository.CountByStatusAsync(SubscriptionStatus.Active, cancellationToken);
        var totalPayments = await paymentRepository.CountAllAsync(cancellationToken);
        var approved = await paymentRepository.CountByStatusAsync(PaymentStatus.Approved, cancellationToken);
        var declined = await paymentRepository.CountByStatusAsync(PaymentStatus.Declined, cancellationToken);
        var pending = await paymentRepository.CountByStatusAsync(PaymentStatus.Pending, cancellationToken);
        var everDeclined = await paymentRepository.CountEverDeclinedAsync(cancellationToken);
        var (recoveredCount, recoveredRevenue) = await paymentRepository.GetRecoveredStatsAsync(cancellationToken);
        var totalRevenue = await paymentRepository.GetApprovedRevenueAsync(cancellationToken);
        var pendingRecoveryActions = await recoveryAnalysisRepository.CountPendingExecutionAsync(cancellationToken);

        var recoveryRate = everDeclined > 0 ? recoveredCount / (double)everDeclined : 0;

        return new DashboardSummaryDto(
            totalCustomers,
            activeSubscriptions,
            totalPayments,
            approved,
            declined,
            pending,
            recoveredCount,
            recoveryRate,
            totalRevenue,
            recoveredRevenue,
            pendingRecoveryActions);
    }

    public async Task<IReadOnlyList<DeclineReasonCountDto>> GetDeclineReasonBreakdownAsync(CancellationToken cancellationToken = default)
    {
        var counts = await paymentRepository.GetDeclineReasonCountsAsync(cancellationToken);
        return counts.Select(c => new DeclineReasonCountDto(c.DeclineReason, c.Count)).ToList();
    }

    public async Task<IReadOnlyList<PaymentTrendPointDto>> GetTrendAsync(int days, CancellationToken cancellationToken = default)
    {
        var since = DateTime.UtcNow.Date.AddDays(-Math.Max(days, 1));
        var points = await paymentRepository.GetTrendAsync(since, cancellationToken);
        return points.Select(p => new PaymentTrendPointDto(p.Date, p.Approved, p.Declined)).ToList();
    }
}
