using System.Collections.Immutable;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Grammars
{
    [ComparableEquatable]
    public sealed partial class NonTerminalSymbol(Label label) : Symbol
    {
        public Label Label { get; } = label;

        public override IEnumerable<TerminalSymbol> Alphabet => [];

        public override IEnumerable<NonTerminalSymbol> NonTerminals => [this];

        public bool EqualsNonnull(NonTerminalSymbol other) => Label.Equals(other.Label);

        public int CompareToNonnull(NonTerminalSymbol other) => Label.CompareTo(other.Label);

        public override int GetHashCode() => Label.GetHashCode();

        public override Symbol Reorganize(
            IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map
        ) => map.ValueOrThrow(this);

        public override SymbolType SymbolType => SymbolType.NonTerminal;

        public override string ToString() => Label.ToString();
    }
}
