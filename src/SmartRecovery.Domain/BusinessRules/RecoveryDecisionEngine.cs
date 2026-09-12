using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>Decide a <see cref="RecoveryAction"/> a partir do Recovery Score e do motivo da recusa — tabela completa em <see cref="RecoveryAction"/>.</summary>
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
