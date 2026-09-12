using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Ciclo de vida: Pending → Approved | Declined. Se Declined, uma RecoveryAnalysis é criada
/// automaticamente e o motor decide entre agendar retry, pedir atualização de pagamento ou cancelar.
/// </summary>
public class Payment : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DeclineReason? DeclineReason { get; set; }
    public int AttemptCount { get; set; } = 0;
    public DateTime? ScheduledRetryAt { get; set; }

    public Customer Customer { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!;
    public ICollection<PaymentAttempt> Attempts { get; set; } = [];
    public RecoveryAnalysis? RecoveryAnalysis { get; set; }
}
