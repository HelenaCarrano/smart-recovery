using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.DTOs;

public record PaymentDto(
    Guid Id,
    Guid CustomerId,
    Guid SubscriptionId,
    decimal Amount,
    PaymentStatus Status,
    DeclineReason? DeclineReason,
    int AttemptCount,
    DateTime? ScheduledRetryAt,
    DateTime CreatedAt);
