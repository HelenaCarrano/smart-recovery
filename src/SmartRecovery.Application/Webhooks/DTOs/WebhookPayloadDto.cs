using System.ComponentModel.DataAnnotations;
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
    DeclineReason? DeclineReason) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(IdempotencyKey))
            yield return new ValidationResult("IdempotencyKey é obrigatório.", [nameof(IdempotencyKey)]);

        if (string.IsNullOrWhiteSpace(EventType))
            yield return new ValidationResult("EventType é obrigatório.", [nameof(EventType)]);

        if (PaymentId == Guid.Empty)
            yield return new ValidationResult("PaymentId é obrigatório.", [nameof(PaymentId)]);

        // Um provedor de pagamentos real nunca envia esses dois campos de forma inconsistente,
        // mas como o payload chega de fora, validamos explicitamente em vez de confiar nele.
        if (Status == PaymentStatus.Declined && DeclineReason is null)
            yield return new ValidationResult("DeclineReason é obrigatório quando Status é Declined.", [nameof(DeclineReason)]);

        if (Status != PaymentStatus.Declined && DeclineReason is not null)
            yield return new ValidationResult("DeclineReason só é válido quando Status é Declined.", [nameof(DeclineReason)]);
    }
}
