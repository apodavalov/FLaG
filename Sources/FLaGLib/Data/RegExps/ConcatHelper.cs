using FLaGLib.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    internal class ConcatHelper
    {
        public static Expression MakeExpression(ICollection<Expression> expressions)
        {
            if (expressions == null)
            {
                return null;
            }

            if (expressions.Count == 0)
            {
                return Empty.Instance;
            }
            else if (expressions.Count == 1)
            {
                return expressions.Single();
            }
            else
            {
                return new Concat(expressions);
            }
        }

        public static IEnumerable<Expression> Iterate(IEnumerable<Expression> expressions)
        {
            foreach (Expression expression in expressions)
            {
                foreach (Expression subExpression in Iterate(expression))
                {
                    yield return subExpression;
                }
            }
        }

        public static IEnumerable<Expression> Iterate(Expression expressionToIterate)
        {
            BinaryConcat binaryConcat = expressionToIterate.As<BinaryConcat>();

            if (binaryConcat != null)
            {
                foreach (Expression expression in Iterate(binaryConcat.Left.AsSequence().Concat(binaryConcat.Right)))
                {
                    yield return expression;
                }

                yield break;
            }

            Concat concat = expressionToIterate.As<Concat>();

            if (concat != null)
            {
                foreach (Expression expression in Iterate(concat.Expressions))
                {
                    yield return expression;
                }

                yield break;
            }

            yield return expressionToIterate;
        }

        public static void ToString(StringBuilder builder, IReadOnlyList<Expression> expressions, int priority)
        {
            bool first = true;

            foreach (Expression expression in expressions)
            {
                if (first)
                {
                    expression.ToString(expression.Priority > priority, builder);
                    first = false;
                }
                else
                {
                    expression.ToString(expression.Priority >= priority, builder);
                }
            }
        }
    }
}
