using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.Extension
{
    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source) => new(source);

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey>? comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            comparer ??= Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        

        public static T GetRandomElement<T>(this IList<T> list, out int elIndex) =>
            list.GetRandomElement(out elIndex, new Random());

        public static T GetRandomElement<T>(this IList<T> list, out int elIndex, Random randomSource)
        {
            elIndex = DiscreteUniform.Sample(randomSource, 0, list.Count - 1);
            return list[elIndex];
        }


        public static IEnumerable<T> GetRandomElements<T>(this IList<T> list) => list.GetRandomElements(new Random());

        public static IEnumerable<T> GetRandomElements<T>(this IList<T> list, Random randomSource)
        {
            var tempList = list.ToList();

            for (var i = 0; i < list.Count; i++)
            {
                var elem = tempList.GetRandomElement(out var elIndex, randomSource);
                tempList.RemoveAt(elIndex);
                yield return elem;
            }
        }

        public static IEnumerable<T> GetRandomElementsExcept<T>(this IList<T> list, T excepted, Random randomSource)
        {
            var tempList = list.ToList();
            tempList.Remove(excepted);
            var listCount = tempList.Count;

            for (var i = 0; i < listCount; i++)
            {
                var elem = tempList.GetRandomElement(out var elIndex, randomSource);
                tempList.RemoveAt(elIndex);
                yield return elem;
            }
        }
        
        public static IEnumerable<T> GetRandomElementsAlt<T>(this IList<T> list, Random randomSource)
        {
            var tempArr = list.ToArray();

            for (var i = 0; i < tempArr.Length; i++)
            {
                var j = randomSource.Next(i, tempArr.Length);
                yield return tempArr[j];
                tempArr[j] = tempArr[i];
            }
        }


        public static IEnumerable<(T, T)> GetRandomUniquePairs<T>(this IList<T> list) =>
            GetRandomUniquePairs(list, new Random());

        public static IEnumerable<(T, T)> GetRandomUniquePairs<T>(this IList<T> list, Random randomSource)
        {
            var resultList = new List<(T, T)>();

            for (var i = 0; i < list.Count - 1; i++)
            for (var j = i + 1; j < list.Count; j++)
                resultList.Add((list[i], list[j]));

            return resultList.GetRandomElements(randomSource);
        }

        public static IEnumerable<(int, int)> GetRandomUniquePairsIndexes<T>(this IList<T> list, Random randomSource)
        {
            var resultList = new List<(int, int)>();

            for (var i = 0; i < list.Count - 1; i++)
            for (var j = i + 1; j < list.Count; j++)
                resultList.Add((i, j));

            return resultList.GetRandomElements(randomSource);
        }

        // public static IEnumerable<(T, T)> GetRandomUniquePairsAlt<T>(this IList<T> list, Random randomSource)
        // {
        //     return Enumerable.Range(0, list.Count - 1)
        //         .SelectMany(
        //             i => Enumerable.Range(i + 1, list.Count - (i + 1)),
        //             (i, j) => (list[i], list[j]))
        //         .ToList().GetRandomElements(randomSource);
        // }
    }
}