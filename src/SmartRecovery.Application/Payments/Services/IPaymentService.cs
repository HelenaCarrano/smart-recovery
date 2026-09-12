using SmartRecovery.Application.Common;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.Services;

public interface IPaymentService
{
    Task<PaymentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<PagedResult<PaymentListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        PaymentStatus? status,
        DeclineReason? declineReason,
        Guid? customerId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentAttemptDto>> GetAttemptsAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>Cria uma cobrança Pending para uma assinatura, copiando o valor do plano vigente.</summary>
    Task<PaymentDto> CreatePendingForSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa uma cobrança pendente: consulta o gateway (simulado), registra a tentativa
    /// e, em caso de recusa, aciona o motor de recuperação automaticamente.
    /// Usado tanto para a primeira tentativa quanto para retries agendados.
    /// </summary>
    Task<PaymentDto> ProcessAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica um resultado vindo de fora (ex: webhook de um provedor real) em vez de simular via gateway.
    /// Segue o mesmo fluxo de <see cref="ProcessAsync"/>: registra a tentativa e aciona o motor de recuperação se recusado.
    /// </summary>
    Task<PaymentDto> ApplyExternalResultAsync(Guid paymentId, PaymentStatus status, DeclineReason? declineReason, CancellationToken cancellationToken = default);
}
