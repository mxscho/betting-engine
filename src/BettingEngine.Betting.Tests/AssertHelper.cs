using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BettingEngine.Betting.Tests
{
    public static class AssertHelper
    {
        public static void CollectionContainsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            Assert.Collection(
                expectedList,
                actualList
                    .Select<T, Action<T>>(_ => result => Assert.Contains(result, actualList))
                    .ToArray());
            Assert.Collection(
                actualList,
                expectedList
                    .Select<T, Action<T>>(_ => result => Assert.Contains(result, expectedList))
                    .ToArray());
        }
    }
}
