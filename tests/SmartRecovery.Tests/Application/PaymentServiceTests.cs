using Moq;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Application.Recovery.DTOs;
using SmartRecovery.Application.Recovery.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepository = new();
    private readonly Mock<IPaymentAttemptRepository> _paymentAttemptRepository = new();
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository = new();
    private readonly Mock<IPaymentGatewaySimulator> _gatewaySimulator = new();
    private readonly Mock<IRecoveryService> _recoveryService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _sut = new PaymentService(
            _paymentRepository.Object,
            _paymentAttemptRepository.Object,
            _subscriptionRepository.Object,
            _gatewaySimulator.Object,
            _recoveryService.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreatePendingForSubscriptionAsync_ThrowsKeyNotFound_WhenSubscriptionDoesNotExist()
    {
        _subscriptionRepository
            .Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreatePendingForSubscriptionAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreatePendingForSubscriptionAsync_CopiesAmountFromPlan()
    {
        var plan = new Plan { Price = 129.90m };
        var subscription = new Subscription { Plan = plan, CustomerId = Guid.NewGuid() };
        _subscriptionRepository
            .Setup(r => r.GetWithDetailsAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var result = await _sut.CreatePendingForSubscriptionAsync(subscription.Id);

        Assert.Equal(plan.Price, result.Amount);
        Assert.Equal(PaymentStatus.Pending, result.Status);
        _paymentRepository.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_ThrowsKeyNotFound_WhenPaymentDoesNotExist()
    {
        _paymentRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ProcessAsync(Guid.NewGuid()));
    }

    [Theory]
    [InlineData(PaymentStatus.Approved)]
    [InlineData(PaymentStatus.Cancelled)]
    [InlineData(PaymentStatus.Refunded)]
    public async Task ProcessAsync_ThrowsInvalidOperation_WhenPaymentIsNotPendingOrDeclined(PaymentStatus status)
    {
        var payment = new Payment { Status = status };
        _paymentRepository.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ProcessAsync(payment.Id));
    }

    [Fact]
    public async Task ProcessAsync_Approved_ClearsScheduledRetryAndSkipsRecoveryAnalysis()
    {
        var payment = new Payment { Status = PaymentStatus.Pending, ScheduledRetryAt = DateTime.UtcNow.AddHours(2) };
        _paymentRepository.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _gatewaySimulator.Setup(g => g.Charge()).Returns((PaymentStatus.Approved, (DeclineReason?)null));

        var result = await _sut.ProcessAsync(payment.Id);

        Assert.Equal(PaymentStatus.Approved, result.Status);
        Assert.Null(payment.ScheduledRetryAt);
        Assert.Equal(1, payment.AttemptCount);
        _paymentAttemptRepository.Verify(r => r.AddAsync(It.Is<PaymentAttempt>(a => a.ResultStatus == PaymentStatus.Approved), It.IsAny<CancellationToken>()), Times.Once);
        _recoveryService.Verify(r => r.AnalyzeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_Declined_TriggersRecoveryAnalysis()
    {
        var payment = new Payment { Status = PaymentStatus.Pending };
        _paymentRepository.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _gatewaySimulator.Setup(g => g.Charge()).Returns((PaymentStatus.Declined, DeclineReason.InsufficientFunds));
        _recoveryService
            .Setup(r => r.AnalyzeAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecoveryAnalysisDto(
                Guid.NewGuid(), payment.Id, 70, RecoveryAction.RetryIn72Hours, DateTime.UtcNow, null,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        var result = await _sut.ProcessAsync(payment.Id);

        Assert.Equal(PaymentStatus.Declined, result.Status);
        Assert.Equal(DeclineReason.InsufficientFunds, result.DeclineReason);
        _recoveryService.Verify(r => r.AnalyzeAsync(payment.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyExternalResultAsync_AppliesGivenStatus_WithoutCallingGateway()
    {
        var payment = new Payment { Status = PaymentStatus.Declined };
        _paymentRepository.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var result = await _sut.ApplyExternalResultAsync(payment.Id, PaymentStatus.Approved, null);

        Assert.Equal(PaymentStatus.Approved, result.Status);
        _gatewaySimulator.Verify(g => g.Charge(), Times.Never);
    }

    [Fact]
    public async Task GetAttemptsAsync_ReturnsMappedAttempts()
    {
        var paymentId = Guid.NewGuid();
        var attempts = new List<PaymentAttempt>
        {
            new() { PaymentId = paymentId, ResultStatus = PaymentStatus.Declined, DeclineReason = DeclineReason.ExpiredCard },
            new() { PaymentId = paymentId, ResultStatus = PaymentStatus.Approved },
        };
        _paymentAttemptRepository.Setup(r => r.GetByPaymentAsync(paymentId, It.IsAny<CancellationToken>())).ReturnsAsync(attempts);

        var result = await _sut.GetAttemptsAsync(paymentId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.DeclineReason == DeclineReason.ExpiredCard);
    }
}
