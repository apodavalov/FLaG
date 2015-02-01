using FLaGLib.Data.Helpers;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.RegExps
{
    public class ExpressionTreeCollection : TreeCollection<Expression, ExpressionTreeCollection>
    {
        public ExpressionTreeCollection(IEnumerable<ExpressionTree> subtrees, TreeOperator @operator)
            : base(subtrees, @operator)
        {

        }
    }
}
