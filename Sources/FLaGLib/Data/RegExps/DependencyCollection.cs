using FLaGLib.Collections;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.RegExps
{
    public class DependencyCollection : ReadOnlySet<int>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        internal DependencyCollection(Expression expression, ISet<int> dependencyIndices) : base(dependencyIndices)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            Expression = expression;
        }
    }
}
