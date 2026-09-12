using Microsoft.AspNetCore.Mvc;
using SmartRecovery.Application.Common;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.API.Controllers;

/// <summary>Consulta e processamento de cobranças.</summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    /// <summary>Lista pagamentos paginados, com filtros opcionais por status, motivo da recusa, cliente e período.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<PaymentListItemDto>>> GetPaged(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] PaymentStatus? status,
        [FromQuery] DeclineReason? declineReason,
        [FromQuery] Guid? customerId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken cancellationToken) =>
        Ok(await paymentService.GetPagedAsync(
            page <= 0 ? 1 : page, pageSize <= 0 ? 25 : pageSize, status, declineReason, customerId, dateFrom, dateTo, cancellationToken));

    /// <summary>Busca uma cobrança pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await paymentService.GetByIdAsync(id, cancellationToken));

    /// <summary>Lista o histórico de cobranças de um cliente.</summary>
    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetByCustomer(Guid customerId, CancellationToken cancellationToken) =>
        Ok(await paymentService.GetByCustomerAsync(customerId, cancellationToken));

    /// <summary>Lista as tentativas registradas para uma cobrança.</summary>
    [HttpGet("{id:guid}/attempts")]
    public async Task<ActionResult<IReadOnlyList<PaymentAttemptDto>>> GetAttempts(Guid id, CancellationToken cancellationToken) =>
        Ok(await paymentService.GetAttemptsAsync(id, cancellationToken));

    /// <summary>
    /// Cria uma cobrança Pending para uma assinatura, copiando o valor do plano vigente.
    /// Normalmente disparado automaticamente pelo worker de faturamento; exposto aqui para testes manuais.
    /// </summary>
    [HttpPost("subscriptions/{subscriptionId:guid}")]
    public async Task<ActionResult<PaymentDto>> CreateForSubscription(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var payment = await paymentService.CreatePendingForSubscriptionAsync(subscriptionId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }

    /// <summary>
    /// Processa uma cobrança pendente ou recusada através do gateway simulado.
    /// Em caso de recusa, aciona automaticamente o motor de recuperação.
    /// </summary>
    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<PaymentDto>> Process(Guid id, CancellationToken cancellationToken) =>
        Ok(await paymentService.ProcessAsync(id, cancellationToken));
}
