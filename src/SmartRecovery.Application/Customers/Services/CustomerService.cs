using SmartRecovery.Application.Customers.DTOs;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Customers.Services;

public class CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer '{id}' not found.");

        return ToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        if (await customerRepository.GetByEmailAsync(dto.Email, cancellationToken) is not null)
            throw new InvalidOperationException($"A customer with email '{dto.Email}' already exists.");

        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Document = dto.Document
        };

        await customerRepository.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(customer);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Customer '{id}' not found.");

        customer.IsActive = false;
        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CustomerDto ToDto(Customer c) => new(c.Id, c.Name, c.Email, c.Document, c.IsActive, c.CreatedAt);
}
