using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public sealed record PaymentFilter(
    int Page,
    int PageSize,
    PaymentStatus? Status = null,
    DeclineReason? DeclineReason = null,
    Guid? CustomerId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null);
