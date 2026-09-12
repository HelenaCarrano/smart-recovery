using System.ComponentModel.DataAnnotations;

namespace SmartRecovery.Application.Customers.DTOs;

public record CreateCustomerDto(
    [property: Required, StringLength(200)] string Name,
    [property: Required, EmailAddress, StringLength(256)] string Email,
    [property: Required, StringLength(20)] string Phone,
    [property: Required, StringLength(32)] string Document);
