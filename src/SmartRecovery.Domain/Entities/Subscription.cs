using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Representa o vínculo entre um cliente e um plano — a assinatura ativa.
/// É a assinatura que dispara a criação de cobranças (Payments) nas datas programadas.
/// </summary>
public class Subscription : BaseEntity
{
    // ── Chaves estrangeiras ───────────────────────────────────────────────────
    public Guid CustomerId { get; set; }
    public Guid PlanId { get; set; }

    public DateTime StartDate { get; set; }

    /// <summary>
    /// Data da próxima cobrança programada.
    /// O BackgroundWorker usa essa data para saber quando criar o próximo Payment.
    /// </summary>
    public DateTime NextBillingDate { get; set; }

    /// <summary>Data em que a assinatura foi cancelada ou pausada. Null se ainda ativa.</summary>
    public DateTime? EndDate { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

    // ── Navegação ─────────────────────────────────────────────────────────────
    public Customer Customer { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = [];
}
