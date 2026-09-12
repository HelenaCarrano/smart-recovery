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

    public async Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default) =>
        await DbSet.Where(p => p.Status == status).ToListAsync(cancellationToken);

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
}
