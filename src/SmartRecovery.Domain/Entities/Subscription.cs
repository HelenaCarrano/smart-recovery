using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid PlanId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    public Customer Customer { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = [];
}
