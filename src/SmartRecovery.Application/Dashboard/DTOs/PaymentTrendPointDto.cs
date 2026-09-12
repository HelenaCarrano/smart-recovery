namespace SmartRecovery.Application.Dashboard.DTOs;

public record PaymentTrendPointDto(DateOnly Date, int Approved, int Declined);
