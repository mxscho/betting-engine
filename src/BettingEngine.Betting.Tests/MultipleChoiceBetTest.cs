using System;
using Moq;
using Xunit;

namespace BettingEngine.Betting.Tests
{
    public class MultipleChoiceBetTest
    {
        public MultipleChoiceBetTest()
        {
            _resultA = new Mock<IResult>();
            _resultB = new Mock<IResult>();
            _resultC = new Mock<IResult>();
        }

        private const int DecimalPrecision = 10;
        private const decimal StakeValue = 42M;
        private readonly Mock<IResult> _resultA;
        private readonly Mock<IResult> _resultB;
        private readonly Mock<IResult> _resultC;

        [Fact]
        public void AddExpectedResults_ReturnsCorrectly()
        {
            var expectedResults = new ResultSet(new[] {_resultA.Object});
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object});
            var wager = bet.AddExpectedResults(expectedResults, StakeValue);

            Assert.NotNull(wager.ExpectedResults);
            Assert.NotNull(wager.Stake);
            Assert.Equal(expectedResults, wager.ExpectedResults);
            Assert.Equal(StakeValue, wager.Stake.Value, DecimalPrecision);
        }

        [Fact]
        public void AddExpectedResults_ThrowsCorrectly()
        {
            var expectedResults = new ResultSet(new[] {_resultA.Object});
            var invalidExpectedResults = new ResultSet(new[] {_resultA.Object, _resultB.Object});
            var bet = new MultipleChoiceBet(new[] {_resultA.Object});

            Assert.Throws<ArgumentNullException>(() => bet.AddExpectedResults(null, StakeValue));
            Assert.Throws<ArgumentException>(() => bet.AddExpectedResults(expectedResults, 0));
            Assert.Throws<ArgumentException>(() => bet.AddExpectedResults(expectedResults, -1));
            Assert.Throws<ArgumentException>(() => bet.AddExpectedResults(invalidExpectedResults, StakeValue));
        }

        [Fact]
        public void Constructor_ThrowsCorrectly()
        {
            var resultsWithDuplicates = new[] {_resultA.Object, _resultA.Object};

            Assert.Throws<ArgumentNullException>(() => new MultipleChoiceBet(null));
            Assert.Throws<ArgumentException>(() => new MultipleChoiceBet(resultsWithDuplicates));
        }

        [Fact]
        public void GetOdds_ThrowsCorrectly()
        {
            var results = new ResultSet(new[] {_resultB.Object});
            var resultsWithImpossibleResult = new ResultSet(new[] {_resultC.Object});
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object});

            Assert.Throws<ArgumentNullException>(() => bet.GetOdds(null, results));
            Assert.Throws<ArgumentNullException>(() => bet.GetOdds(results, null));
            Assert.Throws<ArgumentException>(() => bet.GetOdds(resultsWithImpossibleResult, results));
            Assert.Throws<ArgumentException>(() => bet.GetOdds(results, resultsWithImpossibleResult));
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfCanceled()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultC.Object}), 1M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultB.Object, _resultC.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Canceled, outcomeA.Type);
            Assert.Equal(OutcomeType.Canceled, outcomeB.Type);
            Assert.Equal(OutcomeType.Canceled, outcomeC.Type);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfMultipleWinnersWithoutSplit()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object}), 3M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultB.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultB.Object, _resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(1M, outcomeA.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Win, outcomeB.Type);
            Assert.Equal(0M, outcomeB.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Loss, outcomeC.Type);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfMultipleWinnersWithSplit_1()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object}), 3M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultB.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(0.75M, outcomeA.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Win, outcomeB.Type);
            Assert.Equal(0.25M, outcomeB.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Loss, outcomeC.Type);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfMultipleWinnersWithSplit_2()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object}), 3M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultB.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(0M, outcomeA.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Win, outcomeB.Type);
            Assert.Equal(0M, outcomeB.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Win, outcomeC.Type);
            Assert.Equal(0M, outcomeC.Winnings, DecimalPrecision);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfMultipleWinnersWithSplit_3()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultC.Object}), 3M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object, _resultC.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(0.75M, outcomeA.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Win, outcomeB.Type);
            Assert.Equal(0.25M, outcomeB.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Loss, outcomeC.Type);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfSingleWinner()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);

            Assert.NotNull(outcomeA);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(0M, outcomeA.Winnings, DecimalPrecision);
        }

        [Fact]
        public void GetOutcome_ReturnsCorrectly_IfSingleWinnerMultipleLosers()
        {
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});
            var actualResults = new ResultSet(new[] {_resultA.Object});
            var wagerA = bet.AddExpectedResults(new ResultSet(new[] {_resultA.Object}), 1M);
            var wagerB = bet.AddExpectedResults(new ResultSet(new[] {_resultB.Object, _resultC.Object}), 1M);
            var wagerC = bet.AddExpectedResults(new ResultSet(new[] {_resultC.Object}), 1M);
            var outcomeA = bet.GetOutcome(wagerA, actualResults);
            var outcomeB = bet.GetOutcome(wagerB, actualResults);
            var outcomeC = bet.GetOutcome(wagerC, actualResults);

            Assert.NotNull(outcomeA);
            Assert.NotNull(outcomeB);
            Assert.NotNull(outcomeC);
            Assert.Equal(OutcomeType.Win, outcomeA.Type);
            Assert.Equal(2M, outcomeA.Winnings, DecimalPrecision);
            Assert.Equal(OutcomeType.Loss, outcomeB.Type);
            Assert.Equal(OutcomeType.Loss, outcomeC.Type);
        }

        [Fact]
        public void GetOutcome_ThrowsCorrectly()
        {
            var results = new ResultSet(new[] {_resultA.Object});
            var resultsWithImpossibleResult = new ResultSet(new[] {_resultC.Object});
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object});
            var wager = bet.AddExpectedResults(results, StakeValue);
            var foreignBet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object});
            var foreignWager = foreignBet.AddExpectedResults(results, StakeValue);

            Assert.Throws<ArgumentNullException>(() => bet.GetOutcome(null, results));
            Assert.Throws<ArgumentNullException>(() => bet.GetOutcome(wager, null));
            Assert.Throws<ArgumentException>(() => bet.GetOutcome(foreignWager, results));
            Assert.Throws<ArgumentException>(() => bet.GetOutcome(wager, resultsWithImpossibleResult));
        }

        [Fact]
        public void PossibleResults_ReturnsCorrectly()
        {
            var possibleResults = new[]
            {
                new ResultSet(new[] {_resultA.Object}),
                new ResultSet(new[] {_resultB.Object}),
                new ResultSet(new[] {_resultC.Object}),
                new ResultSet(new[] {_resultA.Object, _resultB.Object}),
                new ResultSet(new[] {_resultA.Object, _resultC.Object}),
                new ResultSet(new[] {_resultB.Object, _resultC.Object})
            };
            var bet = new MultipleChoiceBet(new[] {_resultA.Object, _resultB.Object, _resultC.Object});

            AssertHelper.CollectionContainsEqual(possibleResults, bet.PossibleResults);
        }
    }
}
