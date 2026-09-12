namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Detalhamento do Recovery Score por fator, para permitir que a API explique como o
/// score final foi composto sem que nenhum consumidor precise reimplementar a fórmula.
/// Cada campo já é a contribuição com o sinal correto: <c>Total</c> é simplesmente a
/// soma de todos eles, clampada em [0, 100].
/// </summary>
/// <param name="BaseScore">Score inicial atribuído ao motivo da recusa.</param>
/// <param name="HistoryAdjustment">Ajuste pela taxa histórica de sucesso do cliente (-20 a +20).</param>
/// <param name="RecoveryTrackRecordBonus">Bônus por recuperações anteriores bem-sucedidas (0 a +15).</param>
/// <param name="RecentDeclinesAdjustment">Penalidade por recusas recentes, já negativa (0 a -30).</param>
/// <param name="RecentAttemptsAdjustment">Penalidade por tentativas recentes, já negativa (0 a -20).</param>
/// <param name="Total">Soma de todos os fatores acima, clampada em [0, 100] — é o RecoveryScore final.</param>
public sealed record RecoveryScoreBreakdown(
    int BaseScore,
    int HistoryAdjustment,
    int RecoveryTrackRecordBonus,
    int RecentDeclinesAdjustment,
    int RecentAttemptsAdjustment,
    int Total);
