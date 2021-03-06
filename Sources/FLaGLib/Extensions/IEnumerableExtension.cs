﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Extensions
{
    public static class IEnumerableExtension
    {
        public static SortedDictionary<TKey, TSource> ToSortedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            SortedDictionary<TKey, TSource> result = new SortedDictionary<TKey, TSource>();

            foreach (TSource item in source)
            {
                result.Add(keySelector(item), item);
            }

            return result;

        }

        public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }

            SortedDictionary<TKey, TElement> result = new SortedDictionary<TKey, TElement>();

            foreach (TSource item in source)
            {
                result.Add(keySelector(item),elementSelector(item));
            }

            return result;
        }

        public static IDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> obj)
        {
            return obj.ToSortedDictionary(x => x.Key, x => x.Value);
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> obj)
        {
            return obj.ToDictionary(x => x.Key, x => x.Value);
        }

        public static bool AnyNull<T>(this IEnumerable<T> obj) where T : class
        {
            return obj.Any(item => item == null);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> obj, params T[] items)
        {
            foreach (T element in obj)
            {
                yield return element;
            }

            foreach (T element in items)
            {
                yield return element;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            //if (obj.Is<HashSet<T>>())
            //{
            //    return obj.Of<HashSet<T>>();
            //}

            return new HashSet<T>(obj);
        }

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            //if (obj.Is<SortedSet<T>>())
            //{
            //    return obj.Of<SortedSet<T>>();
            //}

            return new SortedSet<T>(obj);
        }
        
        public static int GetSequenceHashCode<T>(this IEnumerable<T> obj) where T : IEquatable<T>
        {
            if (obj == null)
            {
                return 0;
            }

            int hash = 0;

            foreach (T o in obj)
            {
                hash ^= o.GetHashCodeNullable();
            }

            return hash;
        }

        public static int SequenceCompare<T>(this IEnumerable<T> obj, IEnumerable<T> other) where T : IComparable<T>
        {
            if (obj == null && other == null)
            {
                return 0;
            }

            if (obj == null)
            {
                return -1;
            }

            if (other == null)
            {
                return 1;
            }

            int result = 0;

            using (IEnumerator<T> enumerator1 = obj.GetEnumerator())
            {
                using (IEnumerator<T> enumerator2 = other.GetEnumerator())
                {

                    bool hasNext1 = enumerator1.MoveNext();
                    bool hasNext2 = enumerator2.MoveNext();

                    while (hasNext1 && hasNext2)
                    {
                        result = enumerator1.Current.CompareToNullable(enumerator2.Current);

                        if (result != 0)
                        {
                            return result;
                        }

                        hasNext1 = enumerator1.MoveNext();
                        hasNext2 = enumerator2.MoveNext();
                    }

                    if (hasNext1)
                    {
                        return 1;
                    }

                    if (hasNext2)
                    {
                        return -1;
                    }

                    return 0;
                }
            }
        }
    }
}
