using System.Net;
using System.Net.Http.Json;
using SmartRecovery.Application.Payments.DTOs;
using SmartRecovery.Application.Recovery.DTOs;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Integration;

/// <summary>Fluxo completo HTTP → Controller → Application → EF Core → Postgres para criação/processamento de cobranças.</summary>
public class PaymentsApiTests(SmartRecoveryApiFactory factory) : IntegrationTestBase(factory)
{
    private async Task<Subscription> SeedActiveSubscriptionAsync()
    {
        // Não descartamos o DbContext aqui: ele pertence ao escopo compartilhado da base class,
        // que é disposed uma vez ao final do teste (ver IntegrationTestBase.DisposeAsync).
        var db = CreateDbContext();

        var plan = new Plan { Name = "Básico", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly };
        var customer = new Customer { Name = "Cliente Teste", Email = "teste@example.com", Document = "000.000.000-00" };
        var subscription = new Subscription
        {
            CustomerId = customer.Id,
            PlanId = plan.Id,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow,
            Status = SubscriptionStatus.Active
        };

        db.Plans.Add(plan);
        db.Customers.Add(customer);
        db.Subscriptions.Add(subscription);
        await db.SaveChangesAsync();

        return subscription;
    }

    [Fact]
    public async Task CreatePayment_ReturnsCreated_ForActiveSubscription()
    {
        var subscription = await SeedActiveSubscriptionAsync();

        var response = await Client.PostAsync($"/api/payments/subscriptions/{subscription.Id}", content: null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<PaymentDto>(JsonOptions);
        Assert.NotNull(payment);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(49.90m, payment.Amount);
    }

    [Fact]
    public async Task CreatePayment_ReturnsNotFound_ForUnknownSubscription()
    {
        var response = await Client.PostAsync($"/api/payments/subscriptions/{Guid.NewGuid()}", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_Approved_MarksPaymentApproved()
    {
        var subscription = await SeedActiveSubscriptionAsync();
        var created = await (await Client.PostAsync($"/api/payments/subscriptions/{subscription.Id}", content: null))
            .Content.ReadFromJsonAsync<PaymentDto>(JsonOptions);

        Factory.Gateway.NextResult = (PaymentStatus.Approved, null);
        var response = await Client.PostAsync($"/api/payments/{created!.Id}/process", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payment = await response.Content.ReadFromJsonAsync<PaymentDto>(JsonOptions);
        Assert.Equal(PaymentStatus.Approved, payment!.Status);
    }

    [Fact]
    public async Task ProcessPayment_Declined_TriggersRecoveryAnalysis()
    {
        var subscription = await SeedActiveSubscriptionAsync();
        var created = await (await Client.PostAsync($"/api/payments/subscriptions/{subscription.Id}", content: null))
            .Content.ReadFromJsonAsync<PaymentDto>(JsonOptions);

        Factory.Gateway.NextResult = (PaymentStatus.Declined, DeclineReason.ExpiredCard);
        var processResponse = await Client.PostAsync($"/api/payments/{created!.Id}/process", content: null);
        var payment = await processResponse.Content.ReadFromJsonAsync<PaymentDto>(JsonOptions);
        Assert.Equal(PaymentStatus.Declined, payment!.Status);

        // O motor de recuperação roda automaticamente dentro do mesmo request de /process (ver
        // PaymentService.ApplyResultAsync) — a análise já deve existir sem nenhuma chamada adicional.
        var analysisResponse = await Client.GetAsync($"/api/recovery/payments/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, analysisResponse.StatusCode);
        var analysis = await analysisResponse.Content.ReadFromJsonAsync<RecoveryAnalysisDto>(JsonOptions);
        Assert.Equal(created.Id, analysis!.PaymentId);
        Assert.InRange(analysis.RecoveryScore, 0, 100);
    }
}
