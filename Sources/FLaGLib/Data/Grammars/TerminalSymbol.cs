using System.Collections.Immutable;

namespace FLaGLib.Data.Grammars
{
    [ComparableEquatable]
    public sealed partial class TerminalSymbol(char symbol) : Symbol
    {
        public char Symbol { get; private set; } = symbol;

        public override IEnumerable<TerminalSymbol> Alphabet => [this];

        public override IEnumerable<NonTerminalSymbol> NonTerminals => [];

        public bool EqualsNonnull(TerminalSymbol other) => Symbol.Equals(other.Symbol);

        public int CompareToNonnull(TerminalSymbol other) => Symbol.CompareTo(other.Symbol);

        public override int GetHashCode() => Symbol.GetHashCode();

        public override Symbol Reorganize(
            IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map
        )
        {
            return this;
        }

        public override SymbolType SymbolType => SymbolType.Terminal;

        public override string ToString() => Symbol.ToString();
    }
}
