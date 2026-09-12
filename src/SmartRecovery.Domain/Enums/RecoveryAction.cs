namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Mapeamento Score → Ação, implementado em RecoveryDecisionEngine:
/// Score >= 80 + (TemporaryError ou IssuerUnavailable) → RetryIn2Hours
/// Score >= 80                                         → RetryIn24Hours
/// Score 50-79                                         → RetryIn72Hours
/// Score 30-49                                         → RequestPaymentMethodUpdate
/// Score &lt; 30                                         → CancelSubscription
/// AttemptCount >= 3 e Score &lt; 80                     → SendPaymentReminderEmail (antes das regras de score acima)
/// Qualquer + (BlockedCard ou SuspectedFraud)          → ManualReview (sempre, tem prioridade sobre as demais)
/// </summary>
public enum RecoveryAction
{
    RetryIn2Hours,
    RetryIn24Hours,
    RetryIn72Hours,
    RequestPaymentMethodUpdate,
    SendPaymentReminderEmail,
    CancelSubscription,
    ManualReview
}
