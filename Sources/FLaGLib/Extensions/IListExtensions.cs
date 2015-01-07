using FLaGLib.Collections;
using System.Collections.Generic;

namespace FLaGLib.Extensions
{
    public static class IListExtensions
    {
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> obj)
        {
            return new ReadOnlyList<T>(obj);
        }
    }
}
