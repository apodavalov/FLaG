using FLaGLib.Helpers;

namespace FLaGLib.Data.RegExps
{
    public sealed class ExpressionTreeCollection(
        IEnumerable<ExpressionTree> subtrees,
        TreeOperator @operator
    ) : TreeCollection<Expression, ExpressionTreeCollection>(subtrees, @operator) { }
}
