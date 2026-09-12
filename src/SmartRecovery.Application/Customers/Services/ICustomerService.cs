using SmartRecovery.Application.Customers.DTOs;

namespace SmartRecovery.Application.Customers.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
