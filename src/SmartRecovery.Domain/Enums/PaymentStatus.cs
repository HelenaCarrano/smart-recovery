namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Transições: Pending → Approved | Declined; Declined → Pending (retry); Approved → Refunded | Cancelled.
/// </summary>
public enum PaymentStatus
{
    Pending,
    Approved,
    Declined,
    Cancelled,
    Refunded
}
