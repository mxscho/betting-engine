using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingEngine.Betting
{
    /// <summary>
    ///     Represents a single-choice bet.
    ///     Single-choice bets are bets where the actual result can be exactly one individual result.
    ///     Wagers which have exactly the same individual result as their expected result are considered winners.
    /// </summary>
    public class SingleChoiceBet : IBet<IResult>
    {
        private readonly IBet<IResultSet> _multipleChoiceBet;
        private readonly IDictionary<IWager<IResult>, IWager<IResultSet>> _wagers;

        /// <summary>
        ///     Creates a new instance of <see cref="SingleChoiceBet" /> for a set of possible results.
        /// </summary>
        /// <param name="possibleResults">The possible results.</param>
        /// <exception cref="ArgumentNullException">
        ///     Occurs if <paramref name="possibleResults" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Occurs if <paramref name="possibleResults" /> contains duplicates or <c>null</c> values.
        /// </exception>
        public SingleChoiceBet(IEnumerable<IResult> possibleResults)
        {
            if (possibleResults == null) throw new ArgumentNullException(nameof(possibleResults));

            var possibleResultsList = possibleResults.ToList();

            if (possibleResultsList.Contains(null))
                throw new ArgumentException("Specified value cannot contain null values.", nameof(possibleResults));

            if (possibleResultsList.Distinct().Count() != possibleResultsList.Count)
                throw new ArgumentException("Specified value cannot contain duplicates.", nameof(possibleResults));

            PossibleResults = possibleResultsList;
            _multipleChoiceBet = new MultipleChoiceBet(possibleResultsList);
            _wagers = new Dictionary<IWager<IResult>, IWager<IResultSet>>();
        }

        /// <inheritdoc />
        public IEnumerable<IResult> PossibleResults { get; }

        /// <inheritdoc />
        public IWager<IResult> AddExpectedResults(IResult expectedResults, decimal stakeValue)
        {
            var multipleChoiceWager = _multipleChoiceBet.AddExpectedResults(
                new ResultSet(new[] {expectedResults}),
                stakeValue);
            var wager = new Wager<IResult>(
                this,
                multipleChoiceWager.ExpectedResults.Results.Single(),
                multipleChoiceWager.Stake);
            _wagers[wager] = multipleChoiceWager;
            return wager;
        }

        /// <inheritdoc />
        public decimal GetOdds(IResult expectedResults, IResult actualResults)
        {
            return _multipleChoiceBet.GetOdds(
                new ResultSet(new[] {expectedResults}),
                new ResultSet(new[] {actualResults}));
        }

        /// <inheritdoc />
        public IOutcome GetOutcome(IWager<IResult> wager, IResult actualResults)
        {
            if (wager == null) throw new ArgumentNullException(nameof(wager));

            if (!_wagers.ContainsKey(wager))
                throw new ArgumentException(
                    "Specified value is not part of this bet.",
                    nameof(wager));

            return _multipleChoiceBet.GetOutcome(
                _wagers[wager],
                new ResultSet(new[] {actualResults}));
        }
    }
}
