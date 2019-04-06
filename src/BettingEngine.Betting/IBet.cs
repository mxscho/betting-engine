using System.Collections.Generic;

namespace BettingEngine.Betting
{
    /// <summary>
    /// Represents a bet. Can be used to calculate odds or the outcome of a <see cref="IWager{TResults}"/>.
    /// </summary>
    /// <typeparam name="TResults">The type of one (possible) result, which can also be a set of results.</typeparam>
    public interface IBet<TResults>
    {
        /// <summary>
        /// All possible results of the bet. All possible results can then be selected as actual results.
        /// </summary>
        IEnumerable<TResults> PossibleResults { get; }

        /// <summary>
        /// Creates a new wager by adding expected results together with stake to the bet.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="stakeValue">The stake's value.</param>
        /// <returns>A new wager which is used to track outcome and potential winnings later on.</returns>
        IWager<TResults> AddExpectedResults(TResults expectedResults, decimal stakeValue);

        /// <summary>
        /// Calculates the current odds of an expected result for an actual result happening. 
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="actualResults">The actual results.</param>
        /// <returns>
        /// The odds in a decimal form.
        /// This value times a wager's stake represents the amount of winnings a wager with the specified
        /// expected results is able to claim from the pool if the specified actual results happen.
        /// </returns>
        decimal GetOdds(TResults expectedResults, TResults actualResults);

        /// <summary>
        /// Calculates the outcome for a specified wager and specific actual results. 
        /// </summary>
        /// <param name="wager">A wager which has been created by adding expected results to this bet.</param>
        /// <param name="actualResults">The actual results.</param>
        /// <returns>
        /// An instance of <see cref="IOutcome"/> which represents the outcome of the specified wager
        /// for specific actual results.
        /// </returns>
        IOutcome GetOutcome(IWager<TResults> wager, TResults actualResults);
    }
}