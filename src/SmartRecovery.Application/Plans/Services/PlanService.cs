using SmartRecovery.Application.Plans.DTOs;
using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Interfaces;
using SmartRecovery.Domain.Interfaces.Repositories;

namespace SmartRecovery.Application.Plans.Services;

public class PlanService(IPlanRepository planRepository, IUnitOfWork unitOfWork) : IPlanService
{
    public async Task<IReadOnlyList<PlanDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var plans = await planRepository.GetActiveAsync(cancellationToken);
        return plans.Select(ToDto).ToList();
    }

    public async Task<PlanDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await planRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Plan '{id}' not found.");

        return ToDto(plan);
    }

    public async Task<PlanDto> CreateAsync(CreatePlanDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Price <= 0)
            throw new ArgumentException("Plan price must be greater than zero.");

        var plan = new Plan
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Periodicity = dto.Periodicity
        };

        await planRepository.AddAsync(plan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    private static PlanDto ToDto(Plan p) => new(p.Id, p.Name, p.Description, p.Price, p.Periodicity, p.IsActive);
}
