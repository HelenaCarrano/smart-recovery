using SmartRecovery.Application.Common;
using SmartRecovery.Application.Subscriptions.DTOs;
using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Subscriptions.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    ICustomerRepository customerRepository,
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork) : ISubscriptionService
{
    public async Task<SubscriptionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Subscription '{id}' not found.");

        return ToDto(subscription);
    }

    public async Task<IReadOnlyList<SubscriptionDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var subscriptions = await subscriptionRepository.GetByCustomerAsync(customerId, cancellationToken);
        return subscriptions.Select(ToDto).ToList();
    }

    public async Task<PagedResult<SubscriptionListItemDto>> GetPagedAsync(int page, int pageSize, SubscriptionStatus? status, CancellationToken cancellationToken = default)
    {
        var (subscriptions, totalCount) = await subscriptionRepository.GetPagedAsync(page, pageSize, status, cancellationToken);

        var items = subscriptions.Select(s => new SubscriptionListItemDto(
            s.Id, s.CustomerId, s.Customer.Name, s.PlanId, s.Plan.Name, s.Plan.Price, s.Plan.Periodicity,
            s.StartDate, s.NextBillingDate, s.EndDate, s.Status)).ToList();

        return new PagedResult<SubscriptionListItemDto>(items, page, pageSize, totalCount);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(dto.CustomerId, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer '{dto.CustomerId}' not found.");

        if (!customer.IsActive)
            throw new InvalidOperationException("Cannot create a subscription for an inactive customer.");

        var plan = await planRepository.GetByIdAsync(dto.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException($"Plan '{dto.PlanId}' not found.");

        if (!plan.IsActive)
            throw new InvalidOperationException("Cannot subscribe to an inactive plan.");

        var now = DateTime.UtcNow;
        var subscription = new Subscription
        {
            CustomerId = customer.Id,
            PlanId = plan.Id,
            StartDate = now,
            NextBillingDate = BillingCycleCalculator.NextBillingDate(now, plan.Periodicity),
            Status = SubscriptionStatus.Active
        };

        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        subscription.Plan = plan;
        return ToDto(subscription);
    }

    public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Subscription '{id}' not found.");

        if (subscription.Status == SubscriptionStatus.Cancelled)
            return;

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.EndDate = DateTime.UtcNow;

        subscriptionRepository.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<SubscriptionsSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var total = await subscriptionRepository.CountAsync(cancellationToken);
        var active = await subscriptionRepository.GetByStatusAsync(SubscriptionStatus.Active, cancellationToken);
        var paused = await subscriptionRepository.CountByStatusAsync(SubscriptionStatus.Paused, cancellationToken);
        var cancelled = await subscriptionRepository.CountByStatusAsync(SubscriptionStatus.Cancelled, cancellationToken);

        var monthlyRecurringRevenue = active.Sum(s => BillingCycleCalculator.MonthlyEquivalent(s.Plan.Price, s.Plan.Periodicity));

        var planDistribution = active
            .GroupBy(s => s.Plan)
            .Select(g =>
            {
                var monthlyRevenue = g.Sum(s => BillingCycleCalculator.MonthlyEquivalent(s.Plan.Price, s.Plan.Periodicity));
                return new PlanDistributionDto(
                    g.Key.Id,
                    g.Key.Name,
                    g.Count(),
                    monthlyRevenue,
                    monthlyRecurringRevenue > 0 ? (double)(monthlyRevenue / monthlyRecurringRevenue) * 100 : 0);
            })
            .OrderByDescending(p => p.MonthlyRevenue)
            .ToList();

        return new SubscriptionsSummaryDto(
            total,
            active.Count,
            total > 0 ? (double)active.Count / total * 100 : 0,
            paused,
            cancelled,
            total > 0 ? (double)cancelled / total * 100 : 0,
            monthlyRecurringRevenue,
            planDistribution);
    }

    private static SubscriptionDto ToDto(Subscription s) => new(
        s.Id, s.CustomerId, s.PlanId, s.Plan.Name, s.Plan.Price, s.StartDate, s.NextBillingDate, s.EndDate, s.Status);
}
