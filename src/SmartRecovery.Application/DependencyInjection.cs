using Microsoft.Extensions.DependencyInjection;
using SmartRecovery.Application.Customers.Services;
using SmartRecovery.Application.Dashboard.Services;
using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Application.Plans.Services;
using SmartRecovery.Application.Recovery.Services;
using SmartRecovery.Application.Subscriptions.Services;
using SmartRecovery.Application.Webhooks.Services;

namespace SmartRecovery.Application;

/// <summary>Ponto único de registro de todos os serviços da camada de Application na DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IRecoveryService, RecoveryService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddSingleton<IPaymentGatewaySimulator, PaymentGatewaySimulator>();

        return services;
    }
}
