using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Domain;

public class RecoveryScoreCalculatorTests
{
    [Fact]
    public void Calculate_NewCustomerWithTemporaryError_ReturnsBaseScore()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.TemporaryError,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var score = RecoveryScoreCalculator.Calculate(input);

        Assert.Equal(90, score);
    }

    [Fact]
    public void Calculate_GoodHistory_IncreasesScoreAboveBase()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.InsufficientFunds,
            TotalPayments: 10,
            SuccessfulPayments: 10,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var score = RecoveryScoreCalculator.Calculate(input);

        // Base 70 + histórico perfeito (+20)
        Assert.Equal(90, score);
    }

    [Fact]
    public void Calculate_BadHistory_DecreasesScoreBelowBase()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.InsufficientFunds,
            TotalPayments: 10,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var score = RecoveryScoreCalculator.Calculate(input);

        // Base 70 + histórico péssimo (-20)
        Assert.Equal(50, score);
    }

    [Fact]
    public void Calculate_ManyRecentDeclines_AppliesPenaltyCappedAt30()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.TemporaryError,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 0,
            RecentDeclines: 10,
            RecentAttempts: 0);

        var score = RecoveryScoreCalculator.Calculate(input);

        // Base 90 - teto de penalidade de recusas recentes (-30)
        Assert.Equal(60, score);
    }

    [Fact]
    public void Calculate_PreviouslyRecoveredPayments_GrantsBonusCappedAt15()
    {
        var input = new RecoveryScoreInput(
            DeclineReason.BlockedCard,
            TotalPayments: 0,
            SuccessfulPayments: 0,
            PreviouslyRecoveredPayments: 10,
            RecentDeclines: 1,
            RecentAttempts: 1);

        var score = RecoveryScoreCalculator.Calculate(input);

        // Base 15 + teto de bônus de recuperação (+15)
        Assert.Equal(30, score);
    }

    [Fact]
    public void Calculate_NeverGoesBelowZeroOrAboveOneHundred()
    {
        var worst = new RecoveryScoreInput(DeclineReason.BlockedCard, 10, 0, 0, 50, 50);
        var best = new RecoveryScoreInput(DeclineReason.TemporaryError, 10, 10, 10, 1, 1);

        Assert.InRange(RecoveryScoreCalculator.Calculate(worst), 0, 100);
        Assert.InRange(RecoveryScoreCalculator.Calculate(best), 0, 100);
    }
}
