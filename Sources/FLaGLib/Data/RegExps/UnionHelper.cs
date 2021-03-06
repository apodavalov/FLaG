﻿using FLaGLib.Collections;
using FLaGLib.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public static class UnionHelper
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
                return new Union(expressions);
            }
        }

        public static IEnumerable<Expression> Iterate(ISet<Expression> visitedExpressions, IEnumerable<Expression> expressions)
        {
            foreach (Expression expression in expressions)
            {
                foreach (Expression subExpression in Iterate(visitedExpressions, expression))
                {
                    yield return subExpression;
                }
            }
        }

        public static IEnumerable<Expression> Iterate(ISet<Expression> visitedExpressions, Expression expressionToIterate)
        {
            BinaryUnion binaryUnion = expressionToIterate.As<BinaryUnion>();

            if (binaryUnion != null)
            {
                foreach (Expression expression in Iterate(visitedExpressions, binaryUnion.Left.AsSequence().Concat(binaryUnion.Right)))
                {
                    if (visitedExpressions.Add(expression))
                    {
                        yield return expression;
                    }
                }

                yield break;
            }

            Union union = expressionToIterate.As<Union>();

            if (union != null)
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

        public static void ToString(StringBuilder builder, IReadOnlySet<Expression> expressions, int priority)
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
