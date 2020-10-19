using System;
using System.Collections.Generic;

namespace Thuja
{
    public static class Extensions
    {
        public static void RemoveLast<T>(this List<T> self, int count = 1)
        {
            self.RemoveRange(self.Count - count, count);
        }

        public static IEnumerable<(TKey, List<TSource>)> MakeGroups<TSource, TKey>(this IEnumerable<TSource> self,
            Func<TSource, TKey> selector)
        {
            using var enumerator = self.GetEnumerator();
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
                if (currentKey is null && key is null
                    || !(currentKey is null) && currentKey.Equals(key))
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

        public static T? MinBy<T>(this IEnumerable<T> enumerable, Func<T, int> keySelector) where T : struct
        {
            (int key, T item)? result = null;
            foreach (var item in enumerable)
            {
                var key = keySelector(item);
                if (result == null || key < result.Value.key)
                {
                    result = (key, item);
                }
            }

            return result?.item;
        }
    }
}