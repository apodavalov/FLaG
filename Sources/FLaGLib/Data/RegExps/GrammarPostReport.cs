namespace FLaGLib.Data.RegExps
{
    public sealed record GrammarPostReport(
        GrammarExpressionTuple New,
        IReadOnlyList<GrammarExpressionWithOriginal> Dependencies
    ) { }
}
