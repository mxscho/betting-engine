using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BettingEngine.Betting
{
    public class ResultSet : IResultSet
    {
        private readonly Lazy<IEnumerable<IResultSet>> _subsets;

        public ResultSet(IEnumerable<IResult> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            var resultsList = results.ToList();

            if (resultsList.Contains(null))
                throw new ArgumentException("Specified value cannot contain null values.", nameof(results));

            if (resultsList.Distinct().Count() != resultsList.Count)
                throw new ArgumentException("Specified value cannot contain duplicates.", nameof(results));

            Results = new ReadOnlyCollection<IResult>(resultsList);
            _subsets = new Lazy<IEnumerable<IResultSet>>(() => new ReadOnlyCollection<IResultSet>(Results
                .Subsets(false, false)
                .Select(_ => (IResultSet) new ResultSet(_))
                .ToList()));
        }

        public IEnumerable<IResult> Results { get; }

        public IEnumerable<IResultSet> Subsets => _subsets.Value;

        public bool Equals(IResultSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Results.Count() == other.Results.Count() &&
                   Results.Intersect(other.Results).Count() == Results.Count();
        }

        public bool SharesResultWith(IResultSet other)
        {
            return Results.Any(_ => other.Results.Contains(_));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is IResultSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Results != null ? Results.Aggregate(0, (a, b) => a.GetHashCode() ^ b.GetHashCode()) : 0;
        }
    }
}