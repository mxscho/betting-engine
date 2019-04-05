namespace BettingEngine.Betting
{
    public class Stake : IStake
    {
        internal Stake(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }
    }
}