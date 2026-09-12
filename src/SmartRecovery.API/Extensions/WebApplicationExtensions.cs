using Microsoft.EntityFrameworkCore;
using SmartRecovery.Infrastructure.Data;
using SmartRecovery.Infrastructure.Data.Seed;

namespace SmartRecovery.API.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Aplica migrations pendentes e popula dados de demonstração na inicialização.
    /// Convém para um projeto de portfólio; em produção o ideal seria rodar migrations
    /// como um passo separado do pipeline de deploy.
    /// </summary>
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SmartRecoveryDbContext>();

        await context.Database.MigrateAsync();
        await SmartRecoveryDataSeeder.SeedAsync(context);
    }
}
