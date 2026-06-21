namespace FLaG.Core.Data.Grammars
{
    public sealed record ChainRulesEndTuple(
        IReadOnlySet<NonTerminalSymbol> NonTerminals,
        IReadOnlySet<NonTerminalSymbol> FinalNonTerminals,
        int Iteration
    );
}
