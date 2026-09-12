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
        // A connection string é resolvida dentro do callback (não antes) porque ele só roda quando o
        // DbContext é de fato construído — já com o IConfiguration final. Lendo-a antecipadamente aqui
        // (fora do callback), testes de integração que sobrescrevem "ConnectionStrings:DefaultConnection"
        // via WebApplicationFactory.ConfigureAppConfiguration perderiam o valor sobrescrito, pois esse
        // hook só é aplicado no Build() do host, que acontece depois desta chamada a AddInfrastructure.
        services.AddDbContext<SmartRecoveryDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["DATABASE_URL"]
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DATABASE_URL");

            options.UseNpgsql(connectionString);
        });

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
