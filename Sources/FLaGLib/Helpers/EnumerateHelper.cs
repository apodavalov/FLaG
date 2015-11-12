using FLaGLib.Extensions;
using System.Collections.Generic;

namespace FLaGLib.Helpers
{
    public static class EnumerateHelper
    {
        public static IEnumerable<T> ReverseSequence<T>(params T[] items)
        {
            return items.FastReverse();
        }

        public static IEnumerable<T> Sequence<T>(params T[] items)
        {
            return items;
        }
    }
}
