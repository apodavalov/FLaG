using System.Collections;
using System.Collections.Immutable;

namespace FLaGLib.Data.RegExps
{
    public sealed class DependencyCollection(
        Expression expression,
        ImmutableList<int> dependencyIndices
    ) : IEnumerable<int>
    {
        public Expression Expression { get; } = expression;

        public ImmutableList<int> DependencyIndices { get; } = dependencyIndices;

        public IEnumerator<int> GetEnumerator()
        {
            return DependencyIndices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
