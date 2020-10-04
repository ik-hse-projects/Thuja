using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja
{
    public static class ListExt
    {
        public static void RemoveLast<T>(this List<T> self, int count)
        {
            self.RemoveRange(self.Count - count, count);
        }

        public static IEnumerable<(TKey, List<TSource>)> MakeGroups<TSource, TKey>(this IEnumerable<TSource> self,
            Func<TSource, TKey> selector)
        {
            var enumerator = self.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var first = enumerator.Current;
            var key = selector(first);
            var group = new List<TSource> {first};

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var currentKey = selector(current);
                if (currentKey.Equals(key))
                {
                    group.Add(current);
                }
                else
                {
                    yield return (key, group);
                    key = currentKey;
                    group = new List<TSource> {current};
                }
            }

            yield return (key, group);
        }
    }
}