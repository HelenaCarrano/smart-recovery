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

    /// <summary>Distribuição de pagamentos recusados por motivo.</summary>
    [HttpGet("decline-reasons")]
    public async Task<ActionResult<IReadOnlyList<DeclineReasonCountDto>>> GetDeclineReasons(CancellationToken cancellationToken) =>
        Ok(await dashboardService.GetDeclineReasonBreakdownAsync(cancellationToken));

    /// <summary>Aprovados x recusados por dia, nos últimos <paramref name="days"/> dias (padrão 90).</summary>
    [HttpGet("trends")]
    public async Task<ActionResult<IReadOnlyList<PaymentTrendPointDto>>> GetTrends([FromQuery] int days, CancellationToken cancellationToken) =>
        Ok(await dashboardService.GetTrendAsync(days <= 0 ? 90 : days, cancellationToken));
}
