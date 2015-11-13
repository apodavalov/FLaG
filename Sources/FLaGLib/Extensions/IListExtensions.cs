using FLaGLib.Collections;
using System;
using System.Collections.Generic;

namespace FLaGLib.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> obj, IEnumerable<T> items)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (T item in items)
            {
                obj.Add(item);
            }
        }

        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> obj)
        {
            return new ReadOnlyList<T>(obj);
        }
    }
}
