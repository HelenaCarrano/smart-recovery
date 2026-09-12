using SmartRecovery.Application.Dashboard.DTOs;

namespace SmartRecovery.Application.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
