using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.DTOs;

public record PaymentAttemptDto(
    Guid Id,
    DateTime AttemptedAt,
    PaymentStatus ResultStatus,
    DeclineReason? DeclineReason,
    string? Notes);
