using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Webhooks.DTOs;
using SmartRecovery.Application.Webhooks.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Recebe eventos de webhook de provedores de pagamento (simulados).</summary>
[ApiController]
[Route("api/webhooks")]
public class WebhooksController(IWebhookService webhookService) : ControllerBase
{
    /// <summary>
    /// Recebe o resultado de uma cobrança. Idempotente por IdempotencyKey: reenvios do mesmo
    /// evento são reconhecidos e não reprocessados.
    /// </summary>
    [HttpPost("payments")]
    public async Task<ActionResult<WebhookEventDto>> ReceivePaymentEvent(WebhookPayloadDto payload, CancellationToken cancellationToken) =>
        Ok(await webhookService.ProcessPaymentWebhookAsync(payload, cancellationToken));
}
