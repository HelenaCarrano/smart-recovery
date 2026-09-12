namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Motivos pelos quais uma cobrança pode ser recusada.
/// Cada motivo tem um perfil diferente de recuperabilidade,
/// o que influencia diretamente no cálculo do Recovery Score.
///
/// Perfis de recuperabilidade:
/// - TemporaryError       → Alta chance de sucesso em retry rápido
/// - InsufficientFunds    → Boa chance se tentar em 24-72h (esperar próxima virada)
/// - ExpiredCard          → Baixa chance de retry; melhor solicitar atualização de dados
/// - InvalidCard          → Baixa chance de retry; melhor solicitar atualização de dados
/// - BlockedCard          → Muito baixa chance; pode requerer revisão manual
/// - Unknown              → Incerto; analisar histórico para decidir
/// </summary>
public enum DeclineReason
{
    /// <summary>Saldo ou limite insuficiente no momento da cobrança.</summary>
    InsufficientFunds,

    /// <summary>Cartão com data de validade expirada.</summary>
    ExpiredCard,

    /// <summary>Dados do cartão inválidos (número, CVV, etc.).</summary>
    InvalidCard,

    /// <summary>Falha temporária no sistema do emissor ou adquirente. Alta chance de sucesso em retry.</summary>
    TemporaryError,

    /// <summary>Cartão bloqueado pelo banco emissor. Requer ação do cliente.</summary>
    BlockedCard,

    /// <summary>Motivo não identificado ou não informado pelo emissor.</summary>
    Unknown
}
