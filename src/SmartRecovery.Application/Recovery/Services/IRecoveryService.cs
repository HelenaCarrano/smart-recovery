using SmartRecovery.Application.Recovery.DTOs;

namespace SmartRecovery.Application.Recovery.Services;

public interface IRecoveryService
{
    /// <summary>
    /// Executa o motor de recuperação para um pagamento recém-recusado: calcula o Recovery Score,
    /// decide a ação recomendada e aplica seus efeitos imediatos (agendar retry ou cancelar assinatura).
    /// Deve ser chamado logo após o Payment ter seu status alterado para Declined.
    /// </summary>
    Task<RecoveryAnalysisDto> AnalyzeAsync(Guid paymentId, CancellationToken cancellationToken = default);

    Task<RecoveryAnalysisDto?> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecoveryAnalysisDto>> GetPendingExecutionAsync(CancellationToken cancellationToken = default);
}
