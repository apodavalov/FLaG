using FLaGLib.Extensions;
using System.Collections.Generic;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    internal class ConcatHelper
    {
        public static IEnumerable<Expression> Iterate(Expression expressionToIterate)
        {
            BinaryConcat binaryConcat = expressionToIterate.As<BinaryConcat>();

            if (binaryConcat != null)
            {
                foreach (Expression expression in binaryConcat.Iterate())
                {
                    yield return expression;
                }

                yield break;
            }

            Concat concat = expressionToIterate.As<Concat>();

            if (concat != null)
            {
                foreach (Expression expression in concat.Iterate())
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
