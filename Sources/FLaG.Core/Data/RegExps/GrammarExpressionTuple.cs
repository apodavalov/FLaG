using FLaG.Core.Data.Grammars;

namespace FLaG.Core.Data.RegExps
{
    public sealed record GrammarExpressionTuple(
        Expression Expression,
        Grammar Grammar,
        int Number
    ) { }
}
