using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IWebhookEventRepository : IRepository<WebhookEvent>
{
    /// <summary>Usado para checar idempotência antes de processar um novo evento.</summary>
    Task<WebhookEvent?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
}
