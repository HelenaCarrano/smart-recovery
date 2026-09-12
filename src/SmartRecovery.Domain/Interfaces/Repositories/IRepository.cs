using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

/// <summary>
/// Contrato genérico de acesso a dados, comum a todas as entidades.
/// Repositórios específicos herdam daqui e adicionam apenas as consultas
/// que fogem do CRUD básico (ex: buscar por e-mail, por status, etc.).
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
