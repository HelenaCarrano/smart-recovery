using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class WebhookEventRepository(SmartRecoveryDbContext context)
    : RepositoryBase<WebhookEvent>(context), IWebhookEventRepository
{
    public Task<WebhookEvent?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(w => w.IdempotencyKey == idempotencyKey, cancellationToken);
}
