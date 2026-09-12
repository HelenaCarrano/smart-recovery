using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Busca um cliente já carregando suas assinaturas e pagamentos, usado nas telas de detalhe.</summary>
    Task<Customer?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
