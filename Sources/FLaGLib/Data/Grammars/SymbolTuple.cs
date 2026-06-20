namespace FLaGLib.Data.Grammars
{
    internal sealed record SymbolTuple(
        NonTerminalSymbol Target,
        TerminalSymbol? TerminalSymbol,
        NonTerminalSymbol? NonTerminalSymbol
    );
}
