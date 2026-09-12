using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using SmartRecovery.Application.Payments.Services;

namespace SmartRecovery.Tests.Integration;

/// <summary>
/// Sobe a API real (Program.cs) apontando para um banco Postgres separado (smart_recovery_test),
/// no mesmo servidor local já usado em desenvolvimento — não há Docker/Testcontainers disponível
/// neste ambiente. O banco de teste é criado se não existir, migrado uma vez, e o Respawn reseta
/// as tabelas entre testes (ver IntegrationTestBase) para isolar os casos sem recriar o schema.
/// </summary>
public class SmartRecoveryApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string MaintenanceConnectionString = "Host=localhost;Port=5432;Database=postgres;Username=admin;Password=admin123";
    public const string TestConnectionString = "Host=localhost;Port=5432;Database=smart_recovery_test;Username=admin;Password=admin123";

    private Respawner _respawner = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // O gateway real decide aleatoriamente (75%/25%) — nos testes precisamos de um resultado
            // determinístico para exercitar os fluxos de aprovação/recusa sem depender de sorte.
            services.RemoveAll<IPaymentGatewaySimulator>();
            services.AddSingleton<IPaymentGatewaySimulator, FakePaymentGatewaySimulator>();

            // O PaymentRetryWorker roda num timer de fundo e tocaria o mesmo banco de teste
            // concorrentemente com os testes (faturando assinaturas, retentando pagamentos) —
            // fonte de flakiness que nada tem a ver com o que cada teste está verificando.
            services.RemoveAll<IHostedService>();
        });
    }

    /// <summary>Acesso direto ao fake de gateway para o teste configurar o próximo resultado de cobrança.</summary>
    public FakePaymentGatewaySimulator Gateway =>
        (FakePaymentGatewaySimulator)Services.GetRequiredService<IPaymentGatewaySimulator>();

    public async Task InitializeAsync()
    {
        await EnsureTestDatabaseExistsAsync();

        // Acessar Services força a criação do host agora — o próprio Program.cs já migra o schema
        // no startup (ramo "Testing"), então não precisamos repetir isso aqui.
        _ = Services;

        await using var connection = new NpgsqlConnection(TestConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    /// <summary>Limpa todas as tabelas de dados — chamado antes de cada teste (ver IntegrationTestBase).</summary>
    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(TestConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    private static async Task EnsureTestDatabaseExistsAsync()
    {
        await using var connection = new NpgsqlConnection(MaintenanceConnectionString);
        await connection.OpenAsync();

        await using var checkCommand = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = 'smart_recovery_test'", connection);
        var exists = await checkCommand.ExecuteScalarAsync() is not null;
        if (exists)
            return;

        await using var createCommand = new NpgsqlCommand("CREATE DATABASE smart_recovery_test", connection);
        await createCommand.ExecuteNonQueryAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
