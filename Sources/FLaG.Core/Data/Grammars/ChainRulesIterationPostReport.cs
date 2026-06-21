namespace FLaG.Core.Data.Grammars
{
    public sealed record ChainRulesIterationPostReport(
        int Iteration,
        IReadOnlyDictionary<NonTerminalSymbol, ChainRulesIterationTuple> SymbolMap,
        bool IsLastIteration
    );
}
