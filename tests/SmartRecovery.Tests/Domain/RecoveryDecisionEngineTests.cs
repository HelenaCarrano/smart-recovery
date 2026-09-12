using SmartRecovery.Domain.BusinessRules;
using SmartRecovery.Domain.Enums;

namespace SmartRecovery.Tests.Domain;

public class RecoveryDecisionEngineTests
{
    [Fact]
    public void Decide_BlockedCard_AlwaysReturnsManualReview_RegardlessOfScore()
    {
        Assert.Equal(RecoveryAction.ManualReview, RecoveryDecisionEngine.Decide(100, DeclineReason.BlockedCard));
        Assert.Equal(RecoveryAction.ManualReview, RecoveryDecisionEngine.Decide(0, DeclineReason.BlockedCard));
    }

    [Fact]
    public void Decide_HighScoreWithTemporaryError_ReturnsRetryIn2Hours()
    {
        var action = RecoveryDecisionEngine.Decide(80, DeclineReason.TemporaryError);
        Assert.Equal(RecoveryAction.RetryIn2Hours, action);
    }

    [Fact]
    public void Decide_HighScoreWithOtherReason_ReturnsRetryIn24Hours()
    {
        var action = RecoveryDecisionEngine.Decide(80, DeclineReason.InsufficientFunds);
        Assert.Equal(RecoveryAction.RetryIn24Hours, action);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(79)]
    public void Decide_MidScore_ReturnsRetryIn72Hours(int score)
    {
        var action = RecoveryDecisionEngine.Decide(score, DeclineReason.InsufficientFunds);
        Assert.Equal(RecoveryAction.RetryIn72Hours, action);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(49)]
    public void Decide_LowScore_ReturnsRequestPaymentMethodUpdate(int score)
    {
        var action = RecoveryDecisionEngine.Decide(score, DeclineReason.ExpiredCard);
        Assert.Equal(RecoveryAction.RequestPaymentMethodUpdate, action);
    }

    [Fact]
    public void Decide_VeryLowScore_ReturnsCancelSubscription()
    {
        var action = RecoveryDecisionEngine.Decide(29, DeclineReason.InvalidCard);
        Assert.Equal(RecoveryAction.CancelSubscription, action);
    }
}
