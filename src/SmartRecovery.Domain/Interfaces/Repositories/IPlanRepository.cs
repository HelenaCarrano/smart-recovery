using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IPlanRepository : IRepository<Plan>
{
    Task<IReadOnlyList<Plan>> GetActiveAsync(CancellationToken cancellationToken = default);
}
