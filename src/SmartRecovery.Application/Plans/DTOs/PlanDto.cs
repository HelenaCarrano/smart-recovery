using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Plans.DTOs;

public record PlanDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    PlanPeriodicity Periodicity,
    bool IsActive);
