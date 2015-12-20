using System;

namespace FLaGLib.Data.RegExps
{
    public class LanguageExpressionTuple
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public int LanguageNumber
        {
            get;
            private set;
        }

        public LanguageExpressionTuple(Expression expression, int languageNumber)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            Expression = expression;
            LanguageNumber = languageNumber;
        }
    }
}
