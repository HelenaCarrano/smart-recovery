using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Registra cada tentativa individual de processar um pagamento.
///
/// Por que ter PaymentAttempt separado de Payment?
/// O Payment representa "a cobrança" (o que deve ser pago).
/// O PaymentAttempt representa "cada tentativa" de processar essa cobrança.
/// Isso permite rastrear o histórico completo de retries sem perder dados anteriores.
///
/// Exemplo: Payment com 3 PaymentAttempts
///   Attempt 1: Declined (InsufficientFunds) — 01/09
///   Attempt 2: Declined (InsufficientFunds) — 02/09
///   Attempt 3: Approved                     — 03/09
/// </summary>
public class PaymentAttempt : BaseEntity
{
    public Guid PaymentId { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus ResultStatus { get; set; }
    public DeclineReason? DeclineReason { get; set; }

    /// <summary>Observações livres sobre o resultado desta tentativa.</summary>
    public string? Notes { get; set; }

    // ── Navegação ─────────────────────────────────────────────────────────────
    public Payment Payment { get; set; } = null!;
}
