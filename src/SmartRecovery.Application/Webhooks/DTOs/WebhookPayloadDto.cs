using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Webhooks.DTOs;

/// <summary>
/// Corpo esperado em POST /api/webhooks/payments. Simula o payload que um provedor de
/// pagamentos real enviaria ao notificar o resultado de uma cobrança.
/// </summary>
public record WebhookPayloadDto(
    string IdempotencyKey,
    string EventType,
    Guid PaymentId,
    PaymentStatus Status,
    DeclineReason? DeclineReason);
