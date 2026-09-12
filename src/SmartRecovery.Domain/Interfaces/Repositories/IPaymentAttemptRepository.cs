using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IPaymentAttemptRepository : IRepository<PaymentAttempt>
{
    Task<IReadOnlyList<PaymentAttempt>> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
