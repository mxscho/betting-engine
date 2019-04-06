namespace BettingEngine.Betting
{
    /// <summary>
    /// Represents the type of an <see cref="IOutcome"/>.
    /// </summary>
    public enum OutcomeType
    {
        /// <summary>
        /// A win means that the <see cref="IWager{TResults}"/> can keep its stake and also claim potential winnings.
        /// </summary>
        Win,

        /// <summary>
        /// A win means that the <see cref="IWager{TResults}"/> loses its stake and does not get any winnings. 
        /// </summary>
        Loss,

        /// <summary>
        /// Cancellation occurs for a <see cref="IWager{TResults}"/> if the corresponding <see cref="IBet{TResults}"/>
        /// has been canceled, e.g. because there do not exist any winning wagers at all.
        /// </summary>
        Canceled
    }
}