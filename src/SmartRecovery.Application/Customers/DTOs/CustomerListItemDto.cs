namespace SmartRecovery.Application.Customers.DTOs;

public record CustomerListItemDto(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    int SubscriptionsCount,
    int PaymentsCount);
