using Xunit;

namespace BettingEngine.Betting.Tests
{
    public class StakeTest
    {
        private const int DecimalPrecision = 10;
        private const decimal StakeValue = 42M;

        [Fact]
        public void Value_ReturnsCorrectly()
        {
            var stake = new Stake(StakeValue);

            Assert.Equal(StakeValue, stake.Value, DecimalPrecision);
        }
    }
}