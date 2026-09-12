namespace SmartRecovery.Infrastructure.BackgroundServices;

/// <summary>Configuração lida de "SmartRecovery:PaymentRetryWorker" no appsettings.json.</summary>
public class PaymentRetryWorkerOptions
{
    public const string SectionName = "SmartRecovery:PaymentRetryWorker";

    /// <summary>Intervalo, em segundos, entre cada verificação de pagamentos/assinaturas pendentes.</summary>
    public int IntervalSeconds { get; set; } = 60;
}
