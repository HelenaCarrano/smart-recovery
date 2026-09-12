using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Recovery.DTOs;

/// <summary>
/// BaseScore/HistoryAdjustment/RecoveryTrackRecordBonus/RecentDeclinesAdjustment/RecentAttemptsAdjustment
/// já vêm com o sinal correto: RecoveryScore é a soma de todos eles, clampada em [0, 100].
/// Isso permite que qualquer consumidor (frontend, relatórios) explique a composição do
/// score sem reimplementar a fórmula de SmartRecovery.Domain.BusinessRules.RecoveryScoreCalculator.
/// </summary>
public record RecoveryAnalysisDto(
    Guid Id,
    Guid PaymentId,
    int RecoveryScore,
    RecoveryAction RecommendedAction,
    DateTime AnalyzedAt,
    DateTime? ExecutedAt,
    int TotalPayments,
    int SuccessfulPayments,
    int PreviouslyRecoveredPayments,
    int RecentDeclines,
    int RecentAttempts,
    int BaseScore,
    int HistoryAdjustment,
    int RecoveryTrackRecordBonus,
    int RecentDeclinesAdjustment,
    int RecentAttemptsAdjustment);
