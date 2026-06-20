using System.Collections.Immutable;

namespace FLaGLib.Data.Grammars
{
    [ComparableEquatable]
    public abstract partial class Symbol
    {
        public abstract IEnumerable<TerminalSymbol> Alphabet { get; }

        public abstract IEnumerable<NonTerminalSymbol> NonTerminals { get; }

        public abstract Symbol Reorganize(
            IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map
        );

        public abstract SymbolType SymbolType { get; }

        public abstract override int GetHashCode();

        public virtual int CompareToNonnull(Symbol other) => SymbolType.CompareTo(other.SymbolType);

        public virtual bool EqualsNonnull(Symbol other) => SymbolType.Equals(other.SymbolType);
    }
}
