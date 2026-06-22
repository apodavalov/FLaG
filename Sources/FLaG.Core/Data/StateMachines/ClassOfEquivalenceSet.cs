using System.Collections.Immutable;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class ClassOfEquivalenceSet(IEnumerable<ClassOfEquivalence> set)
    {
        public ImmutableSortedSet<ClassOfEquivalence> ClassOfEquivalences { get; } =
            set.ToImmutableSortedSet();

        public int FetchHashCode()
        {
            HashCode hashCode = new();
            foreach (ClassOfEquivalence classOfEquivalence in ClassOfEquivalences)
            {
                hashCode.Add(classOfEquivalence);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(ClassOfEquivalenceSet other) =>
            ClassOfEquivalences.SequenceEqual(other.ClassOfEquivalences);

        public int CompareToNonnull(ClassOfEquivalenceSet other) =>
            ClassOfEquivalences.SequenceCompare(other.ClassOfEquivalences);
    }
}
