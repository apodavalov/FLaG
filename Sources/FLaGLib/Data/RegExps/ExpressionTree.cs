using FLaGLib.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Data.RegExps
{
    public class ExpressionTree : Tree<Expression, ExpressionTreeCollection>
    {
        public ExpressionTree(Expression entry, ExpressionTreeCollection subtrees = null)
            : base(entry, subtrees)
        {

        }
    }
}
