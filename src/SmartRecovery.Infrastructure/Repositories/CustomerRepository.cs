using Microsoft.EntityFrameworkCore;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces.Repositories;
using SmartRecovery.Infrastructure.Data;

namespace SmartRecovery.Infrastructure.Repositories;

public class CustomerRepository(SmartRecoveryDbContext context)
    : RepositoryBase<Customer>(context), ICustomerRepository
{
    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    public Task<Customer?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet
            .Include(c => c.Subscriptions).ThenInclude(s => s.Plan)
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
