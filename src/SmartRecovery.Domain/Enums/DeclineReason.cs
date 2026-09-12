namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Cada motivo tem um perfil de recuperabilidade diferente (ver RecoveryScoreCalculator.BaseScoreFor):
/// TemporaryError e InsufficientFunds recuperam bem com retry; ExpiredCard/InvalidCard precisam de
/// atualização de dados; BlockedCard tende a exigir revisão manual.
/// </summary>
public enum DeclineReason
{
    InsufficientFunds,
    ExpiredCard,
    InvalidCard,
    TemporaryError,
    BlockedCard,
    SuspectedFraud,
    CardLimitExceeded,
    SecurityCodeInvalid,
    IssuerUnavailable,
    Unknown
}
