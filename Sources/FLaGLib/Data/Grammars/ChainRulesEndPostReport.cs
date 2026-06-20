namespace FLaGLib.Data.Grammars
{
    public sealed record ChainRulesEndPostReport(
        IReadOnlyDictionary<NonTerminalSymbol, ChainRulesEndTuple> SymbolMap
    );
}
