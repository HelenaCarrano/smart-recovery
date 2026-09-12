using Moq;
using SmartRecovery.Application.Customers.DTOs;
using SmartRecovery.Application.Customers.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository = new();
    private readonly Mock<IPaymentRepository> _paymentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _sut = new CustomerService(
            _customerRepository.Object,
            _subscriptionRepository.Object,
            _paymentRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsKeyNotFound_WhenCustomerDoesNotExist()
    {
        _customerRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenCustomerExists()
    {
        var customer = new Customer { Name = "Ana Dias", Email = "ana@example.com", Phone = "+55 11 91234-5678", Document = "123.456.789-00" };
        _customerRepository
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _sut.GetByIdAsync(customer.Id);

        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(customer.Phone, result.Phone);
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperation_WhenEmailAlreadyExists()
    {
        var dto = new CreateCustomerDto("Bruno Costa", "bruno@example.com", "+55 21 99876-5432", "987.654.321-00");
        _customerRepository
            .Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Email = dto.Email });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));

        _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_PersistsCustomer_WhenEmailIsUnique()
    {
        var dto = new CreateCustomerDto("Carla Pereira", "carla@example.com", "+55 31 98765-4321", "111.222.333-44");
        _customerRepository
            .Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.Phone, result.Phone);
        Assert.Equal(dto.Document, result.Document);
        _customerRepository.Verify(r => r.AddAsync(It.Is<Customer>(c => c.Email == dto.Email), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_ThrowsKeyNotFound_WhenCustomerDoesNotExist()
    {
        _customerRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeactivateAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeactivateAsync_SetsIsActiveFalse_AndSaves()
    {
        var customer = new Customer { Name = "Diego Lima", Email = "diego@example.com", IsActive = true };
        _customerRepository
            .Setup(r => r.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        await _sut.DeactivateAsync(customer.Id);

        Assert.False(customer.IsActive);
        _customerRepository.Verify(r => r.Update(customer), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
