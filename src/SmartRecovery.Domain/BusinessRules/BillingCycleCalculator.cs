using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Calcula datas de cobrança a partir da periodicidade de um plano.
/// Os intervalos seguem a documentação de <see cref="PlanPeriodicity"/>: mensal = 30 dias,
/// trimestral = 90 dias, anual = 365 dias.
/// </summary>
public static class BillingCycleCalculator
{
    public static DateTime NextBillingDate(DateTime from, PlanPeriodicity periodicity) => periodicity switch
    {
        PlanPeriodicity.Monthly => from.AddDays(30),
        PlanPeriodicity.Quarterly => from.AddDays(90),
        PlanPeriodicity.Annual => from.AddDays(365),
        _ => throw new ArgumentOutOfRangeException(nameof(periodicity), periodicity, "Unknown periodicity.")
    };

    /// <summary>Normaliza o preço de um plano para uma equivalência mensal — usado no cálculo de MRR.</summary>
    public static decimal MonthlyEquivalent(decimal price, PlanPeriodicity periodicity) => periodicity switch
    {
        PlanPeriodicity.Monthly => price,
        PlanPeriodicity.Quarterly => price / 3m,
        PlanPeriodicity.Annual => price / 12m,
        _ => throw new ArgumentOutOfRangeException(nameof(periodicity), periodicity, "Unknown periodicity.")
    };
}
