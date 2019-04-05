using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingEngine.Betting
{
    public class MultipleChoiceBet : IBet<IResultSet>
    {
        private readonly MultipleChoicePool _multipleChoicePool;

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

        public IEnumerable<IResultSet> PossibleResults { get; }

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