using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class PaymentAttemptRepository(SmartRecoveryDbContext context)
    : RepositoryBase<PaymentAttempt>(context), IPaymentAttemptRepository
{
    public async Task<IReadOnlyList<PaymentAttempt>> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(a => a.PaymentId == paymentId)
            .OrderBy(a => a.AttemptedAt)
            .ToListAsync(cancellationToken);
}
