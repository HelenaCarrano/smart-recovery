namespace SmartRecovery.Domain.Enums;

/// <summary>Ver BillingCycleCalculator: usa dias fixos, não meses de calendário.</summary>
public enum PlanPeriodicity
{
    /// <summary>30 dias.</summary>
    Monthly,

    /// <summary>90 dias.</summary>
    Quarterly,

    /// <summary>365 dias.</summary>
    Annual
}
