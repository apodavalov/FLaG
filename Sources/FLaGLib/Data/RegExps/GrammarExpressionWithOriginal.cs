namespace FLaGLib.Data.RegExps
{
    public sealed record GrammarExpressionWithOriginal(
        GrammarExpressionTuple GrammarExpression,
        GrammarExpressionTuple? OriginalGrammarExpression = null
    ) { }
}
