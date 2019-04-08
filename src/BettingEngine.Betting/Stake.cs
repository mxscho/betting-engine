namespace BettingEngine.Betting
{
    /// <inheritdoc />
    public class Stake : IStake
    {
        internal Stake(decimal value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public decimal Value { get; }
    }
}
