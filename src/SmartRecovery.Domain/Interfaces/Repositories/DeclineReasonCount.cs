using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Interfaces.Repositories;

public sealed record DeclineReasonCount(DeclineReason DeclineReason, int Count);
