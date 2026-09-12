using SmartRecovery.Application.Payments.Services;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Integration;

/// <summary>
/// Substitui o PaymentGatewaySimulator (que decide aleatoriamente) nos testes de integração,
/// para que "processar pagamento" seja determinístico: cada teste define o resultado que quer
/// exercitar via NextResult antes de chamar POST /api/payments/{id}/process.
/// </summary>
public class FakePaymentGatewaySimulator : IPaymentGatewaySimulator
{
    public (PaymentStatus Status, DeclineReason? DeclineReason) NextResult { get; set; } = (PaymentStatus.Approved, null);

    public (PaymentStatus Status, DeclineReason? DeclineReason) Charge() => NextResult;
}
