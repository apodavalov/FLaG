using System;
using System.Collections.Generic;

namespace FLaGLib.Extensions
{
    public static class IEnumerableExtension
    {
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

            IEnumerator<T> enumerator1 = obj.GetEnumerator();
            IEnumerator<T> enumerator2 = other.GetEnumerator();

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
