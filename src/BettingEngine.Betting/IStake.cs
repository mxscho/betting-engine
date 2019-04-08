namespace BettingEngine.Betting
{
    /// <summary>
    ///     Represents a stake used to make a <see cref="IWager{TResults}" /> for a <see cref="IBet{TResults}" />.
    /// </summary>
    public interface IStake
    {
        /// <summary>
        ///     The value of the stake, e.g. a specific amount of money.
        /// </summary>
        decimal Value { get; }
    }
}
