namespace FLaGLib.Data.Grammars
{
    public sealed record ChainRulesBeginPostReport(
        int Iteration,
        IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> SymbolMap
    );
}
