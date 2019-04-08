using Moq;
using Xunit;

namespace BettingEngine.Betting.Tests
{
    public class WagerTest
    {
        public WagerTest()
        {
            _bet = new Mock<IBet<IResultSet>>();
            _expectedResults = new Mock<IResultSet>();
            _stake = new Mock<IStake>();
            _outcome = new Mock<IOutcome>();
        }

        private readonly Mock<IBet<IResultSet>> _bet;
        private readonly Mock<IResultSet> _expectedResults;
        private readonly Mock<IStake> _stake;
        private readonly Mock<IOutcome> _outcome;

        [Fact]
        public void ExpectedResults_ReturnsCorrectly()
        {
            var wager = new Wager<IResultSet>(_bet.Object, _expectedResults.Object, _stake.Object);

            _expectedResults.Setup(_ => _.Equals(_expectedResults.Object)).Returns(true);

            Assert.Equal(_expectedResults.Object, wager.ExpectedResults);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly()
        {
            var wager = new Wager<IResultSet>(_bet.Object, _expectedResults.Object, _stake.Object);

            _bet.Setup(_ => _.GetOutcome(wager, _expectedResults.Object)).Returns(_outcome.Object);

            Assert.Equal(_outcome.Object, wager.GetOutcome(_expectedResults.Object));
        }

        [Fact]
        public void Stake_ReturnsCorrectly()
        {
            var wager = new Wager<IResultSet>(_bet.Object, _expectedResults.Object, _stake.Object);

            Assert.Equal(_stake.Object, wager.Stake);
        }
    }
}
