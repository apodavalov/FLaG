using System;
using System.Collections.Generic;

namespace FLaGLib.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private IList<T> _List;

        public ReadOnlyList(IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            _List = list;
        }

        public T this[int index]
        {
            get 
            {
                return _List[index];
            }
        }

        public int Count
        {
            get 
            {
                return _List.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_List).GetEnumerator();
        }
    }
}
