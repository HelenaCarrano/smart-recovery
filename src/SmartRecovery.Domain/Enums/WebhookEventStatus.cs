namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Status de um evento de webhook recebido pela API.
/// Usado para controle de idempotência e reprocessamento.
/// </summary>
public enum WebhookEventStatus
{
    /// <summary>Evento recebido mas ainda não processado.</summary>
    Received,

    /// <summary>Evento processado com sucesso.</summary>
    Processed,

    /// <summary>Falha ao processar o evento (pode ser reprocessado).</summary>
    Failed
}
