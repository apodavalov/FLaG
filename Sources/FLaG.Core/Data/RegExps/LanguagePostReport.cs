namespace FLaG.Core.Data.RegExps
{
    public sealed record LanguagePostReport(
        LanguageExpressionTuple New,
        IReadOnlyList<LanguageExpressionTuple> Dependencies
    ) { }
}
