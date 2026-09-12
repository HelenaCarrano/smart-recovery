using Microsoft.EntityFrameworkCore;
using SmartRecovery.Infrastructure.Data;
using SmartRecovery.Infrastructure.Data.Seed;

namespace SmartRecovery.API.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>Roda migrations e seed no startup — conveniente para portfólio; em produção seria um passo separado do deploy.</summary>
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SmartRecoveryDbContext>();

        await context.Database.MigrateAsync();
        await SmartRecoveryDataSeeder.SeedAsync(context);
    }

    /// <summary>Só a migration, sem o seed — usado em testes de integração (ambiente Testing).</summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SmartRecoveryDbContext>();

        await context.Database.MigrateAsync();
    }
}
