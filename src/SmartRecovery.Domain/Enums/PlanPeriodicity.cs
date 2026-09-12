namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Periodicidade de cobrança de um plano de assinatura.
/// Determina o intervalo entre cada cobrança automática.
/// </summary>
public enum PlanPeriodicity
{
    /// <summary>Cobrança mensal (a cada 30 dias).</summary>
    Monthly,

    /// <summary>Cobrança trimestral (a cada 90 dias).</summary>
    Quarterly,

    /// <summary>Cobrança anual (a cada 365 dias).</summary>
    Annual
}
