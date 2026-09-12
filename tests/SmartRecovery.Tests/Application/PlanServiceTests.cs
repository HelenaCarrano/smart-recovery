using Moq;
using SmartRecovery.Application.Plans.DTOs;
using SmartRecovery.Application.Plans.Services;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Tests.Application;

public class PlanServiceTests
{
    private readonly Mock<IPlanRepository> _planRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PlanService _sut;

    public PlanServiceTests()
    {
        _sut = new PlanService(_planRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsMappedDtos()
    {
        var plans = new List<Plan>
        {
            new() { Name = "Básico", Description = "Entrada", Price = 49.90m, Periodicity = PlanPeriodicity.Monthly, IsActive = true },
            new() { Name = "Enterprise", Description = "Completo", Price = 999.90m, Periodicity = PlanPeriodicity.Annual, IsActive = true },
        };
        _planRepository.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(plans);

        var result = await _sut.GetActiveAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Básico" && p.Price == 49.90m);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsKeyNotFound_WhenPlanDoesNotExist()
    {
        _planRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Plan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenPriceIsNotPositive()
    {
        var dto = new CreatePlanDto("Grátis", "Sem custo", 0m, PlanPeriodicity.Monthly);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));

        _planRepository.Verify(r => r.AddAsync(It.IsAny<Plan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_PersistsPlan_WhenPriceIsValid()
    {
        var dto = new CreatePlanDto("Profissional", "Plano intermediário", 129.90m, PlanPeriodicity.Quarterly);

        var result = await _sut.CreateAsync(dto);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
        Assert.True(result.IsActive);
        _planRepository.Verify(r => r.AddAsync(It.Is<Plan>(p => p.Name == dto.Name), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
