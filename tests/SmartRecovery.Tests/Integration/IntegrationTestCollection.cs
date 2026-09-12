namespace SmartRecovery.Tests.Integration;

/// <summary>
/// Compartilha uma única SmartRecoveryApiFactory (e portanto uma única migration/Respawner) entre
/// todas as classes de teste de integração, em vez de subir um host novo por classe.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<SmartRecoveryApiFactory>
{
    public const string Name = "Integration";
}
