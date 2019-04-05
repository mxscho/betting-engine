using System;
using Xunit;

namespace BettingEngine.Betting.Tests
{
    public class OutcomeTest
    {
        private const decimal WinOutcomeWinnings = 42M;

        [Fact]
        public void Type_ReturnsCorrectly()
        {
            var winOutcome = Outcome.CreateWin(WinOutcomeWinnings);
            var lossOutcome = Outcome.CreateLoss();
            var canceledOutcome = Outcome.CreateCanceled();

            Assert.Equal(OutcomeType.Win, winOutcome.Type);
            Assert.Equal(OutcomeType.Loss, lossOutcome.Type);
            Assert.Equal(OutcomeType.Canceled, canceledOutcome.Type);
        }

        [Fact]
        public void Winnings_ReturnsCorrectly()
        {
            var winOutcome = Outcome.CreateWin(WinOutcomeWinnings);

            Assert.Equal(WinOutcomeWinnings, winOutcome.Winnings);
        }

        [Fact]
        public void Winnings_ThrowsCorrectly()
        {
            var lossOutcome = Outcome.CreateLoss();
            var canceledOutcome = Outcome.CreateCanceled();

            Assert.Throws<InvalidOperationException>(() => lossOutcome.Winnings);
            Assert.Throws<InvalidOperationException>(() => canceledOutcome.Winnings);
        }
    }
}