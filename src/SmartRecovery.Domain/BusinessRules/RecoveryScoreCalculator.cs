using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Domain.BusinessRules;

/// <summary>
/// Calcula o Recovery Score (0–100) de um pagamento recusado: a probabilidade
/// estimada de que a cobrança seja recuperada com sucesso.
///
/// O score combina três fatores:
/// 1. Perfil de recuperabilidade do motivo da recusa (base score).
/// 2. Histórico geral de sucesso do cliente (ajuste por taxa de aprovação).
/// 3. Sinais recentes de dificuldade — recusas e tentativas repetidas pesam contra o cliente.
///
/// Esta é uma implementação baseada em regras, pensada para ser facilmente substituída
/// por um modelo de Machine Learning no futuro: os campos de <see cref="RecoveryScoreInput"/>
/// já são as features que tal modelo usaria como entrada.
/// </summary>
public static class RecoveryScoreCalculator
{
    private const int MinScore = 0;
    private const int MaxScore = 100;

    /// <summary>Score inicial atribuído a cada motivo de recusa, antes de ajustes por histórico.</summary>
    private static int BaseScoreFor(DeclineReason reason) => reason switch
    {
        DeclineReason.TemporaryError => 90,
        DeclineReason.InsufficientFunds => 70,
        DeclineReason.Unknown => 50,
        DeclineReason.ExpiredCard => 40,
        DeclineReason.InvalidCard => 35,
        DeclineReason.BlockedCard => 15,
        _ => 50
    };

    public static int Calculate(RecoveryScoreInput input)
    {
        var score = BaseScoreFor(input.DeclineReason);

        score += HistoryAdjustment(input);
        score += RecoveryTrackRecordBonus(input);
        score -= RecentDeclinesPenalty(input);
        score -= RecentAttemptsPenalty(input);

        return Math.Clamp(score, MinScore, MaxScore);
    }

    /// <summary>
    /// Ajusta o score com base na taxa histórica de aprovação do cliente.
    /// Clientes sem histórico (TotalPayments == 0) são tratados de forma neutra,
    /// já que ainda não há dados suficientes para favorecê-los ou penalizá-los.
    /// Varia de -20 (cliente com histórico ruim) a +20 (cliente confiável).
    /// </summary>
    private static int HistoryAdjustment(RecoveryScoreInput input)
    {
        if (input.TotalPayments <= 0)
            return 0;

        var successRate = input.SuccessfulPayments / (double)input.TotalPayments;
        return (int)Math.Round((successRate - 0.5) * 40);
    }

    /// <summary>
    /// Clientes que já foram recuperados com sucesso antes tendem a ser recuperados de novo.
    /// +5 por recuperação anterior, até um teto de +15.
    /// </summary>
    private static int RecoveryTrackRecordBonus(RecoveryScoreInput input) =>
        Math.Min(input.PreviouslyRecoveredPayments * 5, 15);

    /// <summary>
    /// Cada recusa recente além da atual indica um problema persistente (ex: cartão sem
    /// solução, briga recorrente com o limite). -10 por recusa adicional, até -30.
    /// </summary>
    private static int RecentDeclinesPenalty(RecoveryScoreInput input)
    {
        var extraDeclines = Math.Max(input.RecentDeclines - 1, 0);
        return Math.Min(extraDeclines * 10, 30);
    }

    /// <summary>
    /// Muitas tentativas recentes sem que o problema se resolva sugerem que insistir
    /// não vai adiantar. -5 por tentativa além das duas primeiras, até -20.
    /// </summary>
    private static int RecentAttemptsPenalty(RecoveryScoreInput input)
    {
        var extraAttempts = Math.Max(input.RecentAttempts - 2, 0);
        return Math.Min(extraAttempts * 5, 20);
    }
}
