namespace SmartRecovery.Application.Dashboard.DTOs;

public record DashboardSummaryDto(
    int TotalCustomers,
    int ActiveSubscriptions,
    int TotalPayments,
    int ApprovedPayments,
    int DeclinedPayments,
    int PendingPayments,
    int RecoveredPayments,
    double RecoveryRate,
    decimal TotalRevenue,
    decimal RecoveredRevenue,
    int PendingRecoveryActions);
