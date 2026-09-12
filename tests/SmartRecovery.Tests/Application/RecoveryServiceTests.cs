using Moq;
using SmartRecovery.Application.Recovery.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class RecoveryServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepository = new();
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository = new();
    private readonly Mock<IRecoveryAnalysisRepository> _recoveryAnalysisRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RecoveryService _sut;

    public RecoveryServiceTests()
    {
        _sut = new RecoveryService(
            _paymentRepository.Object,
            _subscriptionRepository.Object,
            _recoveryAnalysisRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task AnalyzeAsync_ThrowsKeyNotFound_WhenPaymentDoesNotExist()
    {
        _paymentRepository
            .Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AnalyzeAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AnalyzeAsync_ThrowsInvalidOperation_WhenPaymentIsNotDeclined()
    {
        var payment = new Payment { Status = PaymentStatus.Approved };
        _paymentRepository
            .Setup(r => r.GetWithDetailsAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AnalyzeAsync(payment.Id));
    }

    [Fact]
    public async Task AnalyzeAsync_HighScoreTemporaryError_SchedulesRetryAndMarksExecuted()
    {
        var customerId = Guid.NewGuid();
        var payment = new Payment
        {
            CustomerId = customerId,
            SubscriptionId = Guid.NewGuid(),
            Status = PaymentStatus.Declined,
            DeclineReason = DeclineReason.TemporaryError,
            AttemptCount = 1
        };

        _paymentRepository
            .Setup(r => r.GetWithDetailsAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        // Histórico vazio (cliente novo): a análise não deve ser penalizada, mantendo o score base do motivo.
        _paymentRepository
            .Setup(r => r.GetHistoryByCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.AnalyzeAsync(payment.Id);

        Assert.Equal(RecoveryAction.RetryIn2Hours, result.RecommendedAction);
        Assert.NotNull(payment.ScheduledRetryAt);
        Assert.NotNull(result.ExecutedAt);
        _recoveryAnalysisRepository.Verify(r => r.AddAsync(It.IsAny<RecoveryAnalysis>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AnalyzeAsync_BlockedCard_RecommendsManualReviewAndDoesNotAutomate()
    {
        var customerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var payment = new Payment
        {
            CustomerId = customerId,
            SubscriptionId = subscriptionId,
            Status = PaymentStatus.Declined,
            DeclineReason = DeclineReason.BlockedCard,
            AttemptCount = 1
        };
        var subscription = new Subscription { Id = subscriptionId, Status = SubscriptionStatus.Active };

        _paymentRepository
            .Setup(r => r.GetWithDetailsAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _paymentRepository
            .Setup(r => r.GetHistoryByCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([payment]);
        _subscriptionRepository
            .Setup(r => r.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var result = await _sut.AnalyzeAsync(payment.Id);

        Assert.Equal(RecoveryAction.ManualReview, result.RecommendedAction);
        Assert.Null(payment.ScheduledRetryAt);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
    }

    [Fact]
    public async Task AnalyzeAsync_VeryLowScore_CancelsSubscriptionAndMarksExecuted()
    {
        var customerId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var payment = new Payment
        {
            CustomerId = customerId,
            SubscriptionId = subscriptionId,
            Status = PaymentStatus.Declined,
            DeclineReason = DeclineReason.InvalidCard,
            AttemptCount = 1
        };
        var subscription = new Subscription { Id = subscriptionId, Status = SubscriptionStatus.Active };

        // Histórico ruim o suficiente para derrubar o score base (35) abaixo de 30.
        var badHistory = Enumerable.Range(0, 5)
            .Select(_ => new Payment { CustomerId = customerId, Status = PaymentStatus.Declined, CreatedAt = DateTime.UtcNow })
            .Append(payment)
            .ToList();

        _paymentRepository
            .Setup(r => r.GetWithDetailsAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _paymentRepository
            .Setup(r => r.GetHistoryByCustomerAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(badHistory);
        _subscriptionRepository
            .Setup(r => r.GetByIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var result = await _sut.AnalyzeAsync(payment.Id);

        Assert.Equal(RecoveryAction.CancelSubscription, result.RecommendedAction);
        Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        Assert.NotNull(subscription.EndDate);
        Assert.NotNull(result.ExecutedAt);
    }
}
