using Microsoft.AspNetCore.Mvc;
using SmartRecovery.API.Common;
using SmartRecovery.Application.Common;
using SmartRecovery.Application.Customers.DTOs;
using SmartRecovery.Application.Customers.Services;

namespace SmartRecovery.API.Controllers;

/// <summary>Gerenciamento de clientes.</summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    /// <summary>Lista clientes paginados; <paramref name="search"/> filtra por nome ou email.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerListItemDto>>> GetPaged(
        [FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await customerService.GetPagedAsync(Pagination.NormalizePage(page), Pagination.NormalizePageSize(pageSize, 20), search, cancellationToken));

    /// <summary>Busca um cliente pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await customerService.GetByIdAsync(id, cancellationToken));

    /// <summary>Cadastra um novo cliente.</summary>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var customer = await customerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>Desativa um cliente (soft delete lógico).</summary>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await customerService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }
}
