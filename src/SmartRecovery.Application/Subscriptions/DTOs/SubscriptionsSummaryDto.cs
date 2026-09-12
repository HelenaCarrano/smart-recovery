namespace SmartRecovery.Application.Subscriptions.DTOs;

/// <summary>Participação de um plano na base de assinaturas ativas.</summary>
public record PlanDistributionDto(
    Guid PlanId,
    string PlanName,
    int ActiveSubscriptionsCount,
    decimal MonthlyRevenue,
    double SharePercentage);

/// <summary>Indicadores consolidados para o dashboard de assinaturas.</summary>
public record SubscriptionsSummaryDto(
    int TotalSubscriptions,
    int ActiveSubscriptions,
    double ActivePercentage,
    int PausedSubscriptions,
    int CancelledSubscriptions,
    double CancellationRate,
    decimal MonthlyRecurringRevenue,
    IReadOnlyList<PlanDistributionDto> PlanDistribution);
