using System.Collections.Immutable;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Grammars
{
    [ComparableEquatable]
    public sealed partial class Rule
    {
        public IImmutableSet<Chain> Chains { get; }

        public NonTerminalSymbol Target { get; }

        public IImmutableSet<TerminalSymbol> Alphabet { get; }

        public IImmutableSet<NonTerminalSymbol> NonTerminals { get; }

        public Rule(IEnumerable<Chain> chains, NonTerminalSymbol target)
        {
            Chains = chains.ToImmutableSortedSet();
            Alphabet = Chains.SelectMany(chain => chain.Alphabet).ToImmutableSortedSet();
            Target = target;
            NonTerminals = Chains
                .SelectMany(chain => chain.NonTerminals)
                .Concat([Target])
                .ToImmutableSortedSet();
        }

        public Rule Reorganize(IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map) =>
            new(Chains.Select(chain => chain.Reorganize(map)), map.ValueOrThrow(Target));

        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(Target);
            foreach (Chain chain in Chains)
            {
                hashCode.Add(chain);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(Rule other) =>
            Target.Equals(other.Target) && Chains.SequenceEqual(other.Chains);

        public int CompareToNonnull(Rule other)
        {
            int result = Target.CompareTo(other.Target);

            if (result != 0)
            {
                return result;
            }

            return Chains.SequenceCompare(other.Chains);
        }

        public override string ToString() =>
            string.Format("{0} -> {1}", Target, string.Join("|", Chains));
    }
}
