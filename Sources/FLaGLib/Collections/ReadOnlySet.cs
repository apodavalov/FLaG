using System;
using System.Collections.Generic;

namespace FLaGLib.Collections
{
    public class ReadOnlySet<T> : IReadOnlySet<T>
    {
        private ISet<T> _Set;

        public ReadOnlySet(ISet<T> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            _Set = set;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _Set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _Set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _Set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _Set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _Set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _Set.SetEquals(other);
        }

        public bool Contains(T item)
        {
            return _Set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _Set.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Set.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Set.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Set).GetEnumerator();
        }
    }
}
