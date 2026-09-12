using SmartRecovery.Application.Plans.DTOs;

namespace SmartRecovery.Application.Plans.Services;

public interface IPlanService
{
    Task<IReadOnlyList<PlanDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<PlanDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PlanDto> CreateAsync(CreatePlanDto dto, CancellationToken cancellationToken = default);
}
