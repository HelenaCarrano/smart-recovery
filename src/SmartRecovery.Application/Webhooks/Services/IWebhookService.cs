using SmartRecovery.Application.Webhooks.DTOs;

namespace SmartRecovery.Application.Webhooks.Services;

public interface IWebhookService
{
    /// <summary>
    /// Processa um evento de webhook de pagamento de forma idempotente: se a IdempotencyKey
    /// já foi vista antes, o evento é ignorado silenciosamente e o registro original é retornado.
    /// </summary>
    Task<WebhookEventDto> ProcessPaymentWebhookAsync(WebhookPayloadDto payload, CancellationToken cancellationToken = default);
}
