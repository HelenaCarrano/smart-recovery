using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Infrastructure.Data.Seed;

/// <summary>
/// Popula o banco com um conjunto mínimo de planos e clientes de demonstração,
/// úteis para explorar a API/Swagger sem precisar cadastrar tudo manualmente.
/// Só insere dados se o banco estiver vazio (idempotente entre reinícios).
/// </summary>
public static class SmartRecoveryDataSeeder
{
    public static async Task SeedAsync(SmartRecoveryDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Plans.AnyAsync(cancellationToken))
            return;

        var plans = new[]
        {
            new Plan { Name = "Básico", Description = "Plano de entrada, cobrança mensal.", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly },
            new Plan { Name = "Profissional", Description = "Plano intermediário, cobrança trimestral.", Price = 129.90m, Periodicity = PlanPeriodicity.Quarterly },
            new Plan { Name = "Enterprise", Description = "Plano completo, cobrança anual.", Price = 999.90m, Periodicity = PlanPeriodicity.Annual }
        };

        var customers = new[]
        {
            new Customer { Name = "Ana Souza", Email = "ana.souza@example.com", Document = "111.111.111-11" },
            new Customer { Name = "Bruno Lima", Email = "bruno.lima@example.com", Document = "222.222.222-22" },
            new Customer { Name = "Carla Mendes", Email = "carla.mendes@example.com", Document = "333.333.333-33" }
        };

        await context.Plans.AddRangeAsync(plans, cancellationToken);
        await context.Customers.AddRangeAsync(customers, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
