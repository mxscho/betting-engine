using System;

namespace BettingEngine.Betting
{
    /// <summary>
    /// Used to track outcome and potential winnings for a specific <see cref="IBet{TResults}"/>.
    /// </summary>
    /// <typeparam name="TResults">The type of one (possible) result, which can also be a set of results.</typeparam>
    public interface IWager<TResults>
    {
        /// <summary>
        /// The results that are expected to happen for a specific <see cref="IBet{TResults}"/>.
        /// </summary>
        TResults ExpectedResults { get; }

        /// <summary>
        /// The stake used to make this wager. Also determines the amount of winnings in case of a win.
        /// </summary>
        IStake Stake { get; }

        /// <summary>
        /// Calculates the outcome for specific actual results. 
        /// </summary>
        /// <param name="actualResults">The actual results.</param>
        /// <returns>
        /// An instance of <see cref="IOutcome"/> which represents the outcome for specific actual results.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs if <paramref name="actualResults"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs if <paramref name="actualResults"/> is not one of the values of
        /// <see cref="IBet{TResults}.PossibleResults"/>.
        /// </exception>
        IOutcome GetOutcome(TResults actualResults);
    }
}