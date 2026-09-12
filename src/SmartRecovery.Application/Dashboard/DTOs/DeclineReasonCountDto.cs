using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Application.Dashboard.DTOs;

public record DeclineReasonCountDto(DeclineReason DeclineReason, int Count);
