using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.Services;

/// <summary>
/// Implementação padrão do simulador: 75% de chance de aprovação, 25% de recusa com
/// um motivo escolhido aleatoriamente. Serve para exercitar o motor de recuperação
/// com dados realistas sem depender de um provedor externo.
/// </summary>
public class PaymentGatewaySimulator : IPaymentGatewaySimulator
{
    private static readonly DeclineReason[] DeclineReasons = Enum.GetValues<DeclineReason>();
    private const double ApprovalRate = 0.75;

    public (PaymentStatus Status, DeclineReason? DeclineReason) Charge()
    {
        if (Random.Shared.NextDouble() < ApprovalRate)
            return (PaymentStatus.Approved, null);

        var reason = DeclineReasons[Random.Shared.Next(DeclineReasons.Length)];
        return (PaymentStatus.Declined, reason);
    }
}
