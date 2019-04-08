using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingEngine.Betting
{
    /// <summary>
    ///     Represents a multiple-choice bet.
    ///     Multiple-choice bets are bets where the actual result can be a set of individual results.
    ///     Wagers which include at least one of these individual results in their expected results are considered winners.
    /// </summary>
    public class MultipleChoiceBet : IBet<IResultSet>
    {
        private readonly MultipleChoicePool _multipleChoicePool;

        /// <summary>
        ///     Creates a new instance of <see cref="MultipleChoiceBet" /> for a set of individual results.
        ///     The <see cref="PossibleResults" /> for this bet are equal to all proper subsets of the specified
        ///     set of individual results.
        /// </summary>
        /// <param name="availableResults">All individual results.</param>
        /// <exception cref="ArgumentNullException">
        ///     Occurs if <paramref name="availableResults" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Occurs if <paramref name="availableResults" /> contains duplicates or <c>null</c> values.
        /// </exception>
        public MultipleChoiceBet(IEnumerable<IResult> availableResults)
        {
            if (availableResults == null) throw new ArgumentNullException(nameof(availableResults));

            var availableResultsList = availableResults.ToList();

            if (availableResultsList.Contains(null))
                throw new ArgumentException("Specified value cannot contain null values.", nameof(availableResults));

            if (availableResultsList.Distinct().Count() != availableResultsList.Count)
                throw new ArgumentException("Specified value cannot contain duplicates.", nameof(availableResults));

            PossibleResults = new ResultSet(availableResultsList).Subsets;
            _multipleChoicePool = new MultipleChoicePool(PossibleResults);
        }

        /// <inheritdoc />
        public IEnumerable<IResultSet> PossibleResults { get; }

        /// <inheritdoc />
        public IWager<IResultSet> AddExpectedResults(IResultSet expectedResults, decimal stakeValue)
        {
            if (expectedResults == null) throw new ArgumentNullException(nameof(expectedResults));

            if (!PossibleResults.Contains(expectedResults))
                throw new ArgumentException(
                    "Specified value has to be one of the possible results.",
                    nameof(expectedResults));

            if (stakeValue <= 0)
                throw new ArgumentException(
                    "Specified value cannot be less than or equal to zero.",
                    nameof(stakeValue));

            var stake = new Stake(stakeValue);
            var wager = new Wager<IResultSet>(this, expectedResults, stake);
            _multipleChoicePool.AddWager(wager);

            return wager;
        }

        /// <inheritdoc />
        public decimal GetOdds(IResultSet expectedResults, IResultSet actualResults)
        {
            if (expectedResults == null) throw new ArgumentNullException(nameof(expectedResults));

            if (!PossibleResults.Contains(expectedResults))
                throw new ArgumentException(
                    "Specified value must be one of the possible results.",
                    nameof(expectedResults));

            if (actualResults == null) throw new ArgumentNullException(nameof(actualResults));

            if (!PossibleResults.Contains(actualResults))
                throw new ArgumentException(
                    "Specified value must be one of the possible results.",
                    nameof(actualResults));

            var pool = _multipleChoicePool.ForActualResults(actualResults);

            if (!pool.WinnerWagers.Exist) return 0M;

            if (!expectedResults.SharesResultWith(actualResults)) return -1M;

            var odds = 0M;

            // In the following calculation:
            // - A wager is a winner if its expected results share at least one result with the actual results.
            // - A wager is a loser if its expected results do not share any result with the actual results.
            // - A wager is contrary to another wager if their expected results do not share any result.

            // Divide stakes of loser wagers contrary to the evaluated expected results up among the respective
            // contrary winner wagers.
            odds += PossibleResults
                .Where(_ => pool.WinnerWagers.ContraryTo(_).Exist)
                .Select(_ =>
                    pool.LoserWagers.ContraryTo(expectedResults).WithExpectedResults(_).TotalStakeValue /
                    pool.WinnerWagers.ContraryTo(_).TotalStakeValue)
                .Sum();

            // Divide stakes of remaining loser wagers without contrary winner wagers up among all winner wagers.
            odds += pool.LoserWagers.WithoutContraryWinnerWagers.TotalStakeValue /
                    pool.WinnerWagers.TotalStakeValue;

            return odds;
        }

        /// <inheritdoc />
        public IOutcome GetOutcome(IWager<IResultSet> wager, IResultSet actualResults)
        {
            if (wager == null) throw new ArgumentNullException(nameof(wager));

            if (!_multipleChoicePool.ContainsWager(wager))
                throw new ArgumentException(
                    "Specified value is not part of this bet.",
                    nameof(wager));

            if (actualResults == null) throw new ArgumentNullException(nameof(actualResults));

            if (!PossibleResults.Contains(actualResults))
                throw new ArgumentException(
                    "Specified value must be one of the possible results.",
                    nameof(actualResults));

            if (!_multipleChoicePool.ForActualResults(actualResults).WinnerWagers.Exist)
                return Outcome.CreateCanceled();

            return wager.ExpectedResults.SharesResultWith(actualResults)
                ? Outcome.CreateWin(GetOdds(wager.ExpectedResults, actualResults) * wager.Stake.Value)
                : Outcome.CreateLoss();
        }
    }
}
