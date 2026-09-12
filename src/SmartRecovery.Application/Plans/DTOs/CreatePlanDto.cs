using System.ComponentModel.DataAnnotations;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Plans.DTOs;

public record CreatePlanDto(
    [property: Required, StringLength(200)] string Name,
    [property: Required, StringLength(1000)] string Description,
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price,
    PlanPeriodicity Periodicity);
