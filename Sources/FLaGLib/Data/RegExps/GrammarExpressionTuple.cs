using FLaGLib.Data.Grammars;

namespace FLaGLib.Data.RegExps
{
    public sealed record GrammarExpressionTuple(
        Expression Expression,
        Grammar Grammar,
        int Number
    ) { }
}
