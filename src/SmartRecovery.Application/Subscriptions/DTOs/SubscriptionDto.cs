using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Subscriptions.DTOs;

public record SubscriptionDto(
    Guid Id,
    Guid CustomerId,
    Guid PlanId,
    string PlanName,
    decimal PlanPrice,
    DateTime StartDate,
    DateTime NextBillingDate,
    DateTime? EndDate,
    SubscriptionStatus Status);
