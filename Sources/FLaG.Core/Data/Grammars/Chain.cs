using System.Collections;
using System.Collections.Immutable;
using FLaG.Core.Extensions;

namespace FLaG.Core.Data.Grammars
{
    [ComparableEquatable]
    public sealed partial class Chain(IEnumerable<Symbol> sequence) : IEnumerable<Symbol>
    {
        public static Chain Empty => new([]);

        public IImmutableList<Symbol> Sequence { get; } = sequence.ToImmutableList();

        public IImmutableSet<TerminalSymbol> Alphabet { get; } =
            sequence.SelectMany(symbol => symbol.Alphabet).ToImmutableSortedSet();

        public IImmutableSet<NonTerminalSymbol> NonTerminals { get; } =
            sequence.SelectMany(symbol => symbol.NonTerminals).ToImmutableSortedSet();

        public Chain Reorganize(IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map) =>
            new(Sequence.Select(symbol => symbol.Reorganize(map)));

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            foreach (Symbol symbol in Sequence)
            {
                hashCode.Add(symbol);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(Chain other) => Sequence.SequenceEqual(other.Sequence);

        public int CompareToNonnull(Chain other) => Sequence.SequenceCompare(other.Sequence);

        public Symbol this[int index] => Sequence[index];

        public int Count => Sequence.Count;

        public IEnumerator<Symbol> GetEnumerator() => Sequence.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Sequence.GetEnumerator();

        public override string ToString()
        {
            if (Sequence.Count > 0)
            {
                return string.Join(string.Empty, Sequence);
            }
            else
            {
                return "ε";
            }
        }
    }
}
