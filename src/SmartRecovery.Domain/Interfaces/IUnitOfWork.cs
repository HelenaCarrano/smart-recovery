namespace SmartRecovery.Domain.Interfaces;

/// <summary>
/// Abstrai a persistência das mudanças rastreadas pelos repositórios em uma única transação.
/// Os repositórios apenas marcam entidades como adicionadas/alteradas/removidas;
/// é o UnitOfWork quem efetivamente grava no banco, garantindo atomicidade.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
