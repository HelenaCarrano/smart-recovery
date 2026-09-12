using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class SubscriptionRepository(SmartRecoveryDbContext context)
    : RepositoryBase<Subscription>(context), ISubscriptionRepository
{
    public Task<Subscription?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(s => s.Customer)
            .Include(s => s.Plan)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Subscription>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await DbSet.Include(s => s.Plan).Where(s => s.CustomerId == customerId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Subscription>> GetByStatusAsync(SubscriptionStatus status, CancellationToken cancellationToken = default) =>
        await DbSet.Where(s => s.Status == status).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Subscription>> GetDueForBillingAsync(DateTime asOf, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(s => s.Plan)
            .Include(s => s.Customer)
            .Where(s => s.Status == SubscriptionStatus.Active && s.NextBillingDate <= asOf)
            .ToListAsync(cancellationToken);
}
