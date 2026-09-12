using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pagamentos com retry agendado (ScheduledRetryAt) para até o instante informado.
    /// Usados pelo PaymentRetryWorker para saber o que processar em cada ciclo.
    /// </summary>
    Task<IReadOnlyList<Payment>> GetDueForRetryAsync(DateTime asOf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Histórico completo de pagamentos de um cliente, usado como entrada do RecoveryScoreCalculator.
    /// Inclui os Attempts para permitir contar tentativas recentes.
    /// </summary>
    Task<IReadOnlyList<Payment>> GetHistoryByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>Listagem paginada e filtrável — não materializa a tabela inteira em memória.</summary>
    Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(PaymentFilter filter, CancellationToken cancellationToken = default);

    Task<int> CountAllAsync(CancellationToken cancellationToken = default);

    Task<int> CountByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

    /// <summary>Pagamentos que já foram recusados em algum momento: status atual Declined OU mais de uma tentativa.</summary>
    Task<int> CountEverDeclinedAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeclineReasonCount>> GetDeclineReasonCountsAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetApprovedRevenueAsync(CancellationToken cancellationToken = default);

    /// <summary>Contagem e soma de pagamentos aprovados que precisaram de mais de uma tentativa — a "receita recuperada".</summary>
    Task<(int Count, decimal Revenue)> GetRecoveredStatsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentTrendPoint>> GetTrendAsync(DateTime since, CancellationToken cancellationToken = default);

    /// <summary>Contagem de pagamentos por cliente, para os ids informados — uma única query agrupada, sem N+1.</summary>
    Task<IReadOnlyDictionary<Guid, int>> CountByCustomerIdsAsync(IReadOnlyList<Guid> customerIds, CancellationToken cancellationToken = default);
}
