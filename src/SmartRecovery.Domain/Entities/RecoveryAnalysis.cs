using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Armazena o resultado da análise de recuperação para um pagamento recusado.
/// É criada automaticamente quando um Payment muda para Status = Declined.
///
/// Contém:
/// - O Recovery Score calculado (0–100)
/// - A ação recomendada pelo motor de decisão
/// - Os dados de contexto usados no cálculo (para auditoria e futura entrada de ML)
/// </summary>
public class RecoveryAnalysis : BaseEntity
{
    public Guid PaymentId { get; set; }

    /// <summary>
    /// Pontuação de recuperabilidade calculada pelo RecoveryScoreCalculator.
    /// Varia de 0 (sem chance) a 100 (alta probabilidade de recuperação).
    /// </summary>
    public int RecoveryScore { get; set; }

    /// <summary>Ação determinada pelo RecoveryDecisionEngine com base no score.</summary>
    public RecoveryAction RecommendedAction { get; set; }

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Data em que a ação recomendada foi efetivamente executada. Null se ainda pendente.</summary>
    public DateTime? ExecutedAt { get; set; }

    // ── Dados de contexto usados no cálculo ───────────────────────────────────
    // Armazenados aqui para auditoria e para futura substituição por modelo de ML.
    // Um modelo preditivo precisará desses dados como features de entrada.

    public int TotalPayments { get; set; }
    public int SuccessfulPayments { get; set; }
    public int PreviouslyRecoveredPayments { get; set; }
    public int RecentDeclines { get; set; }
    public int RecentAttempts { get; set; }

    // ── Navegação ─────────────────────────────────────────────────────────────
    public Payment Payment { get; set; } = null!;
}
