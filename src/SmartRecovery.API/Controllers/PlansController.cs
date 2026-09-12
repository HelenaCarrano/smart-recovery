using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Plans.DTOs;
using SmartRecovery.Application.Plans.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Gerenciamento de planos de assinatura.</summary>
[ApiController]
[Route("api/[controller]")]
public class PlansController(IPlanService planService) : ControllerBase
{
    /// <summary>Lista os planos ativos disponíveis para assinatura.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanDto>>> GetActive(CancellationToken cancellationToken) =>
        Ok(await planService.GetActiveAsync(cancellationToken));

    /// <summary>Busca um plano pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlanDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await planService.GetByIdAsync(id, cancellationToken));

    /// <summary>Cadastra um novo plano.</summary>
    [HttpPost]
    public async Task<ActionResult<PlanDto>> Create(CreatePlanDto dto, CancellationToken cancellationToken)
    {
        var plan = await planService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
    }
}
