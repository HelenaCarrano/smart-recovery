using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class RecoveryAnalysisRepository(SmartRecoveryDbContext context)
    : RepositoryBase<RecoveryAnalysis>(context), IRecoveryAnalysisRepository
{
    public Task<RecoveryAnalysis?> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(r => r.PaymentId == paymentId, cancellationToken);

    public async Task<IReadOnlyList<RecoveryAnalysis>> GetPendingExecutionAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(r => r.Payment)
            .Where(r => r.ExecutedAt == null)
            .ToListAsync(cancellationToken);

    public Task<int> CountPendingExecutionAsync(CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(r => r.ExecutedAt == null, cancellationToken);
}
