namespace SmartRecovery.Domain.Enums;

/// <summary>
/// Ações de recuperação que o motor de decisão pode recomendar
/// após calcular o Recovery Score de um pagamento recusado.
///
/// O mapeamento Score → Ação é responsabilidade do RecoveryDecisionEngine:
/// Score >= 80 + TemporaryError  → RetryIn2Hours
/// Score >= 80                   → RetryIn24Hours
/// Score 50-79                   → RetryIn72Hours
/// Score 30-49                   → RequestPaymentMethodUpdate
/// Score < 30                    → CancelSubscription
/// Qualquer + BlockedCard        → ManualReview
/// </summary>
public enum RecoveryAction
{
    /// <summary>Tentar nova cobrança em 2 horas. Usado para erros temporários com score alto.</summary>
    RetryIn2Hours,

    /// <summary>Tentar nova cobrança em 24 horas. Padrão para clientes com bom histórico.</summary>
    RetryIn24Hours,

    /// <summary>Tentar nova cobrança em 72 horas. Para clientes com histórico moderado.</summary>
    RetryIn72Hours,

    /// <summary>Solicitar ao cliente que atualize os dados de pagamento. Para cartões expirados ou inválidos.</summary>
    RequestPaymentMethodUpdate,

    /// <summary>Score muito baixo ou muitas tentativas sem sucesso — encerrar assinatura.</summary>
    CancelSubscription,

    /// <summary>Situação ambígua (ex: cartão bloqueado) — requer análise humana.</summary>
    ManualReview
}
