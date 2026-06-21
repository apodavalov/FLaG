using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    public sealed class ExpressionTreeCollection(
        IEnumerable<ExpressionTree> subtrees,
        TreeOperator @operator
    ) : TreeCollection<Expression, ExpressionTreeCollection>(subtrees, @operator) { }
}
