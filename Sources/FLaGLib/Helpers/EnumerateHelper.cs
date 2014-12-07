using System.Collections.Generic;

namespace FLaGLib.Helpers
{
    public static class EnumerateHelper
    {
        public static IEnumerable<T> Sequence<T>(params T[] items)
        {
            foreach (T item in items)
            {
                yield return item;
            }

            yield break;
        }
    }
}
