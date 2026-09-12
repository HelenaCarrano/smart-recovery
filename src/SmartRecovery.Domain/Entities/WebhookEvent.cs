using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Registra cada evento de webhook recebido pelo endpoint POST /api/webhooks/payments.
///
/// A idempotência é garantida pelo campo IdempotencyKey:
/// antes de processar qualquer evento, o sistema verifica se já existe um
/// WebhookEvent com a mesma chave. Se existir, ignora silenciosamente.
///
/// Isso protege contra o cenário comum em sistemas de pagamento onde
/// o mesmo webhook é disparado mais de uma vez (retry do provedor, falha de rede, etc.).
/// </summary>
public class WebhookEvent : BaseEntity
{
    /// <summary>
    /// Chave única enviada pelo provedor para identificar o evento.
    /// Usada para garantir idempotência — o mesmo evento não é processado duas vezes.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>Tipo do evento recebido. Ex: "payment.approved", "payment.declined".</summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>Corpo JSON do webhook, armazenado para auditoria e reprocessamento.</summary>
    public string Payload { get; set; } = string.Empty;

    public WebhookEventStatus Status { get; set; } = WebhookEventStatus.Received;

    public DateTime? ProcessedAt { get; set; }

    /// <summary>Mensagem de erro caso o processamento tenha falhado.</summary>
    public string? ErrorMessage { get; set; }
}
