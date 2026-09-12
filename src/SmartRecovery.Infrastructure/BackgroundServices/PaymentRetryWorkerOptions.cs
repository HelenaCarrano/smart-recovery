namespace SmartRecovery.Infrastructure.BackgroundServices;

public class PaymentRetryWorkerOptions
{
    public const string SectionName = "SmartRecovery:PaymentRetryWorker";

    public int IntervalSeconds { get; set; } = 60;
}
