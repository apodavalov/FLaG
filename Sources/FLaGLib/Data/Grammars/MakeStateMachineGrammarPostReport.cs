namespace FLaGLib.Data.Grammars
{
    public sealed record MakeStateMachineGrammarPostReport(
        NonTerminalSymbol Target,
        Chain Chain,
        IReadOnlySet<Rule> PreviousRules,
        IReadOnlySet<Rule> NewRules,
        IReadOnlySet<Rule> NextRules,
        bool Converted
    );
}
