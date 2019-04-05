namespace BettingEngine.Betting
{
    public interface IWager<TResults>
    {
        TResults ExpectedResults { get; }

        IStake Stake { get; }

        IOutcome GetOutcome(TResults actualResults);
    }
}