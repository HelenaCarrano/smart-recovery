using SmartRecovery.Application.Dashboard.DTOs;

namespace SmartRecovery.Application.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeclineReasonCountDto>> GetDeclineReasonBreakdownAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentTrendPointDto>> GetTrendAsync(int days, CancellationToken cancellationToken = default);
}
