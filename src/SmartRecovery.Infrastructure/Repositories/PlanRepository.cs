using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class PlanRepository(SmartRecoveryDbContext context)
    : RepositoryBase<Plan>(context), IPlanRepository
{
    public async Task<IReadOnlyList<Plan>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await DbSet.Where(p => p.IsActive).ToListAsync(cancellationToken);
}
