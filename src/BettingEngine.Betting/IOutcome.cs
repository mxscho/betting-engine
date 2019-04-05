namespace BettingEngine.Betting
{
    public interface IOutcome
    {
        OutcomeType Type { get; }

        decimal Winnings { get; }
    }
}