using System.Collections.Immutable;

namespace FLaG.Core.Helpers
{
    public class TreeCollection<E, C> : IEnumerable<Tree<E, C>>
        where C : TreeCollection<E, C>
    {
        protected IEnumerable<Tree<E, C>> _Internal;

        public TreeOperator Operator { get; }

        public TreeCollection(IEnumerable<Tree<E, C>> subtrees, TreeOperator @operator)
        {
            Operator = @operator;
            _Internal = Operator switch
            {
                TreeOperator.Concat => subtrees.ToImmutableList(),
                TreeOperator.Union => subtrees.ToImmutableSortedSet(),
                _ => throw new InvalidOperationException(
                    string.Format("Tree operator {0} is not supported.", Operator)
                ),
            };

            if (_Internal.Count() < 2)
            {
                throw new ArgumentException("Parameter subtrees must have at least two item.");
            }
        }

        public IEnumerator<Tree<E, C>> GetEnumerator() => _Internal.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            _Internal.GetEnumerator();
    }
}
