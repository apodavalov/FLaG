using FLaGLib.Helpers;

namespace FLaGLib.Data.RegExps
{
    public sealed class ExpressionTree(Expression entry, ExpressionTreeCollection? subtrees = null)
        : Tree<Expression, ExpressionTreeCollection>(entry, subtrees) { }
}
