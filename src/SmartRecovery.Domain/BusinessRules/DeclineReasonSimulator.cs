using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Distribuição realista de motivos de recusa entre adquirentes/gateways de pagamento:
/// InsufficientFunds e ExpiredCard dominam no dia a dia, enquanto SuspectedFraud é raro por
/// natureza (antifraude bloqueia poucos casos). Compartilhado por PaymentGatewaySimulator e
/// SmartRecoveryDataSeeder para os dois gerarem dados com a mesma cara, em vez de um motivo
/// uniformemente aleatório entre os 10 valores do enum.
/// </summary>
public static class DeclineReasonSimulator
{
    private static readonly (DeclineReason Reason, double Weight)[] Weights =
    [
        (DeclineReason.InsufficientFunds, 30),
        (DeclineReason.ExpiredCard, 20),
        (DeclineReason.TemporaryError, 15),
        (DeclineReason.IssuerUnavailable, 10),
        (DeclineReason.InvalidCard, 8),
        (DeclineReason.CardLimitExceeded, 7),
        (DeclineReason.SecurityCodeInvalid, 5),
        (DeclineReason.BlockedCard, 3),
        (DeclineReason.Unknown, 1.5),
        (DeclineReason.SuspectedFraud, 0.5),
    ];

    private static readonly double TotalWeight = Weights.Sum(w => w.Weight);

    public static DeclineReason Pick(Random random)
    {
        var roll = random.NextDouble() * TotalWeight;
        var cumulative = 0.0;

        foreach (var (reason, weight) in Weights)
        {
            cumulative += weight;
            if (roll < cumulative)
                return reason;
        }

        return Weights[^1].Reason;
    }
}
