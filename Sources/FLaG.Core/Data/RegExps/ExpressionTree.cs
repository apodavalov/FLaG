using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    public sealed class ExpressionTree(Expression entry, ExpressionTreeCollection? subtrees = null)
        : Tree<Expression, ExpressionTreeCollection>(entry, subtrees) { }
}
