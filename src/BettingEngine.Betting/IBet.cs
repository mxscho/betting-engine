using System.Collections.Generic;

namespace BettingEngine.Betting
{
    public interface IBet<TResults>
    {
        IEnumerable<TResults> PossibleResults { get; }

        IWager<TResults> AddExpectedResults(TResults expectedResults, decimal stakeValue);

        decimal GetOdds(TResults expectedResults, TResults actualResults);

        IOutcome GetOutcome(IWager<TResults> wager, TResults actualResults);
    }
}