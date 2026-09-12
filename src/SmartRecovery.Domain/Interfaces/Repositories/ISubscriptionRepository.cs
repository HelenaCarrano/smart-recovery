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

    Task<int> CountByStatusAsync(SubscriptionStatus status, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Subscription> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, SubscriptionStatus? status, CancellationToken cancellationToken = default);

    /// <summary>Contagem de assinaturas por cliente, para os ids informados — uma única query agrupada, sem N+1.</summary>
    Task<IReadOnlyDictionary<Guid, int>> CountByCustomerIdsAsync(IReadOnlyList<Guid> customerIds, CancellationToken cancellationToken = default);
}
