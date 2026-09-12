using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Subscriptions.DTOs;

/// <summary>Igual a SubscriptionDto, mas com CustomerName e a Periodicity do plano já resolvidos.</summary>
public record SubscriptionListItemDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    Guid PlanId,
    string PlanName,
    decimal PlanPrice,
    PlanPeriodicity Periodicity,
    DateTime StartDate,
    DateTime NextBillingDate,
    DateTime? EndDate,
    SubscriptionStatus Status);
