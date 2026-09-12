using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.BackgroundServices;
using SmartRecovery.Infrastructure.Data;
using SmartRecovery.Infrastructure.Repositories;

namespace SmartRecovery.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SmartRecoveryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SmartRecoveryDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
        services.AddScoped<IRecoveryAnalysisRepository, RecoveryAnalysisRepository>();
        services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();

        services.Configure<PaymentRetryWorkerOptions>(
            configuration.GetSection(PaymentRetryWorkerOptions.SectionName));
        services.AddHostedService<PaymentRetryWorker>();

        return services;
    }
}
