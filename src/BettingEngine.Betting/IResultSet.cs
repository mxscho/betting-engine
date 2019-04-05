using System;
using System.Collections.Generic;

namespace BettingEngine.Betting
{
    public interface IResultSet : IEquatable<IResultSet>
    {
        IEnumerable<IResult> Results { get; }

        bool SharesResultWith(IResultSet other);
    }
}
