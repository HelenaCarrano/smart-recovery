using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Webhooks.DTOs;

public record WebhookEventDto(
    Guid Id,
    string IdempotencyKey,
    string EventType,
    WebhookEventStatus Status,
    DateTime? ProcessedAt,
    string? ErrorMessage);
