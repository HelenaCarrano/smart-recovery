using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Traduz uma <see cref="RecoveryAction"/> de retry no intervalo de tempo até a próxima tentativa.
/// Ações que não são de retry (ex: RequestPaymentMethodUpdate) não têm um delay associado.
/// </summary>
public static class RecoveryActionScheduler
{
    public static bool TryGetRetryDelay(RecoveryAction action, out TimeSpan delay)
    {
        delay = action switch
        {
            RecoveryAction.RetryIn2Hours => TimeSpan.FromHours(2),
            RecoveryAction.RetryIn24Hours => TimeSpan.FromHours(24),
            RecoveryAction.RetryIn72Hours => TimeSpan.FromHours(72),
            _ => TimeSpan.Zero
        };

        return delay > TimeSpan.Zero;
    }
}
