using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Recovery.DTOs;
using SmartRecovery.Application.Recovery.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Consulta das análises de recuperação geradas pelo Recovery Score Engine.</summary>
[ApiController]
[Route("api/[controller]")]
public class RecoveryController(IRecoveryService recoveryService) : ControllerBase
{
    /// <summary>Busca a análise de recuperação de um pagamento recusado.</summary>
    [HttpGet("payments/{paymentId:guid}")]
    public async Task<ActionResult<RecoveryAnalysisDto>> GetByPayment(Guid paymentId, CancellationToken cancellationToken)
    {
        var analysis = await recoveryService.GetByPaymentAsync(paymentId, cancellationToken);
        return analysis is null ? NotFound() : Ok(analysis);
    }

    /// <summary>
    /// Lista análises cuja ação recomendada ainda não foi executada — ex: RequestPaymentMethodUpdate
    /// aguardando o cliente, ou ManualReview aguardando um analista.
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<RecoveryAnalysisDto>>> GetPending(CancellationToken cancellationToken) =>
        Ok(await recoveryService.GetPendingExecutionAsync(cancellationToken));
}
