using System.Collections.Immutable;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class SetsOfEquivalence(IEnumerable<SetOfEquivalence> set)
    {
        public IImmutableList<SetOfEquivalence> SortedList { get; } = set.Order().ToImmutableList();

        public int FetchHashCode()
        {
            HashCode hashCode = new();
            foreach (SetOfEquivalence set in SortedList)
            {
                hashCode.Add(set);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(SetsOfEquivalence other) =>
            SortedList.SequenceEqual(other.SortedList);

        public int CompareToNonnull(SetsOfEquivalence other) =>
            SortedList.SequenceCompare(other.SortedList);
    }
}
