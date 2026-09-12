using SmartRecovery.Application.Subscriptions.DTOs;

namespace SmartRecovery.Application.Subscriptions.Services;

public interface ISubscriptionService
{
    Task<SubscriptionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SubscriptionDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
