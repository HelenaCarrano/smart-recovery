using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.Services;

/// <summary>
/// Implementação padrão do simulador: 75% de chance de aprovação, 25% de recusa com um motivo
/// escolhido segundo a distribuição realista de DeclineReasonSimulator. Serve para exercitar o
/// motor de recuperação com dados realistas sem depender de um provedor externo.
/// </summary>
public class PaymentGatewaySimulator : IPaymentGatewaySimulator
{
    private const double ApprovalRate = 0.75;

    public (PaymentStatus Status, DeclineReason? DeclineReason) Charge()
    {
        if (Random.Shared.NextDouble() < ApprovalRate)
            return (PaymentStatus.Approved, null);

        return (PaymentStatus.Declined, DeclineReasonSimulator.Pick(Random.Shared));
    }
}
