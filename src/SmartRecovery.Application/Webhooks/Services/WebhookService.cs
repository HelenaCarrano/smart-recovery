using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Application.Webhooks.DTOs;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Webhooks.Services;

public class WebhookService(
    IWebhookEventRepository webhookEventRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork,
    ILogger<WebhookService> logger) : IWebhookService
{
    public async Task<WebhookEventDto> ProcessPaymentWebhookAsync(WebhookPayloadDto payload, CancellationToken cancellationToken = default)
    {
        var existing = await webhookEventRepository.GetByIdempotencyKeyAsync(payload.IdempotencyKey, cancellationToken);
        if (existing is not null)
            return ToDto(existing);

        var webhookEvent = new WebhookEvent
        {
            IdempotencyKey = payload.IdempotencyKey,
            EventType = payload.EventType,
            Payload = JsonSerializer.Serialize(payload)
        };

        await webhookEventRepository.AddAsync(webhookEvent, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await paymentService.ApplyExternalResultAsync(payload.PaymentId, payload.Status, payload.DeclineReason, cancellationToken);

            webhookEvent.Status = WebhookEventStatus.Processed;
            webhookEvent.ProcessedAt = DateTime.UtcNow;
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            // Não relançamos: o evento fica registrado como Failed para auditoria/reprocessamento,
            // mas o log é o que garante que a falha não passe despercebida em produção.
            logger.LogWarning(ex, "Failed to apply webhook {IdempotencyKey} (payment {PaymentId}): {Reason}",
                payload.IdempotencyKey, payload.PaymentId, ex.Message);

            webhookEvent.Status = WebhookEventStatus.Failed;
            webhookEvent.ErrorMessage = ex.Message;
        }

        webhookEventRepository.Update(webhookEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(webhookEvent);
    }

    private static WebhookEventDto ToDto(WebhookEvent w) =>
        new(w.Id, w.IdempotencyKey, w.EventType, w.Status, w.ProcessedAt, w.ErrorMessage);
}
