using System.Net;
using System.Net.Http.Json;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Application.Webhooks.DTOs;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Integration;

public class WebhooksApiTests(SmartRecoveryApiFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Payment> SeedPendingPaymentAsync()
    {
        var db = CreateDbContext();
        var plan = new Plan { Name = "Básico", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly };
        var customer = new Customer { Name = "Cliente Teste", Email = "teste@example.com", Document = "000.000.000-00" };
        var subscription = new Subscription { CustomerId = customer.Id, PlanId = plan.Id, Status = SubscriptionStatus.Active };
        var payment = new Payment { CustomerId = customer.Id, SubscriptionId = subscription.Id, Amount = 49.90m, Status = PaymentStatus.Pending };

        db.Plans.Add(plan);
        db.Customers.Add(customer);
        db.Subscriptions.Add(subscription);
        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        return payment;
    }

    [Fact]
    public async Task ReceivePaymentEvent_Valid_ProcessesAndUpdatesPayment()
    {
        var payment = await SeedPendingPaymentAsync();
        var payload = new WebhookPayloadDto("wh-valid-1", "payment.result", payment.Id, PaymentStatus.Approved, null);

        var response = await Client.PostAsJsonAsync("/api/webhooks/payments", payload, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<WebhookEventDto>(JsonOptions);
        Assert.Equal(WebhookEventStatus.Processed, result!.Status);

        var updatedPayment = await Client.GetFromJsonAsync<PaymentDto>($"/api/payments/{payment.Id}", JsonOptions);
        Assert.Equal(PaymentStatus.Approved, updatedPayment!.Status);
    }

    [Fact]
    public async Task ReceivePaymentEvent_DuplicateIdempotencyKey_ReturnsSameResultWithoutReprocessing()
    {
        var payment = await SeedPendingPaymentAsync();
        var payload = new WebhookPayloadDto("wh-duplicate-1", "payment.result", payment.Id, PaymentStatus.Approved, null);

        var first = await (await Client.PostAsJsonAsync("/api/webhooks/payments", payload, JsonOptions)).Content.ReadFromJsonAsync<WebhookEventDto>(JsonOptions);
        var second = await (await Client.PostAsJsonAsync("/api/webhooks/payments", payload, JsonOptions)).Content.ReadFromJsonAsync<WebhookEventDto>(JsonOptions);

        // Mesmo evento (mesma IdempotencyKey) reenviado: deve retornar o mesmo registro, não criar um novo.
        Assert.Equal(first!.Id, second!.Id);

        var attempts = await Client.GetFromJsonAsync<List<object>>($"/api/payments/{payment.Id}/attempts", JsonOptions);
        Assert.Single(attempts!);
    }

    [Fact]
    public async Task ReceivePaymentEvent_InvalidPayload_ReturnsValidationProblem()
    {
        var payload = new WebhookPayloadDto("", "", Guid.Empty, PaymentStatus.Declined, null);

        var response = await Client.PostAsJsonAsync("/api/webhooks/payments", payload, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("IdempotencyKey", body);
        Assert.Contains("DeclineReason", body);
    }
}
