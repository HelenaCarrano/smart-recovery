namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Status possíveis de uma assinatura ao longo do seu ciclo de vida.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>Assinatura ativa e com cobranças programadas normalmente.</summary>
    Active,

    /// <summary>Assinatura cancelada. Não gera novas cobranças.</summary>
    Cancelled,

    /// <summary>Assinatura pausada temporariamente. Não gera cobranças até ser reativada.</summary>
    Paused
}
