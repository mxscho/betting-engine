namespace BettingEngine.Betting
{
    public class Wager<TResults> : IWager<TResults>
    {
        private readonly IBet<TResults> _bet;

        internal Wager(IBet<TResults> bet, TResults expectedResults, IStake stake)
        {
            _bet = bet;
            ExpectedResults = expectedResults;
            Stake = stake;
        }

        public TResults ExpectedResults { get; }

        public IStake Stake { get; }

        public IOutcome GetOutcome(TResults actualResults)
        {
            return _bet.GetOutcome(this, actualResults);
        }
    }
}