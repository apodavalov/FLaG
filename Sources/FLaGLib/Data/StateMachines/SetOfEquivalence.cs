using System.Collections.Immutable;
using FLaGLib.Extensions;

namespace FLaGLib.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class SetOfEquivalence(
        IEnumerable<Label> set,
        IEnumerable<SetOfEquivalenceTransition> transitions
    )
    {
        public IImmutableSet<Label> Set { get; } = set.ToImmutableSortedSet();

        public IReadOnlyList<SetOfEquivalenceTransition> Transitions { get; } =
            transitions.ToImmutableList();

        public SetOfEquivalence(IEnumerable<Label> set)
            : this(set, []) { }

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            foreach (Label label in Set)
            {
                hashCode.Add(label);
            }
            foreach (SetOfEquivalenceTransition transition in Transitions)
            {
                hashCode.Add(transition);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(SetOfEquivalence other) => Set.SequenceEqual(other.Set);

        public int CompareToNonnull(SetOfEquivalence other) => Set.SequenceCompare(other.Set);
    }
}
