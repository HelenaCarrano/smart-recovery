using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Recovery.DTOs;

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
    int RecentAttempts);
