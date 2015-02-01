using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Helpers
{
    public class TreeCollection<E,C> : IEnumerable<Tree<E,C>> where C : TreeCollection<E, C>
    {
        protected IEnumerable<Tree<E,C>> _Internal;

        public TreeOperator Operator
        {
            get;
            private set;
        }

        public TreeCollection(IEnumerable<Tree<E,C>> subtrees, TreeOperator @operator)
        {
            Operator = @operator;

            if (Operator == TreeOperator.Concat)
            {
                _Internal = new List<Tree<E,C>>(subtrees);
            }
            else
            {
                _Internal = new SortedSet<Tree<E,C>>(subtrees);
            }

            if (_Internal.Count() < 2)
            {
                throw new ArgumentException("Parameter subtrees must have at least two item.");
            }
        }

        public IEnumerator<Tree<E,C>> GetEnumerator()
        {
            return _Internal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Internal).GetEnumerator();
        }
    }
}
