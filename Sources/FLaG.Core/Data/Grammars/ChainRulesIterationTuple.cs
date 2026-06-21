namespace FLaG.Core.Data.Grammars
{
    public sealed record ChainRulesIterationTuple(
        bool IsLastIteration,
        IReadOnlySet<NonTerminalSymbol> Previous,
        IReadOnlySet<NonTerminalSymbol> New,
        IReadOnlySet<NonTerminalSymbol> Next
    );
}
