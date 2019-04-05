using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingEngine.Betting
{
    internal class MultipleChoicePool
    {
        private readonly IDictionary<IResultSet, ActualResultsPool> _actualResultsPools;

        private readonly IEnumerable<IResultSet> _possibleResults;
        private readonly ISet<IWager<IResultSet>> _wagers;

        public MultipleChoicePool(IEnumerable<IResultSet> possibleResults)
        {
            _possibleResults = possibleResults;
            _actualResultsPools = _possibleResults
                .ToDictionary(_ => _, _ => new ActualResultsPool(this, _));
            _wagers = new HashSet<IWager<IResultSet>>();
        }

        public ActualResultsPool ForActualResults(IResultSet actualResults)
        {
            return _actualResultsPools[actualResults];
        }

        public bool ContainsWager(IWager<IResultSet> wager)
        {
            return _wagers.Contains(wager);
        }

        public void AddWager(IWager<IResultSet> wager)
        {
            _wagers.Add(wager);

            foreach (var possibleResults in _possibleResults) ForActualResults(possibleResults).AddWager(wager);
        }

        public class ActualResultsPool
        {
            private readonly IResultSet _actualResults;

            private readonly IDictionary<IResultSet, IDictionary<IResultSet, decimal>>
                _contraryLoserWagersWithExpectedResultsTotalStakeValue;

            private readonly IDictionary<IResultSet, decimal> _contraryWinnerWagersTotalStakeValue;
            private readonly IDictionary<IResultSet, decimal> _loserWagersWithoutContraryWinnerWagersTotalStakeValue;
            private readonly MultipleChoicePool _multipleChoicePool;

            public ActualResultsPool(MultipleChoicePool multipleChoicePool, IResultSet actualResults)
            {
                _multipleChoicePool = multipleChoicePool;
                _actualResults = actualResults;
                _contraryWinnerWagersTotalStakeValue = _multipleChoicePool._possibleResults
                    .ToDictionary(_ => _, _ => 0M);
                _loserWagersWithoutContraryWinnerWagersTotalStakeValue = _multipleChoicePool._possibleResults
                    .ToDictionary(_ => _, _ => 0M);
                _contraryLoserWagersWithExpectedResultsTotalStakeValue = _multipleChoicePool._possibleResults
                    .ToDictionary(
                        a => a,
                        a => (IDictionary<IResultSet, decimal>) _multipleChoicePool._possibleResults
                            .ToDictionary(b => b, b => 0M));
            }

            public (
                bool Exist,
                decimal TotalStakeValue,
                Func<IResultSet, (
                    bool Exist,
                    decimal TotalStakeValue
                    )>
                ContraryTo)
                WinnerWagers =>
            (
                ContainsWinnerWagers,
                WinnerWagersStakeValue,
                expectedResults => (
                    ContainsContraryWinnerWagers(expectedResults),
                    GetContraryWinnerWagersTotalStakeValue(expectedResults)
                )
            );

            public (
                (
                decimal TotalStakeValue,
                object Dummy
                ) WithoutContraryWinnerWagers,
                Func<IResultSet, (
                    Func<IResultSet, (
                        decimal TotalStakeValue,
                        object Dummy
                        )> WithExpectedResults,
                    object Dummy
                    )>ContraryTo)
                LoserWagers =>
            (
                (
                    LoserWagersWithoutContraryWinnerWagersStakeValue,
                    null
                ),
                expectedResults => (
                    contraryLoserWagerExpectedResults => (
                        GetContraryLoserWagersWithExpectedResultsTotalStakeValue(expectedResults,
                            contraryLoserWagerExpectedResults),
                        null
                    ),
                    null
                )
            );

            private bool ContainsWinnerWagers => WinnerWagersStakeValue > 0;

            private decimal WinnerWagersStakeValue { get; set; }

            private decimal LoserWagersWithoutContraryWinnerWagersStakeValue =>
                _loserWagersWithoutContraryWinnerWagersTotalStakeValue.Values.Sum();

            private bool ContainsContraryWinnerWagers(IResultSet expectedResults)
            {
                return !_loserWagersWithoutContraryWinnerWagersTotalStakeValue.ContainsKey(expectedResults);
            }

            private decimal GetContraryWinnerWagersTotalStakeValue(IResultSet expectedResults)
            {
                return _contraryWinnerWagersTotalStakeValue[expectedResults];
            }

            private decimal GetContraryLoserWagersWithExpectedResultsTotalStakeValue(
                IResultSet expectedResults,
                IResultSet contraryLoserWagersExpectedResults)
            {
                return _contraryLoserWagersWithExpectedResultsTotalStakeValue[
                    expectedResults][
                    contraryLoserWagersExpectedResults];
            }

            public void AddWager(IWager<IResultSet> wager)
            {
                if (_actualResults.SharesResultWith(wager.ExpectedResults))
                    AddWinnerWager(wager);
                else
                    AddLoserWager(wager);
            }

            private void AddWinnerWager(IWager<IResultSet> wager)
            {
                WinnerWagersStakeValue += wager.Stake.Value;

                foreach (var possibleResults in _multipleChoicePool._possibleResults)
                    if (!possibleResults.SharesResultWith(wager.ExpectedResults))
                    {
                        _loserWagersWithoutContraryWinnerWagersTotalStakeValue.Remove(possibleResults);
                        _contraryWinnerWagersTotalStakeValue[possibleResults] += wager.Stake.Value;
                    }
            }

            private void AddLoserWager(IWager<IResultSet> wager)
            {
                if (_loserWagersWithoutContraryWinnerWagersTotalStakeValue.ContainsKey(wager.ExpectedResults))
                    _loserWagersWithoutContraryWinnerWagersTotalStakeValue[wager.ExpectedResults] += wager.Stake.Value;

                foreach (var possibleResults in _multipleChoicePool._possibleResults)
                    if (!possibleResults.SharesResultWith(wager.ExpectedResults))
                        _contraryLoserWagersWithExpectedResultsTotalStakeValue[
                            possibleResults][
                            wager.ExpectedResults] += wager.Stake.Value;
            }
        }
    }
}