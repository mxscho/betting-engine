using System;
using Moq;
using Xunit;

namespace BettingEngine.Betting.Tests
{
    public class ResultSetTest
    {
        public ResultSetTest()
        {
            _resultA = new Mock<IResult>();
            _resultB = new Mock<IResult>();
            _resultC = new Mock<IResult>();
        }

        private readonly Mock<IResult> _resultA;
        private readonly Mock<IResult> _resultB;
        private readonly Mock<IResult> _resultC;

        [Fact]
        public void Constructor_ThrowsCorrectly()
        {
            var resultsWithDuplicates = new[] {_resultA.Object, _resultA.Object};

            Assert.Throws<ArgumentNullException>(() => new ResultSet(null));
            Assert.Throws<ArgumentException>(() => new ResultSet(resultsWithDuplicates));
        }

        [Fact]
        public void Equals_ReturnsCorrectly()
        {
            var resultsA = new[] {_resultA.Object};
            var resultSetA = new ResultSet(resultsA);
            var resultsB = new[] {_resultB.Object};
            var resultSetB = new ResultSet(resultsB);
            var resultsBoth = new[] {_resultA.Object, _resultB.Object};
            var resultSetBoth = new ResultSet(resultsBoth);
            var resultsBothReversed = new[] {_resultB.Object, _resultA.Object};
            var resultSetBothReversed = new ResultSet(resultsBothReversed);

            Assert.NotEqual(
                resultSetA,
                resultSetB);
            Assert.Equal(
                resultSetBoth,
                resultSetBothReversed);
        }

        [Fact]
        public void Results_ReturnsCorrectly()
        {
            var results = new[] {_resultA.Object, _resultB.Object};
            var resultSet = new ResultSet(results);

            AssertHelper.CollectionContainsEqual(results, resultSet.Results);
        }

        [Fact]
        public void SharesResultWith_ReturnsCorrectly()
        {
            var resultsA = new[] {_resultA.Object};
            var resultSetA = new ResultSet(resultsA);
            var resultsB = new[] {_resultB.Object};
            var resultSetB = new ResultSet(resultsB);
            var resultsBoth = new[] {_resultA.Object, _resultB.Object};
            var resultSetBoth = new ResultSet(resultsBoth);

            Assert.False(resultSetA.SharesResultWith(resultSetB));
            Assert.True(resultSetA.SharesResultWith(resultSetBoth));
            Assert.True(resultSetBoth.SharesResultWith(resultSetA));
        }

        [Fact]
        public void Subsets_ReturnsCorrectly()
        {
            var results = new[] {_resultA.Object, _resultB.Object, _resultC.Object};
            var resultSet = new ResultSet(results);

            AssertHelper.CollectionContainsEqual(new[]
            {
                new ResultSet(new[] {_resultA.Object}),
                new ResultSet(new[] {_resultB.Object}),
                new ResultSet(new[] {_resultC.Object}),
                new ResultSet(new[] {_resultA.Object, _resultB.Object}),
                new ResultSet(new[] {_resultA.Object, _resultC.Object}),
                new ResultSet(new[] {_resultB.Object, _resultC.Object})
            }, resultSet.Subsets);
        }
    }
}
