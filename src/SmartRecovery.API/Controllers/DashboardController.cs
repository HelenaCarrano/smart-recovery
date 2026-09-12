using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Dashboard.DTOs;
using SmartRecovery.Application.Dashboard.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Métricas agregadas para a tela principal do dashboard.</summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>Retorna o resumo geral: clientes, assinaturas, pagamentos e taxa de recuperação.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken) =>
        Ok(await dashboardService.GetSummaryAsync(cancellationToken));
}
