using SmartRecovery.Domain.Entities;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IRecoveryAnalysisRepository : IRepository<RecoveryAnalysis>
{
    Task<RecoveryAnalysis?> GetByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>Análises cuja ação recomendada ainda não foi executada (ExecutedAt == null).</summary>
    Task<IReadOnlyList<RecoveryAnalysis>> GetPendingExecutionAsync(CancellationToken cancellationToken = default);

    Task<int> CountPendingExecutionAsync(CancellationToken cancellationToken = default);
}
