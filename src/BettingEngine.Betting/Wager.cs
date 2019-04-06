namespace BettingEngine.Betting
{
    /// <inheritdoc />
    public class Wager<TResults> : IWager<TResults>
    {
        private readonly IBet<TResults> _bet;

        internal Wager(IBet<TResults> bet, TResults expectedResults, IStake stake)
        {
            _bet = bet;
            ExpectedResults = expectedResults;
            Stake = stake;
        }

        /// <inheritdoc />
        public TResults ExpectedResults { get; }

        /// <inheritdoc />
        public IStake Stake { get; }

        /// <inheritdoc />
        public IOutcome GetOutcome(TResults actualResults)
        {
            return _bet.GetOutcome(this, actualResults);
        }
    }
}