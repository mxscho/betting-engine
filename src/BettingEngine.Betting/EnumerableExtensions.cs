using System.Collections.Generic;
using System.Linq;

namespace BettingEngine.Betting
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Subsets<T>(
            this IEnumerable<T> source,
            bool includeEmpty = true,
            bool includeSelf = true)
        {
            var sourceList = source.ToList();
            if (!sourceList.Any()) return Enumerable.Repeat(sourceList, 1);

            var element = sourceList.Take(1);
            var without = sourceList.Skip(1).Subsets().ToList();
            var with = without.Select(set => element.Concat(set));

            return with
                .Concat(without)
                .Where(set => includeEmpty || set.Any())
                .Where(set => includeSelf || set.Count() != sourceList.Count);
        }
    }
}
