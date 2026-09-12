using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.DTOs;

/// <summary>Igual a PaymentDto, mas com CustomerName já resolvido — evita que o consumidor precise buscar o cliente à parte.</summary>
public record PaymentListItemDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    Guid SubscriptionId,
    decimal Amount,
    PaymentStatus Status,
    DeclineReason? DeclineReason,
    int AttemptCount,
    DateTime? ScheduledRetryAt,
    DateTime CreatedAt);
