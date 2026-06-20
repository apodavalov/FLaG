namespace FLaGLib.Extensions
{
    public static class IEnumerableExtension
    {
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> obj) => obj.ToSortedSet();

        public static int SequenceCompare<T>(this IEnumerable<T> obj, IEnumerable<T> other)
            where T : IComparable<T>
        {
            using IEnumerator<T> enumerator1 = obj.GetEnumerator();
            using IEnumerator<T> enumerator2 = other.GetEnumerator();

            bool hasNext1 = enumerator1.MoveNext();
            bool hasNext2 = enumerator2.MoveNext();

            while (hasNext1 && hasNext2)
            {
                int result = enumerator1.Current.CompareTo(enumerator2.Current);

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
