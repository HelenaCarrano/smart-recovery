using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Criada automaticamente quando um Payment é recusado. TotalPayments..RecentAttempts são as
/// features usadas pelo RecoveryScoreCalculator; BaseScore..RecentAttemptsAdjustment são a
/// contribuição de cada fator (ver RecoveryScoreBreakdown) — já somam para RecoveryScore.
/// </summary>
public class RecoveryAnalysis : BaseEntity
{
    public Guid PaymentId { get; set; }
    public int RecoveryScore { get; set; }
    public RecoveryAction RecommendedAction { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExecutedAt { get; set; }

    public int TotalPayments { get; set; }
    public int SuccessfulPayments { get; set; }
    public int PreviouslyRecoveredPayments { get; set; }
    public int RecentDeclines { get; set; }
    public int RecentAttempts { get; set; }

    public int BaseScore { get; set; }
    public int HistoryAdjustment { get; set; }
    public int RecoveryTrackRecordBonus { get; set; }
    public int RecentDeclinesAdjustment { get; set; }
    public int RecentAttemptsAdjustment { get; set; }

    public Payment Payment { get; set; } = null!;
}
