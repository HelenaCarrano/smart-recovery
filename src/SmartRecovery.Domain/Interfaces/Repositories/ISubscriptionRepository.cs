using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Subscription>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Subscription>> GetByStatusAsync(SubscriptionStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assinaturas ativas cuja NextBillingDate já chegou — usadas pelo worker de faturamento
    /// para gerar os próximos Payments.
    /// </summary>
    Task<IReadOnlyList<Subscription>> GetDueForBillingAsync(DateTime asOf, CancellationToken cancellationToken = default);
}
