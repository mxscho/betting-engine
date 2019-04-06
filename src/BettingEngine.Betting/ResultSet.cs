using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BettingEngine.Betting
{
    /// <inheritdoc />
    public class ResultSet : IResultSet
    {
        private readonly Lazy<IEnumerable<IResultSet>> _subsets;

        /// <summary>
        /// Creates a new instance of <see cref="ResultSet"/> for a specific set of individual <see cref="IResult"/>
        /// instances.
        /// </summary>
        /// <param name="results">The set of individual results.</param>
        /// <exception cref="ArgumentNullException">Occurs if <paramref name="results"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// Occurs if <paramref name="results"/> contains duplicates or <c>null</c> values.
        /// </exception>
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

        /// <inheritdoc />
        public IEnumerable<IResult> Results { get; }

        /// <summary>
        /// Gets all proper subsets of this <see cref="ResultSet"/>.
        /// </summary>
        public IEnumerable<IResultSet> Subsets => _subsets.Value;

        /// <inheritdoc />
        public bool Equals(IResultSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Results.Count() == other.Results.Count() &&
                   Results.Intersect(other.Results).Count() == Results.Count();
        }

        /// <inheritdoc />
        public bool SharesResultWith(IResultSet other)
        {
            return Results.Any(_ => other.Results.Contains(_));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is IResultSet other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Results != null ? Results.Aggregate(0, (a, b) => a.GetHashCode() ^ b.GetHashCode()) : 0;
        }
    }
}