using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Plans.DTOs;

public record CreatePlanDto(string Name, string Description, decimal Price, PlanPeriodicity Periodicity);
