namespace SmartRecovery.Application.Customers.DTOs;

public record CustomerListItemDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    bool IsActive,
    int SubscriptionsCount,
    int PaymentsCount);
