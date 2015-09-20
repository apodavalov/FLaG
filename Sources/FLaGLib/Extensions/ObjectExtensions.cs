using System.Collections.Generic;

namespace FLaGLib.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsSequence<T>(this T obj)
        {
            yield return obj;
        }

        public static int GetHashCodeNullable(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        public static T Of<T>(this object obj)
        {
            return (T)obj;
        }
    }
}
