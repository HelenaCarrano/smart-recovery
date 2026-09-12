using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Representa um plano de assinatura disponível na plataforma.
/// Clientes se inscrevem em planos, que definem o valor e a periodicidade da cobrança.
/// </summary>
public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Valor da cobrança por ciclo, em reais.</summary>
    public decimal Price { get; set; }

    public PlanPeriodicity Periodicity { get; set; }
    public bool IsActive { get; set; } = true;

    // ── Navegação ─────────────────────────────────────────────────────────────
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}
