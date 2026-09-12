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
        await DbSet.Include(s => s.Plan).Where(s => s.Status == status).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Subscription>> GetDueForBillingAsync(DateTime asOf, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(s => s.Plan)
            .Include(s => s.Customer)
            .Where(s => s.Status == SubscriptionStatus.Active && s.NextBillingDate <= asOf)
            .ToListAsync(cancellationToken);

    public Task<int> CountByStatusAsync(SubscriptionStatus status, CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(s => s.Status == status, cancellationToken);

    public async Task<(IReadOnlyList<Subscription> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, SubscriptionStatus? status, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(s => s.Customer).Include(s => s.Plan).AsQueryable();

        if (status is not null)
            query = query.Where(s => s.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountByCustomerIdsAsync(IReadOnlyList<Guid> customerIds, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(s => customerIds.Contains(s.CustomerId))
            .GroupBy(s => s.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CustomerId, g => g.Count, cancellationToken);
}
