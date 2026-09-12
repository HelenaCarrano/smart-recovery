using Moq;
using SmartRecovery.Application.Subscriptions.DTOs;
using SmartRecovery.Application.Subscriptions.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IPlanRepository> _planRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly SubscriptionService _sut;

    public SubscriptionServiceTests()
    {
        _sut = new SubscriptionService(
            _subscriptionRepository.Object,
            _customerRepository.Object,
            _planRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_ThrowsKeyNotFound_WhenCustomerDoesNotExist()
    {
        var dto = new CreateSubscriptionDto(Guid.NewGuid(), Guid.NewGuid());
        _customerRepository.Setup(r => r.GetByIdAsync(dto.CustomerId, It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperation_WhenCustomerIsInactive()
    {
        var customer = new Customer { IsActive = false };
        var dto = new CreateSubscriptionDto(customer.Id, Guid.NewGuid());
        _customerRepository.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperation_WhenPlanIsInactive()
    {
        var customer = new Customer { IsActive = true };
        var plan = new Plan { IsActive = false, Periodicity = PlanPeriodicity.Monthly };
        var dto = new CreateSubscriptionDto(customer.Id, plan.Id);
        _customerRepository.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _planRepository.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(plan);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_PersistsSubscription_WithNextBillingDateFromPlan()
    {
        var customer = new Customer { IsActive = true };
        var plan = new Plan { Name = "Básico", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly, IsActive = true };
        var dto = new CreateSubscriptionDto(customer.Id, plan.Id);
        _customerRepository.Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _planRepository.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>())).ReturnsAsync(plan);

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(SubscriptionStatus.Active, result.Status);
        Assert.True(result.NextBillingDate > result.StartDate);
        _subscriptionRepository.Verify(r => r.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_ThrowsKeyNotFound_WhenSubscriptionDoesNotExist()
    {
        _subscriptionRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Subscription?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CancelAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CancelAsync_IsIdempotent_WhenAlreadyCancelled()
    {
        var subscription = new Subscription { Status = SubscriptionStatus.Cancelled };
        _subscriptionRepository.Setup(r => r.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>())).ReturnsAsync(subscription);

        await _sut.CancelAsync(subscription.Id);

        _subscriptionRepository.Verify(r => r.Update(It.IsAny<Subscription>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelAsync_SetsCancelledStatusAndEndDate()
    {
        var subscription = new Subscription { Status = SubscriptionStatus.Active };
        _subscriptionRepository.Setup(r => r.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>())).ReturnsAsync(subscription);

        await _sut.CancelAsync(subscription.Id);

        Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        Assert.NotNull(subscription.EndDate);
        _subscriptionRepository.Verify(r => r.Update(subscription), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSummaryAsync_ComputesMrrAndPlanDistribution_FromActiveSubscriptionsOnly()
    {
        var monthlyPlan = new Plan { Name = "Básico", Price = 60m, Periodicity = PlanPeriodicity.Monthly };
        var annualPlan = new Plan { Name = "Enterprise", Price = 1200m, Periodicity = PlanPeriodicity.Annual };

        var active = new List<Subscription>
        {
            new() { Plan = monthlyPlan, Status = SubscriptionStatus.Active },
            new() { Plan = monthlyPlan, Status = SubscriptionStatus.Active },
            new() { Plan = annualPlan, Status = SubscriptionStatus.Active },
        };

        _subscriptionRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _subscriptionRepository.Setup(r => r.GetByStatusAsync(SubscriptionStatus.Active, It.IsAny<CancellationToken>())).ReturnsAsync(active);
        _subscriptionRepository.Setup(r => r.CountByStatusAsync(SubscriptionStatus.Paused, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _subscriptionRepository.Setup(r => r.CountByStatusAsync(SubscriptionStatus.Cancelled, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.GetSummaryAsync();

        Assert.Equal(5, result.TotalSubscriptions);
        Assert.Equal(3, result.ActiveSubscriptions);
        Assert.Equal(1, result.PausedSubscriptions);
        Assert.Equal(1, result.CancelledSubscriptions);
        // MRR: 2x R$60 (mensal, sem ajuste) + R$1200/12 (anual normalizado) = 220.
        Assert.Equal(220m, result.MonthlyRecurringRevenue);
        Assert.Equal(2, result.PlanDistribution.Count);

        var basico = Assert.Single(result.PlanDistribution, p => p.PlanName == "Básico");
        Assert.Equal(2, basico.ActiveSubscriptionsCount);
        Assert.Equal(120m, basico.MonthlyRevenue);
    }

    [Fact]
    public async Task GetSummaryAsync_HandlesNoActiveSubscriptions_WithoutDivideByZero()
    {
        _subscriptionRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _subscriptionRepository.Setup(r => r.GetByStatusAsync(SubscriptionStatus.Active, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _subscriptionRepository.Setup(r => r.CountByStatusAsync(SubscriptionStatus.Paused, It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _subscriptionRepository.Setup(r => r.CountByStatusAsync(SubscriptionStatus.Cancelled, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var result = await _sut.GetSummaryAsync();

        Assert.Equal(0, result.ActivePercentage);
        Assert.Equal(0, result.CancellationRate);
        Assert.Empty(result.PlanDistribution);
    }
}
