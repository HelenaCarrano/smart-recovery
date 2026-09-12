using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Decide qual <see cref="RecoveryAction"/> tomar para um pagamento recusado,
/// combinando o Recovery Score (ver <see cref="RecoveryScoreCalculator"/>) com o motivo da recusa.
///
/// Regras (ver também a documentação em <see cref="RecoveryAction"/>):
///   BlockedCard              → ManualReview                  (sempre, independente do score)
///   Score >= 80 + TemporaryError → RetryIn2Hours
///   Score >= 80               → RetryIn24Hours
///   Score 50–79                → RetryIn72Hours
///   Score 30–49                → RequestPaymentMethodUpdate
///   Score < 30                 → CancelSubscription
/// </summary>
public static class RecoveryDecisionEngine
{
    public static RecoveryAction Decide(int recoveryScore, DeclineReason declineReason)
    {
        // Cartão bloqueado é sempre um caso ambíguo demais para automatizar:
        // pode ser fraude, bloqueio judicial, etc. Requer revisão humana independente do score.
        if (declineReason == DeclineReason.BlockedCard)
            return RecoveryAction.ManualReview;

        if (recoveryScore >= 80)
        {
            return declineReason == DeclineReason.TemporaryError
                ? RecoveryAction.RetryIn2Hours
                : RecoveryAction.RetryIn24Hours;
        }

        if (recoveryScore >= 50)
            return RecoveryAction.RetryIn72Hours;

        if (recoveryScore >= 30)
            return RecoveryAction.RequestPaymentMethodUpdate;

        return RecoveryAction.CancelSubscription;
    }
}
