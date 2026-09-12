using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.Entities;

/// <summary>
/// Idempotência via IdempotencyKey: antes de processar, verifica se já existe um evento com a
/// mesma chave e ignora silenciosamente se sim — protege contra o provedor disparar o mesmo
/// webhook mais de uma vez (retry, falha de rede etc.).
/// </summary>
public class WebhookEvent : BaseEntity
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public WebhookEventStatus Status { get; set; } = WebhookEventStatus.Received;
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
