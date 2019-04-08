using System;

namespace BettingEngine.Betting
{
    /// <inheritdoc />
    public class Outcome : IOutcome
    {
        private readonly decimal _winnings;

        private Outcome(OutcomeType type, decimal winnings)
        {
            Type = type;
            _winnings = winnings;
        }

        /// <inheritdoc />
        public OutcomeType Type { get; }

        /// <inheritdoc />
        public decimal Winnings
        {
            get
            {
                if (Type != OutcomeType.Win) throw new InvalidOperationException("Outcome type is not a win.");

                return _winnings;
            }
        }

        internal static Outcome CreateWin(decimal winnings)
        {
            return new Outcome(OutcomeType.Win, winnings);
        }

        internal static Outcome CreateLoss()
        {
            return new Outcome(OutcomeType.Loss, 0M);
        }

        internal static Outcome CreateCanceled()
        {
            return new Outcome(OutcomeType.Canceled, 0M);
        }
    }
}
