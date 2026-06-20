using System.Collections.Immutable;

namespace FLaGLib.Data.StateMachines
{
    public sealed class SetOfEquivalenceTransition(
        IEnumerable<char> symbols,
        int indexOfCurrentSetOfEquivalence
    )
    {
        public IImmutableSet<char> Symbols { get; } = symbols.ToImmutableSortedSet();

        public int IndexOfCurrentSetOfEquivalence { get; } = indexOfCurrentSetOfEquivalence;
    }
}
