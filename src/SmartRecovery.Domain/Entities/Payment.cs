using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Representa uma tentativa de cobrança — o coração da plataforma.
///
/// Ciclo de vida de um pagamento no Smart Recovery:
/// 1. Payment é criado com Status = Pending
/// 2. Sistema tenta processar → Approved ou Declined
/// 3. Se Declined: RecoveryAnalysis é criado automaticamente
/// 4. Motor decide a ação → agenda retry ou solicita atualização
/// 5. Se retry: nova PaymentAttempt é registrada e status volta a Pending
/// 6. Processo se repete até aprovação, limite de tentativas ou cancelamento
/// </summary>
public class Payment : BaseEntity
{
    // ── Chaves estrangeiras ───────────────────────────────────────────────────
    public Guid CustomerId { get; set; }
    public Guid SubscriptionId { get; set; }

    /// <summary>Valor da cobrança em reais. Copiado do plano no momento da criação.</summary>
    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// Motivo da última recusa. Null enquanto o pagamento não foi recusado.
    /// Esse campo alimenta o Recovery Score Calculator.
    /// </summary>
    public DeclineReason? DeclineReason { get; set; }

    /// <summary>
    /// Número de vezes que este pagamento foi tentado (incluindo a tentativa inicial).
    /// Muitas tentativas consecutivas sem sucesso reduzem o Recovery Score.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Data e hora em que o próximo retry deve ser executado pelo BackgroundWorker.
    /// Null se não há retry agendado.
    /// </summary>
    public DateTime? ScheduledRetryAt { get; set; }

    // ── Navegação ─────────────────────────────────────────────────────────────
    public Customer Customer { get; set; } = null!;
    public Subscription Subscription { get; set; } = null!;
    public ICollection<PaymentAttempt> Attempts { get; set; } = [];
    public RecoveryAnalysis? RecoveryAnalysis { get; set; }
}
