using System.Collections;
using System.Collections.Generic;

namespace FLaGLib.Collections
{
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        bool IsProperSubsetOf(IEnumerable<T> other);
        bool IsProperSupersetOf(IEnumerable<T> other);
        bool IsSubsetOf(IEnumerable<T> other);
        bool IsSupersetOf(IEnumerable<T> other);
        bool Overlaps(IEnumerable<T> other);
        bool SetEquals(IEnumerable<T> other);
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        bool IsReadOnly { get; } 
    }
}
