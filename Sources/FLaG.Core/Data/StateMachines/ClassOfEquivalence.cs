using System.Collections.Immutable;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data.StateMachines
{
    [ComparableEquatable]
    public sealed partial class ClassOfEquivalence(int setNum, IEnumerable<char> symbols)
    {
        public int SetNum { get; } = setNum;

        public IImmutableSet<char> Symbols { get; } = symbols.ToImmutableSortedSet();

        public int FetchHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(SetNum);
            foreach (char symbol in Symbols)
            {
                hashCode.Add(symbol);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(ClassOfEquivalence other)
        {
            if (!SetNum.Equals(other.SetNum))
            {
                return false;
            }

            return Symbols.SequenceEqual(other.Symbols);
        }

        public int CompareToNonnull(ClassOfEquivalence other)
        {
            int result = SetNum.CompareTo(other.SetNum);

            if (result != 0)
            {
                return result;
            }

            return Symbols.SequenceCompare(other.Symbols);
        }
    }
}
