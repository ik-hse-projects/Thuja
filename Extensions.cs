using System;
using System.Collections.Generic;

namespace Thuja
{
    /// <summary>
    ///     Некоторые вспомогательные методы для удобства.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Удаляет из переданного списка последние <paramref name="count" /> элементов.
        /// </summary>
        /// <param name="list">Список, из которого нужно удалить элементы.</param>
        /// <param name="count">Количество элементов, которые нужно удалить.</param>
        /// <typeparam name="T">Тип элементов списка.</typeparam>
        public static void RemoveLast<T>(this List<T> list, int count = 1)
        {
            list.RemoveRange(list.Count - count, count);
        }

        /// <summary>
        ///     Группирует подряд идущие элементы в итераторе по некотрому ключу.
        /// </summary>
        /// <param name="enumerable">Итератор с элементами.</param>
        /// <param name="selector">Функция получения ключа элемента.</param>
        /// <typeparam name="TSource">Тип элемента.</typeparam>
        /// <typeparam name="TKey">Тип ключа.</typeparam>
        /// <returns>Возвращает итератор групп элементов.</returns>
        public static IEnumerable<(TKey, List<TSource>)> MakeGroups<TSource, TKey>(this IEnumerable<TSource> enumerable,
            Func<TSource, TKey> selector)
        {
            using var enumerator = enumerable.GetEnumerator();
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

        /// <summary>
        ///     Находит минимальный элемент итератора по некоторому ключу.
        /// </summary>
        /// <param name="enumerable">Итератор элементов.</param>
        /// <param name="keySelector">Функция получения ключа.</param>
        /// <typeparam name="T">Тип элемента.</typeparam>
        /// <returns>Минимальный элемент, либо null, если итератор пуст.</returns>
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