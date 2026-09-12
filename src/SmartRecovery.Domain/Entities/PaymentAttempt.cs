using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Payment é "a cobrança"; cada PaymentAttempt é uma tentativa de processá-la —
/// separados para manter o histórico completo de retries sem perder tentativas anteriores.
/// </summary>
public class PaymentAttempt : BaseEntity
{
    public Guid PaymentId { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus ResultStatus { get; set; }
    public DeclineReason? DeclineReason { get; set; }
    public string? Notes { get; set; }

    public Payment Payment { get; set; } = null!;
}
