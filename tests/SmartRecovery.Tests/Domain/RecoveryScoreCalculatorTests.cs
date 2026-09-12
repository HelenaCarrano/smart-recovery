using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Domain;

public class RecoveryScoreCalculatorTests
{
    [Fact]
    public void CalculateBreakdown_NewCustomerWithTemporaryError_ReturnsBaseScore()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.TemporaryError,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var total = RecoveryScoreCalculator.CalculateBreakdown(input).Total;

        Assert.Equal(90, total);
    }

    [Fact]
    public void CalculateBreakdown_GoodHistory_IncreasesScoreAboveBase()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.InsufficientFunds,
            TotalPayments: 10,
            SuccessfulPayments: 10,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var total = RecoveryScoreCalculator.CalculateBreakdown(input).Total;

        // Base 70 + histórico perfeito (+20)
        Assert.Equal(90, total);
    }

    [Fact]
    public void CalculateBreakdown_BadHistory_DecreasesScoreBelowBase()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.InsufficientFunds,
            TotalPayments: 10,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var total = RecoveryScoreCalculator.CalculateBreakdown(input).Total;

        // Base 70 + histórico péssimo (-20)
        Assert.Equal(50, total);
    }

    [Fact]
    public void CalculateBreakdown_ManyRecentDeclines_AppliesPenaltyCappedAt30()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.TemporaryError,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 10,
            RecentAttempts: 0);

        var total = RecoveryScoreCalculator.CalculateBreakdown(input).Total;

        // Base 90 - teto de penalidade de recusas recentes (-30)
        Assert.Equal(60, total);
    }

    [Fact]
    public void CalculateBreakdown_PreviouslyRecoveredPayments_GrantsBonusCappedAt15()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.BlockedCard,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 10,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var total = RecoveryScoreCalculator.CalculateBreakdown(input).Total;

        // Base 15 + teto de bônus de recuperação (+15)
        Assert.Equal(30, total);
    }

    [Fact]
    public void CalculateBreakdown_TotalNeverGoesBelowZeroOrAboveOneHundred()
    {
        var worst = new RecoveryScoreInput(DeclineReason.BlockedCard, 10, 0, 0, 50, 50);
        var best = new RecoveryScoreInput(DeclineReason.TemporaryError, 10, 10, 10, 1, 1);

        Assert.InRange(RecoveryScoreCalculator.CalculateBreakdown(worst).Total, 0, 100);
        Assert.InRange(RecoveryScoreCalculator.CalculateBreakdown(best).Total, 0, 100);
    }

    [Fact]
    public void CalculateBreakdown_FactorsAlwaysSumToTotal()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.InsufficientFunds,
            TotalPayments: 10,
            SuccessfulPayments: 3,
            PreviouslyRecoveredPayments: 2,
            RecentDeclines: 3,
            RecentAttempts: 5);

        var breakdown = RecoveryScoreCalculator.CalculateBreakdown(input);

        Assert.Equal(
            breakdown.BaseScore + breakdown.HistoryAdjustment + breakdown.RecoveryTrackRecordBonus
                + breakdown.RecentDeclinesAdjustment + breakdown.RecentAttemptsAdjustment,
            breakdown.Total);
    }

    [Fact]
    public void CalculateBreakdown_ExposesPenaltiesAsNegativeContributions()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.TemporaryError,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 10,
            RecentAttempts: 10);

        var breakdown = RecoveryScoreCalculator.CalculateBreakdown(input);

        Assert.Equal(90, breakdown.BaseScore);
        Assert.Equal(-30, breakdown.RecentDeclinesAdjustment);
        Assert.Equal(-20, breakdown.RecentAttemptsAdjustment);
    }

    [Fact]
    public void CalculateBreakdown_ClampsTotal_EvenWhenSumOfFactorsIsNegative()
    {
        var input = new RecoveryScoreInput(DeclineReason.BlockedCard, 10, 0, 0, 10, 10);

        var breakdown = RecoveryScoreCalculator.CalculateBreakdown(input);

        Assert.Equal(0, breakdown.Total);
    }
}
