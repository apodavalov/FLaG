using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Languages
{
    public class TreeCollection : ReadOnlyList<Tree>
    {
        public TreeOperator Operator
        {
            get;
            private set;
        }

        public TreeCollection(IList<Tree> subtrees, TreeOperator @operator)
            : base(subtrees)
        {
            if (!subtrees.Any())
            {
                throw new ArgumentException("Parameter subtrees must have at least one item.");
            }

            Operator = @operator;
        }
    }
}
