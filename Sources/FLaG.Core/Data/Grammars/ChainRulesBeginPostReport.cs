namespace FLaG.Core.Data.Grammars
{
    public sealed record ChainRulesBeginPostReport(
        int Iteration,
        IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> SymbolMap
    );
}
