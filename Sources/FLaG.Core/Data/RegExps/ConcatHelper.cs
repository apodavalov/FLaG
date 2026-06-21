using System.Collections.Immutable;
using System.Text;

namespace FLaG.Core.Data.RegExps
{
    public static class ConcatHelper
    {
        public static Expression MakeExpression(IEnumerable<Expression> expressions)
        {
            using IEnumerator<Expression> enumerator = expressions.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return new Empty();
            }
            Expression first = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                return first;
            }
            ImmutableList<Expression>.Builder list = ImmutableList.CreateBuilder<Expression>();
            list.Add(first);
            do
            {
                list.Add(enumerator.Current);
            } while (enumerator.MoveNext());

            return new Concat(list.ToImmutable());
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
            if (expressionToIterate is BinaryConcat binaryConcat)
            {
                foreach (Expression expression in Iterate([binaryConcat.Left, binaryConcat.Right]))
                {
                    yield return expression;
                }

                yield break;
            }

            if (expressionToIterate is Concat concat)
            {
                foreach (Expression expression in Iterate(concat.Expressions))
                {
                    yield return expression;
                }

                yield break;
            }

            yield return expressionToIterate;
        }

        public static void ToString(
            StringBuilder builder,
            IEnumerable<Expression> expressions,
            int priority
        )
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
