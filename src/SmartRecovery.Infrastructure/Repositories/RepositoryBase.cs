using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

/// <summary>Implementação genérica compartilhada por todos os repositórios concretos.</summary>
public abstract class RepositoryBase<T>(SmartRecoveryDbContext context) : IRepository<T>
    where T : BaseEntity
{
    protected readonly SmartRecoveryDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.ToListAsync(cancellationToken);

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void Remove(T entity) => DbSet.Remove(entity);
}
