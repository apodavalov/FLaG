using System;
using System.Collections.Generic;

namespace FLaGLib.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static IEnumerable<T> FastReverse<T>(this IReadOnlyList<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            for (int i = obj.Count - 1; i >= 0; i--)
            {
                yield return obj[i];
            }
        }
    }
}
