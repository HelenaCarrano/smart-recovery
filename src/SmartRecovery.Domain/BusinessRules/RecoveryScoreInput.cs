using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Dados de contexto necessários para calcular o Recovery Score de um pagamento recusado.
/// Espelha os campos de auditoria armazenados em <see cref="Entities.RecoveryAnalysis"/>.
/// </summary>
/// <param name="DeclineReason">Motivo da recusa que originou a análise.</param>
/// <param name="TotalPayments">Total de cobranças anteriores do cliente, sem contar a que está sendo analisada agora.</param>
/// <param name="SuccessfulPayments">Quantas dessas cobranças anteriores foram aprovadas (em qualquer tentativa).</param>
/// <param name="PreviouslyRecoveredPayments">Quantas cobranças recusadas anteriormente foram recuperadas com sucesso via retry.</param>
/// <param name="RecentDeclines">Recusas nos últimos 30 dias, incluindo a atual.</param>
/// <param name="RecentAttempts">Tentativas (de qualquer resultado) nos últimos 30 dias.</param>
public sealed record RecoveryScoreInput(
    DeclineReason DeclineReason,
    int TotalPayments,
    int SuccessfulPayments,
    int PreviouslyRecoveredPayments,
    int RecentDeclines,
    int RecentAttempts);
