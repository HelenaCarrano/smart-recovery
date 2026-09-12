using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Payments.Services;

/// <summary>
/// Simula o retorno de um gateway de pagamento real, já que este é um projeto de portfólio
/// e não há integração com um processador de pagamentos de verdade.
/// </summary>
public interface IPaymentGatewaySimulator
{
    (PaymentStatus Status, DeclineReason? DeclineReason) Charge();
}
