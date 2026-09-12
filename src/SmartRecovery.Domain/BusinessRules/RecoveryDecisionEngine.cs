using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>Decide a <see cref="RecoveryAction"/> a partir do Recovery Score e do motivo da recusa — tabela completa em <see cref="RecoveryAction"/>.</summary>
public static class RecoveryDecisionEngine
{
    public static RecoveryAction Decide(int recoveryScore, DeclineReason declineReason, int attemptCount = 1)
    {
        // Suspeita de fraude ou cartão bloqueado exigem revisão manual de risco
        if (declineReason is DeclineReason.BlockedCard or DeclineReason.SuspectedFraud)
            return RecoveryAction.ManualReview;

        // Se já foram feitas várias tentativas consecutivas (>= 3), notifica o cliente por e-mail com link de regularização
        if (attemptCount >= 3 && recoveryScore < 80)
            return RecoveryAction.SendPaymentReminderEmail;

        if (recoveryScore >= 80)
        {
            return declineReason is DeclineReason.TemporaryError or DeclineReason.IssuerUnavailable
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
