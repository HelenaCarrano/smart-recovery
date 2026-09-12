using SmartRecovery.Application.Common;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Application.Recovery.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Payments.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    IPaymentAttemptRepository paymentAttemptRepository,
    ISubscriptionRepository subscriptionRepository,
    IPaymentGatewaySimulator gatewaySimulator,
    IRecoveryService recoveryService,
    IUnitOfWork unitOfWork) : IPaymentService
{
    public async Task<PaymentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment '{id}' not found.");

        return ToDto(payment);
    }

    public async Task<IReadOnlyList<PaymentDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var payments = await paymentRepository.GetByCustomerAsync(customerId, cancellationToken);
        return payments.Select(ToDto).ToList();
    }

    public async Task<PagedResult<PaymentListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        PaymentStatus? status,
        DeclineReason? declineReason,
        Guid? customerId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default)
    {
        var filter = new PaymentFilter(page, pageSize, status, declineReason, customerId, dateFrom, dateTo);
        var (items, totalCount) = await paymentRepository.GetPagedAsync(filter, cancellationToken);

        var dtos = items.Select(p => new PaymentListItemDto(
            p.Id, p.CustomerId, p.Customer.Name, p.SubscriptionId, p.Amount, p.Status, p.DeclineReason,
            p.AttemptCount, p.ScheduledRetryAt, p.CreatedAt)).ToList();

        return new PagedResult<PaymentListItemDto>(dtos, page, pageSize, totalCount);
    }

    public async Task<IReadOnlyList<PaymentAttemptDto>> GetAttemptsAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var attempts = await paymentAttemptRepository.GetByPaymentAsync(paymentId, cancellationToken);
        return attempts.Select(a => new PaymentAttemptDto(a.Id, a.AttemptedAt, a.ResultStatus, a.DeclineReason, a.Notes)).ToList();
    }

    public async Task<PaymentDto> CreatePendingForSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.GetWithDetailsAsync(subscriptionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Subscription '{subscriptionId}' not found.");

        var payment = new Payment
        {
            CustomerId = subscription.CustomerId,
            SubscriptionId = subscription.Id,
            Amount = subscription.Plan.Price,
            Status = PaymentStatus.Pending
        };

        await paymentRepository.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(payment);
    }

    public Task<PaymentDto> ProcessAsync(Guid paymentId, CancellationToken cancellationToken = default) =>
        ApplyResultAsync(paymentId, gatewaySimulator.Charge, cancellationToken);

    public Task<PaymentDto> ApplyExternalResultAsync(Guid paymentId, PaymentStatus status, DeclineReason? declineReason, CancellationToken cancellationToken = default) =>
        ApplyResultAsync(paymentId, () => (status, declineReason), cancellationToken);

    private async Task<PaymentDto> ApplyResultAsync(
        Guid paymentId,
        Func<(PaymentStatus Status, DeclineReason? DeclineReason)> resolveResult,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Payment '{paymentId}' not found.");

        if (payment.Status is not (PaymentStatus.Pending or PaymentStatus.Declined))
            throw new InvalidOperationException($"Payment '{paymentId}' cannot be processed from status '{payment.Status}'.");

        var (status, declineReason) = resolveResult();

        payment.AttemptCount += 1;
        payment.Status = status;
        payment.DeclineReason = declineReason;
        if (status == PaymentStatus.Approved)
            payment.ScheduledRetryAt = null;

        var attempt = new PaymentAttempt
        {
            PaymentId = payment.Id,
            ResultStatus = status,
            DeclineReason = declineReason
        };

        paymentRepository.Update(payment);
        await paymentAttemptRepository.AddAsync(attempt, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // AnalyzeAsync opera sobre a mesma instância rastreada pelo DbContext (escopo por requisição/ciclo
        // do worker), então o ScheduledRetryAt que ela define já reflete no `payment` retornado abaixo.
        if (status == PaymentStatus.Declined)
            await recoveryService.AnalyzeAsync(payment.Id, cancellationToken);

        return ToDto(payment);
    }

    private static PaymentDto ToDto(Payment p) => new(
        p.Id, p.CustomerId, p.SubscriptionId, p.Amount, p.Status, p.DeclineReason, p.AttemptCount, p.ScheduledRetryAt, p.CreatedAt);
}
