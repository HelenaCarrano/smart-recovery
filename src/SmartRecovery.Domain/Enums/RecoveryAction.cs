namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Mapeamento Score → Ação, implementado em RecoveryDecisionEngine:
/// Score >= 80 + TemporaryError → RetryIn2Hours
/// Score >= 80                  → RetryIn24Hours
/// Score 50-79                  → RetryIn72Hours
/// Score 30-49                  → RequestPaymentMethodUpdate
/// Score &lt; 30                  → CancelSubscription
/// Qualquer + BlockedCard       → ManualReview (sempre, independente do score)
/// </summary>
public enum RecoveryAction
{
    RetryIn2Hours,
    RetryIn24Hours,
    RetryIn72Hours,
    RequestPaymentMethodUpdate,
    CancelSubscription,
    ManualReview
}
