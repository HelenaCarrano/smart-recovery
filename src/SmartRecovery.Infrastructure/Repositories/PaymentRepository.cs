using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class PaymentRepository(SmartRecoveryDbContext context)
    : RepositoryBase<Payment>(context), IPaymentRepository
{
    public Task<Payment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(p => p.Customer)
            .Include(p => p.Subscription)
            .Include(p => p.Attempts)
            .Include(p => p.RecoveryAnalysis)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await DbSet.Where(p => p.CustomerId == customerId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetDueForRetryAsync(DateTime asOf, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Subscription)
            .Where(p => p.Status == PaymentStatus.Declined && p.ScheduledRetryAt != null && p.ScheduledRetryAt <= asOf)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetHistoryByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Attempts)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(PaymentFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(p => p.Customer).AsQueryable();

        if (filter.Status is not null)
            query = query.Where(p => p.Status == filter.Status);
        if (filter.DeclineReason is not null)
            query = query.Where(p => p.DeclineReason == filter.DeclineReason);
        if (filter.CustomerId is not null)
            query = query.Where(p => p.CustomerId == filter.CustomerId);
        if (filter.DateFrom is not null)
            query = query.Where(p => p.CreatedAt >= filter.DateFrom);
        if (filter.DateTo is not null)
            query = query.Where(p => p.CreatedAt <= filter.DateTo);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> CountAllAsync(CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(p => p.Status == status, cancellationToken);

    public Task<int> CountEverDeclinedAsync(CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(p => p.Status == PaymentStatus.Declined || p.AttemptCount > 1, cancellationToken);

    public async Task<IReadOnlyList<DeclineReasonCount>> GetDeclineReasonCountsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(p => p.DeclineReason != null)
            .GroupBy(p => p.DeclineReason!.Value)
            .Select(g => new DeclineReasonCount(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

    public Task<decimal> GetApprovedRevenueAsync(CancellationToken cancellationToken = default) =>
        DbSet.Where(p => p.Status == PaymentStatus.Approved).SumAsync(p => p.Amount, cancellationToken);

    public async Task<(int Count, decimal Revenue)> GetRecoveredStatsAsync(CancellationToken cancellationToken = default)
    {
        var recovered = DbSet.Where(p => p.Status == PaymentStatus.Approved && p.AttemptCount > 1);
        var count = await recovered.CountAsync(cancellationToken);
        var revenue = count > 0 ? await recovered.SumAsync(p => p.Amount, cancellationToken) : 0m;
        return (count, revenue);
    }

    public async Task<IReadOnlyList<PaymentTrendPoint>> GetTrendAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        var raw = await DbSet
            .Where(p => p.CreatedAt >= since)
            .GroupBy(p => new { Date = p.CreatedAt.Date, p.Status })
            .Select(g => new { g.Key.Date, g.Key.Status, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return raw
            .GroupBy(r => r.Date)
            .Select(g => new PaymentTrendPoint(
                DateOnly.FromDateTime(g.Key),
                g.Where(x => x.Status == PaymentStatus.Approved).Sum(x => x.Count),
                g.Where(x => x.Status == PaymentStatus.Declined).Sum(x => x.Count)))
            .OrderBy(p => p.Date)
            .ToList();
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountByCustomerIdsAsync(IReadOnlyList<Guid> customerIds, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(p => customerIds.Contains(p.CustomerId))
            .GroupBy(p => p.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CustomerId, g => g.Count, cancellationToken);
}
