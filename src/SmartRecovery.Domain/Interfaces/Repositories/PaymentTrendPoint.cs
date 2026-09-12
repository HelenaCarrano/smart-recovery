namespace SmartRecovery.Domain.Interfaces.Repositories;

public sealed record PaymentTrendPoint(DateOnly Date, int Approved, int Declined);
