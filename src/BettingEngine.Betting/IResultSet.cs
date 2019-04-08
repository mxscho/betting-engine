using System;
using System.Collections.Generic;

namespace BettingEngine.Betting
{
    /// <summary>
    ///     Represents a set of (possible) <see cref="IResult" /> instances. This can be used if the results that can
    ///     happen is actually a set of multiple individual results.
    /// </summary>
    public interface IResultSet : IEquatable<IResultSet>
    {
        /// <summary>
        ///     The individual results of this set.
        /// </summary>
        IEnumerable<IResult> Results { get; }

        /// <summary>
        ///     Determines if this result set shares any results with another <see cref="IResultSet" />.
        /// </summary>
        /// <param name="other">The other result set.</param>
        /// <returns>True if both result sets share results, false otherwise.</returns>
        bool SharesResultWith(IResultSet other);
    }
}
