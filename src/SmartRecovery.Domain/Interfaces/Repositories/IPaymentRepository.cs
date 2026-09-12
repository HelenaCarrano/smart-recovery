using SmartRecovery.Domain.Entities;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

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
}
