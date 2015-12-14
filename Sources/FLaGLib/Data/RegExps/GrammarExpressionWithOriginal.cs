using System;

namespace FLaGLib.Data.RegExps
{
    public class GrammarExpressionWithOriginal
    {
        public GrammarExpressionTuple GrammarExpression
        {
            get;
            private set;
        }

        public GrammarExpressionTuple OriginalGrammarExpression
        {
            get;
            private set;
        }

        public GrammarExpressionWithOriginal(GrammarExpressionTuple grammarExpression, GrammarExpressionTuple originalGrammarExpression = null)
        {
            if (grammarExpression == null)
            {
                throw new ArgumentNullException(nameof(grammarExpression));
            }

            GrammarExpression = grammarExpression;
            OriginalGrammarExpression = originalGrammarExpression;
        }
    }
}
