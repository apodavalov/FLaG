using System.Collections.Immutable;
using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Extensions;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class Concat : Expression
    {
        public IImmutableList<Expression> Expressions { get; private set; }

        public Concat(IEnumerable<Expression> expressions)
        {
            Expressions = expressions.ToImmutableList();
            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }
        }

        public bool EqualsNonnull(Concat other)
        {
            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Expressions);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Expressions);

            return expression1.SequenceEqual(expression2);
        }

        public int CompareToNonnull(Concat other)
        {
            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Expressions);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Expressions);

            return expression1.SequenceCompare(expression2);
        }

        public override int FetchHashCode()
        {
            HashCode hashCode = new();
            foreach (Expression expression in Expressions)
            {
                hashCode.Add(expression);
            }
            return hashCode.ToHashCode();
        }

        public override int Priority => 2;

        public override ExpressionType ExpressionType => ExpressionType.Concat;

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (Expression expression in Expressions)
            {
                foreach (DepthData<Expression> data in expression.WalkInternal())
                {
                    yield return data;
                }
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        internal override void ToString(StringBuilder builder) =>
            ConcatHelper.ToString(builder, Expressions, Priority);

        internal override GrammarExpressionTuple GenerateGrammar(
            GrammarType grammarType,
            int grammarNumber,
            ref int index,
            ref int additionalGrammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        )
        {
            throw new NotSupportedException();
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(
            int stateMachineNumber,
            ref int index,
            ref int additionalStateMachineNumber,
            Action<StateMachinePostReport>? onIterate,
            params StateMachineExpressionWithOriginal[] dependencies
        )
        {
            throw new NotSupportedException();
        }

        public override Expression Optimize()
        {
            bool somethingChanged;

            Expression? previous;

            List<Expression> oldList = ConcatHelper
                .Iterate(Expressions.Select(e => e.Optimize()))
                .ToList();
            List<Expression> newList;

            do
            {
                somethingChanged = false;
                newList = [];

                previous = null;

                foreach (Expression expression in oldList)
                {
                    if (expression.ExpressionType == ExpressionType.Empty)
                    {
                        somethingChanged = true;
                        continue;
                    }

                    if (previous is null)
                    {
                        previous = expression;
                        continue;
                    }

                    Iteration? iteration1 = previous as Iteration;
                    Iteration? iteration2 = expression as Iteration;

                    if (iteration1 is not null || iteration2 is not null)
                    {
                        if (
                            iteration1 is not null
                            && iteration2 is not null
                            && iteration1.Expression.Equals(iteration2.Expression)
                        )
                        {
                            if (iteration1.IsPositive && iteration2.IsPositive)
                            {
                                newList.Add(iteration1.Expression);
                            }

                            previous = new Iteration(
                                iteration1.Expression,
                                iteration1.IsPositive || iteration2.IsPositive
                            );
                            somethingChanged = true;
                            continue;
                        }

                        Iteration iteration;
                        Expression newExpression;

                        if (iteration1 is not null)
                        {
                            iteration = iteration1;
                            newExpression = expression;
                        }
                        else
                        {
                            iteration = iteration2!;
                            newExpression = previous;
                        }

                        if (iteration.Expression.Equals(newExpression) && !iteration.IsPositive)
                        {
                            previous = new Iteration(newExpression, true);
                            somethingChanged = true;
                            continue;
                        }
                    }

                    ConstIteration? constIteration1 = previous as ConstIteration;
                    ConstIteration? constIteration2 = expression as ConstIteration;

                    if (constIteration1 is not null || constIteration2 is not null)
                    {
                        if (
                            constIteration1 is not null
                            && constIteration2 is not null
                            && constIteration1.Expression == constIteration2.Expression
                        )
                        {
                            previous = new ConstIteration(
                                constIteration1.Expression,
                                constIteration1.IterationCount + constIteration2.IterationCount
                            );
                            somethingChanged = true;
                            continue;
                        }

                        ConstIteration constIteration;
                        Expression newExpression;

                        if (constIteration1 is not null)
                        {
                            constIteration = constIteration1;
                            newExpression = expression;
                        }
                        else
                        {
                            constIteration = constIteration2!;
                            newExpression = previous;
                        }

                        if (constIteration.Expression == newExpression)
                        {
                            previous = new ConstIteration(
                                newExpression,
                                constIteration.IterationCount + 1
                            );
                            somethingChanged = true;
                            continue;
                        }
                    }

                    if (
                        constIteration1 is not null
                        && iteration2 is not null
                        && constIteration1.Expression.Equals(iteration2.Expression)
                    )
                    {
                        if (constIteration1.IterationCount > 2)
                        {
                            newList.Add(
                                new ConstIteration(
                                    constIteration1.Expression,
                                    constIteration1.IterationCount - 1
                                )
                            );
                        }
                        else
                        {
                            newList.Add(constIteration1.Expression);
                        }

                        previous = new Iteration(iteration2.Expression, true);
                        somethingChanged = true;
                        continue;
                    }

                    if (
                        iteration1 is not null
                        && constIteration2 is not null
                        && iteration1.Expression.Equals(constIteration2.Expression)
                    )
                    {
                        newList.Add(new Iteration(iteration1.Expression, true));

                        if (constIteration2.IterationCount > 2)
                        {
                            previous = new ConstIteration(
                                constIteration2.Expression,
                                constIteration2.IterationCount - 1
                            );
                        }
                        else
                        {
                            previous = constIteration2.Expression;
                        }

                        somethingChanged = true;
                        continue;
                    }

                    newList.Add(previous);
                    previous = expression;
                }

                if (previous is not null)
                {
                    newList.Add(previous);
                }

                oldList = newList;

                somethingChanged |= TryToCompress(oldList);
            } while (somethingChanged);

            return ConcatHelper.MakeExpression(newList);
        }

        private static bool TryToCompress(List<Expression> list)
        {
            bool changed;
            bool somethingChanged = false;

            do
            {
                changed = false;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] is not Iteration iteration || iteration.IsPositive)
                    {
                        continue;
                    }

                    List<Expression> list1;

                    if (iteration.Expression is Concat concat)
                    {
                        list1 = concat.Expressions.ToList();
                    }
                    else
                    {
                        list1 = [iteration.Expression];
                    }

                    int j = 0;

                    List<Expression>? list2 = null;

                    for (j = 0; j <= list1.Count; j++)
                    {
                        if (j > i || list1.Count - j > list.Count - i - 1)
                        {
                            continue;
                        }

                        list2 = [];

                        int k;

                        for (k = 0; k < j; k++)
                        {
                            list2.Add(list[i - j + k]);
                        }

                        for (k = 0; k < list1.Count - j; ++k)
                        {
                            list2.Add(list[i + 1 + k]);
                        }

                        bool allEquals = true;

                        for (int l = 0; l < list1.Count; ++l)
                        {
                            if (list1[l] != list2[(l + j) % list2.Count])
                            {
                                allEquals = false;
                                break;
                            }
                        }

                        if (allEquals)
                        {
                            break;
                        }
                    }

                    if (j <= list1.Count)
                    {
                        Expression newExpression = new Iteration(
                            ConcatHelper.MakeExpression(list2!),
                            true
                        );

                        for (int l = list1.Count - j; l > 0; --l)
                        {
                            list.RemoveAt(i + l);
                        }

                        for (int l = 0; l <= j; ++l)
                        {
                            list.RemoveAt(i - l);
                        }

                        i -= j + 1;

                        list.Insert(i + 1, newExpression);

                        changed = true;
                        somethingChanged = true;
                    }
                }
            } while (changed);

            return somethingChanged;
        }

        public override bool CanBeEmpty()
        {
            return Expressions.All(e => e.CanBeEmpty());
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new Concat(Expressions.Select(e => e.TryToLetItBeEmpty()).ToImmutableList());
        }
    }
}
