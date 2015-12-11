using FLaGLib.Data.Grammars;
using System;

namespace FLaGLib.Data.RegExps
{
    public class GrammarExpressionTuple
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public Grammar Grammar
        {
            get;
            private set;
        }

        public int Number
        {
            get;
            private set;
        }

        public GrammarExpressionTuple(Expression expression, Grammar grammar, int number)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (grammar == null)
            {
                throw new ArgumentNullException(nameof(grammar));
            }

            Expression = expression;
            Grammar = grammar;
            Number = number;
        }
    }
}
