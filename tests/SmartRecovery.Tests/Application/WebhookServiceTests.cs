using Microsoft.Extensions.Logging;
using Moq;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Application.Webhooks.DTOs;
using SmartRecovery.Application.Webhooks.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class WebhookServiceTests
{
    private readonly Mock<IWebhookEventRepository> _webhookEventRepository = new();
    private readonly Mock<IPaymentService> _paymentService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly WebhookService _sut;

    public WebhookServiceTests()
    {
        _sut = new WebhookService(
            _webhookEventRepository.Object,
            _paymentService.Object,
            _unitOfWork.Object,
            Mock.Of<ILogger<WebhookService>>());
    }

    [Fact]
    public async Task ProcessPaymentWebhookAsync_ReturnsExistingEvent_WithoutReprocessing_WhenIdempotencyKeyAlreadySeen()
    {
        var existing = new WebhookEvent { IdempotencyKey = "evt-1", EventType = "payment.declined", Status = WebhookEventStatus.Processed };
        _webhookEventRepository
            .Setup(r => r.GetByIdempotencyKeyAsync("evt-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var payload = new WebhookPayloadDto("evt-1", "payment.declined", Guid.NewGuid(), PaymentStatus.Declined, DeclineReason.InsufficientFunds);

        var result = await _sut.ProcessPaymentWebhookAsync(payload);

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal(WebhookEventStatus.Processed, result.Status);
        _paymentService.Verify(p => p.ApplyExternalResultAsync(It.IsAny<Guid>(), It.IsAny<PaymentStatus>(), It.IsAny<DeclineReason?>(), It.IsAny<CancellationToken>()), Times.Never);
        _webhookEventRepository.Verify(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessPaymentWebhookAsync_MarksProcessed_WhenPaymentServiceSucceeds()
    {
        _webhookEventRepository
            .Setup(r => r.GetByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebhookEvent?)null);

        var paymentId = Guid.NewGuid();
        var payload = new WebhookPayloadDto("evt-2", "payment.approved", paymentId, PaymentStatus.Approved, null);

        _paymentService
            .Setup(p => p.ApplyExternalResultAsync(paymentId, PaymentStatus.Approved, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentDto(paymentId, Guid.NewGuid(), Guid.NewGuid(), 100m, PaymentStatus.Approved, null, 1, null, DateTime.UtcNow));

        var result = await _sut.ProcessPaymentWebhookAsync(payload);

        Assert.Equal(WebhookEventStatus.Processed, result.Status);
        Assert.NotNull(result.ProcessedAt);
        Assert.Null(result.ErrorMessage);
        _webhookEventRepository.Verify(r => r.AddAsync(It.IsAny<WebhookEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessPaymentWebhookAsync_MarksFailed_WhenPaymentDoesNotExist_ButDoesNotThrow()
    {
        _webhookEventRepository
            .Setup(r => r.GetByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WebhookEvent?)null);

        var payload = new WebhookPayloadDto("evt-3", "payment.approved", Guid.NewGuid(), PaymentStatus.Approved, null);

        _paymentService
            .Setup(p => p.ApplyExternalResultAsync(payload.PaymentId, payload.Status, payload.DeclineReason, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"Payment '{payload.PaymentId}' not found."));

        var result = await _sut.ProcessPaymentWebhookAsync(payload);

        Assert.Equal(WebhookEventStatus.Failed, result.Status);
        Assert.NotNull(result.ErrorMessage);
        Assert.Null(result.ProcessedAt);
    }
}
