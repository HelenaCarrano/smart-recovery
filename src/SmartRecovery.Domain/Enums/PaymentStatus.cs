namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Status possíveis de um pagamento ao longo do seu ciclo de vida.
/// A transição entre estados segue regras de negócio bem definidas:
/// PENDING → APPROVED | DECLINED
/// DECLINED → PENDING (quando há uma nova tentativa de retry)
/// APPROVED → REFUNDED | CANCELLED
/// </summary>
public enum PaymentStatus
{
    /// <summary>Aguardando processamento ou nova tentativa de cobrança.</summary>
    Pending,

    /// <summary>Cobrança aprovada com sucesso.</summary>
    Approved,

    /// <summary>Cobrança recusada pelo emissor. Pode ser elegível para recuperação.</summary>
    Declined,

    /// <summary>Cobrança cancelada antes do processamento.</summary>
    Cancelled,

    /// <summary>Pagamento aprovado e depois estornado.</summary>
    Refunded
}
