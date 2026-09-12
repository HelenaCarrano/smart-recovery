using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Tests.Integration;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    /// <summary>Espelha o JsonStringEnumConverter registrado em Program.cs — a API serializa enums como string.</summary>
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly SmartRecoveryApiFactory _factory;
    private readonly IServiceScope _scope;

    protected readonly HttpClient Client;
    protected SmartRecoveryApiFactory Factory => _factory;

    protected IntegrationTestBase(SmartRecoveryApiFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();
        Client = factory.CreateClient();
    }

    /// <summary>Reseta o banco antes de cada teste — cada [Fact] parte de um estado vazio e isolado.</summary>
    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>Escopo de DbContext para inserir os dados de Arrange direto no banco (não há endpoint para criar Customer/Plan/Subscription).</summary>
    protected SmartRecoveryDbContext CreateDbContext() =>
        _scope.ServiceProvider.GetRequiredService<SmartRecoveryDbContext>();
}
