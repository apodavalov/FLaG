using System.Collections.Immutable;
using System.Text;

namespace FLaG.Core.Data.RegExps
{
    public static class UnionHelper
    {
        public static Expression MakeExpression(IEnumerable<Expression> expressions)
        {
            using IEnumerator<Expression> enumerator = expressions.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new InvalidOperationException(
                    "Empty union makes an expression that doesn't accept any string."
                );
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

            return new Union(list.ToImmutable());
        }

        public static IEnumerable<Expression> Iterate(
            ISet<Expression> visitedExpressions,
            IEnumerable<Expression> expressions
        )
        {
            foreach (Expression expression in expressions)
            {
                foreach (Expression subExpression in Iterate(visitedExpressions, expression))
                {
                    yield return subExpression;
                }
            }
        }

        public static IEnumerable<Expression> Iterate(
            ISet<Expression> visitedExpressions,
            Expression expressionToIterate
        )
        {
            if (expressionToIterate is BinaryUnion binaryUnion)
            {
                foreach (
                    Expression expression in Iterate(
                        visitedExpressions,
                        [binaryUnion.Left, binaryUnion.Right]
                    )
                )
                {
                    if (visitedExpressions.Add(expression))
                    {
                        yield return expression;
                    }
                }

                yield break;
            }

            if (expressionToIterate is Union union)
            {
                foreach (Expression expression in Iterate(visitedExpressions, union.Expressions))
                {
                    if (visitedExpressions.Add(expression))
                    {
                        yield return expression;
                    }
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
                    builder.Append(" + ");
                    expression.ToString(expression.Priority >= priority, builder);
                }
            }
        }
    }
}
