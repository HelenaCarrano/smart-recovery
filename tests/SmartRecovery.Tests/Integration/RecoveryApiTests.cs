using System.Net;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Integration;

public class RecoveryApiTests(SmartRecoveryApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetByPayment_ReturnsNotFound_WhenPaymentHasNoAnalysis()
    {
        var db = CreateDbContext();
        var plan = new Plan { Name = "Básico", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly };
        var customer = new Customer { Name = "Cliente Teste", Email = "teste@example.com", Document = "000.000.000-00" };
        var subscription = new Subscription { CustomerId = customer.Id, PlanId = plan.Id, Status = SubscriptionStatus.Active };
        var payment = new Payment { CustomerId = customer.Id, SubscriptionId = subscription.Id, Amount = 49.90m, Status = PaymentStatus.Approved };

        db.Plans.Add(plan);
        db.Customers.Add(customer);
        db.Subscriptions.Add(subscription);
        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        var response = await Client.GetAsync($"/api/recovery/payments/{payment.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByPayment_ReturnsNotFound_WhenPaymentDoesNotExist()
    {
        var response = await Client.GetAsync($"/api/recovery/payments/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
