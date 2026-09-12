using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Subscriptions.DTOs;
using SmartRecovery.Application.Subscriptions.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Gerenciamento de assinaturas.</summary>
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController(ISubscriptionService subscriptionService) : ControllerBase
{
    /// <summary>Busca uma assinatura pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubscriptionDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await subscriptionService.GetByIdAsync(id, cancellationToken));

    /// <summary>Lista as assinaturas de um cliente.</summary>
    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<ActionResult<IReadOnlyList<SubscriptionDto>>> GetByCustomer(Guid customerId, CancellationToken cancellationToken) =>
        Ok(await subscriptionService.GetByCustomerAsync(customerId, cancellationToken));

    /// <summary>Cria uma nova assinatura, vinculando um cliente a um plano.</summary>
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionDto dto, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    /// <summary>Cancela uma assinatura ativa.</summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await subscriptionService.CancelAsync(id, cancellationToken);
        return NoContent();
    }
}
