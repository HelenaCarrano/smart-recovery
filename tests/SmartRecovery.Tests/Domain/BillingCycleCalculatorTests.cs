using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Domain;

public class BillingCycleCalculatorTests
{
    [Theory]
    [InlineData(PlanPeriodicity.Monthly, 30)]
    [InlineData(PlanPeriodicity.Quarterly, 90)]
    [InlineData(PlanPeriodicity.Annual, 365)]
    public void NextBillingDate_AddsExpectedNumberOfDays(PlanPeriodicity periodicity, int expectedDays)
    {
        var from = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var next = BillingCycleCalculator.NextBillingDate(from, periodicity);

        Assert.Equal(from.AddDays(expectedDays), next);
    }
}
