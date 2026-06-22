using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class ConstIteration : Expression
    {
        public Expression Expression { get; }

        public int IterationCount { get; }

        public ConstIteration(Expression expression, int iterationCount)
        {
            if (iterationCount < 0)
            {
                throw new ArgumentException("Count must not be less than zero.");
            }

            Expression = expression;
            IterationCount = iterationCount;
        }

        public bool EqualsNonnull(ConstIteration other)
        {
            return Expression.EqualsNonnull(other.Expression)
                && IterationCount.Equals(other.IterationCount);
        }

        public int CompareToNonnull(ConstIteration other)
        {
            int result = Expression.CompareToNonnull(other.Expression);

            if (result != 0)
            {
                return result;
            }

            return IterationCount.CompareTo(other.IterationCount);
        }

        public override int FetchHashCode() => HashCode.Combine(Expression, IterationCount);

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (DepthData<Expression> data in Expression.WalkInternal())
            {
                yield return data;
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority => 1;

        public override ExpressionType ExpressionType => ExpressionType.ConstIteration;

        internal override void ToString(StringBuilder builder)
        {
            Expression.ToString(Expression.Priority > Priority, builder);
            builder.Append("^(");
            builder.Append(IterationCount);
            builder.Append(')');
        }

        internal override GrammarExpressionTuple GenerateGrammar(
            GrammarType grammarType,
            int grammarNumber,
            ref int index,
            ref int additionalGrammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        )
        {
            CheckDependencies(dependencies);

            if (IterationCount == 0)
            {
                return new Empty().GenerateGrammar(
                    grammarType,
                    grammarNumber,
                    ref index,
                    ref additionalGrammarNumber,
                    onIterate
                );
            }

            GrammarExpressionTuple original = dependencies[0].GrammarExpression;

            if (IterationCount == 1)
            {
                GrammarExpressionTuple grammarExpressionTuple = new(
                    this,
                    original.Grammar,
                    grammarNumber
                );

                onIterate?.Invoke(
                    new GrammarPostReport(
                        grammarExpressionTuple,
                        [new GrammarExpressionWithOriginal(original)]
                    )
                );

                return grammarExpressionTuple;
            }

            GrammarExpressionTuple dependency1 = original;

            for (int i = 1; i < IterationCount; ++i)
            {
                GrammarExpressionTuple dependency2 = Mirror(
                    original,
                    ref index,
                    ref additionalGrammarNumber
                );

                int number;

                if (i == IterationCount - 1)
                {
                    number = grammarNumber;
                }
                else
                {
                    number = additionalGrammarNumber++;
                }

                GrammarExpressionTuple expressionTuple = new BinaryConcat(
                    dependency1.Expression,
                    dependency2.Expression
                ).GenerateGrammar(
                    grammarType,
                    number,
                    ref index,
                    ref additionalGrammarNumber,
                    onIterate,
                    new GrammarExpressionWithOriginal(dependency1),
                    new GrammarExpressionWithOriginal(dependency2, original)
                );

                dependency1 = expressionTuple;
            }

            return dependency1;
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(
            int stateMachineNumber,
            ref int index,
            ref int additionalStateMachineNumber,
            Action<StateMachinePostReport>? onIterate,
            params StateMachineExpressionWithOriginal[] dependencies
        )
        {
            CheckDependencies(dependencies);

            if (IterationCount == 0)
            {
                return new Empty().GenerateStateMachine(
                    stateMachineNumber,
                    ref index,
                    ref additionalStateMachineNumber,
                    onIterate
                );
            }

            StateMachineExpressionTuple original = dependencies[0].StateMachineExpression;

            if (IterationCount == 1)
            {
                StateMachineExpressionTuple stateMachineExpressionTuple = new(
                    this,
                    original.StateMachine,
                    stateMachineNumber
                );

                onIterate?.Invoke(
                    new StateMachinePostReport(
                        stateMachineExpressionTuple,
                        [new StateMachineExpressionWithOriginal(original)]
                    )
                );

                return stateMachineExpressionTuple;
            }

            StateMachineExpressionTuple dependency1 = original;

            for (int i = 1; i < IterationCount; i++)
            {
                StateMachineExpressionTuple dependency2 = Mirror(
                    original,
                    ref index,
                    ref additionalStateMachineNumber
                );

                int number;

                if (i == IterationCount - 1)
                {
                    number = stateMachineNumber;
                }
                else
                {
                    number = additionalStateMachineNumber++;
                }

                StateMachineExpressionTuple expressionTuple = new BinaryConcat(
                    dependency1.Expression,
                    dependency2.Expression
                ).GenerateStateMachine(
                    number,
                    ref index,
                    ref additionalStateMachineNumber,
                    onIterate,
                    new StateMachineExpressionWithOriginal(dependency1),
                    new StateMachineExpressionWithOriginal(dependency2, original)
                );

                dependency1 = expressionTuple;
            }

            return dependency1;
        }

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 1);
        }

        private StateMachineExpressionTuple Mirror(
            StateMachineExpressionTuple stateMachine,
            ref int index,
            ref int additionalStateMachineNumber
        )
        {
            StateMachine newStateMachine = stateMachine.StateMachine.Reorganize(index);
            index += newStateMachine.States.Count;

            return new StateMachineExpressionTuple(
                stateMachine.Expression,
                newStateMachine,
                additionalStateMachineNumber++
            );
        }

        private static GrammarExpressionTuple Mirror(
            GrammarExpressionTuple grammar,
            ref int index,
            ref int additionalGrammarNumber
        )
        {
            Grammar newGrammar = grammar.Grammar.Reorganize(index);
            index += newGrammar.NonTerminals.Count;

            return new GrammarExpressionTuple(
                grammar.Expression,
                newGrammar,
                additionalGrammarNumber++
            );
        }

        public override Expression Optimize()
        {
            switch (IterationCount)
            {
                case 0:
                    return new Empty();
                case 1:
                    return Expression.Optimize();
                default:
                    Expression expression = Expression.Optimize();
                    if (expression is ConstIteration constIteration)
                    {
                        return new ConstIteration(
                            constIteration.Expression,
                            constIteration.IterationCount * IterationCount
                        );
                    }

                    if (expression is Iteration iteration && !iteration.IsPositive)
                    {
                        return iteration;
                    }

                    return this;
            }
        }

        public override bool CanBeEmpty()
        {
            if (IterationCount == 0)
            {
                return true;
            }

            return Expression.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new ConstIteration(Expression.TryToLetItBeEmpty(), IterationCount);
        }
    }
}
