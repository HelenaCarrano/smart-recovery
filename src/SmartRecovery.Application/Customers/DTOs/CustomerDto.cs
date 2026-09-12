namespace SmartRecovery.Application.Customers.DTOs;

public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string Document,
    bool IsActive,
    DateTime CreatedAt);
