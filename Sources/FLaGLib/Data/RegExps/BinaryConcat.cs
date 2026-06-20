using System.Collections.Immutable;
using System.Text;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class BinaryConcat(Expression left, Expression right) : Expression
    {
        public Expression Left { get; } = left;

        public Expression Right { get; } = right;

        public bool EqualsNonnull(BinaryConcat other)
        {
            IEnumerable<Expression> expression1 = ConcatHelper.Iterate([Left, Right]);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate([other.Left, other.Right]);

            return expression1.SequenceEqual(expression2);
        }

        public int CompareToNonnull(BinaryConcat other)
        {
            IEnumerable<Expression> expression1 = ConcatHelper.Iterate([Left, Right]);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate([other.Left, other.Right]);

            return expression1.SequenceCompare(expression2);
        }

        public override int GetHashCode() => HashCode.Combine(Left, Right);

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (DepthData<Expression> data in Left.WalkInternal())
            {
                yield return data;
            }

            foreach (DepthData<Expression> data in Right.WalkInternal())
            {
                yield return data;
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority => 2;

        public override ExpressionType ExpressionType => ExpressionType.BinaryConcat;

        internal override void ToString(StringBuilder builder) =>
            ConcatHelper.ToString(builder, ConcatHelper.Iterate([Left, Right]), Priority);

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
            return GrammarDispatcher.Dispatch(
                grammarType,
                () => GenerateLeftGrammar(grammarNumber, onIterate, dependencies),
                () => GenerateRightGrammar(grammarNumber, onIterate, dependencies)
            );
        }

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 2);
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

            StateMachine stateMachine1 = dependencies[0].StateMachineExpression.StateMachine;
            StateMachine stateMachine2 = dependencies[1].StateMachineExpression.StateMachine;

            Label initialState = stateMachine1.InitialState;

            HashSet<Label> finalStates = stateMachine2.FinalStates.ToHashSet();

            if (stateMachine2.FinalStates.Contains(stateMachine2.InitialState))
            {
                finalStates.UnionWith(stateMachine1.FinalStates);
            }

            HashSet<Transition> transitions = stateMachine1.Transitions.ToHashSet();

            foreach (Transition transition in stateMachine2.Transitions)
            {
                if (transition.CurrentState == stateMachine2.InitialState)
                {
                    foreach (Label state in stateMachine1.FinalStates)
                    {
                        transitions.Add(new(state, transition.Symbol, transition.NextState));
                    }
                }

                transitions.Add(transition);
            }

            IEnumerable<Label> states = stateMachine1.States.Concat(stateMachine2.States);

            StateMachineExpressionTuple stateMachineExpressionTuple = new(
                this,
                new StateMachine(initialState, finalStates, transitions, states),
                stateMachineNumber
            );

            onIterate?.Invoke(
                new StateMachinePostReport(stateMachineExpressionTuple, dependencies)
            );

            return stateMachineExpressionTuple;
        }

        private GrammarExpressionTuple GenerateRightGrammar(
            int grammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        )
        {
            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

            leftExpGrammar.SplitRules(
                out ImmutableSortedSet<Rule> terminalSymbolsOnlyRules,
                out ImmutableSortedSet<Rule> otherRules
            );
            HashSet<Rule> newRules = otherRules.Concat(rightExpGrammar.Rules).ToHashSet();

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                HashSet<Chain> newChains = rule
                    .Chains.Select(chain => new Chain(chain.Concat([rightExpGrammar.Target])))
                    .ToHashSet();
                newRules.Add(new Rule(newChains, rule.Target));
            }

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new Grammar(newRules, leftExpGrammar.Target),
                grammarNumber
            );
            onIterate?.Invoke(new GrammarPostReport(grammarExpressionTuple, dependencies));

            return grammarExpressionTuple;
        }

        private GrammarExpressionTuple GenerateLeftGrammar(
            int grammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        )
        {
            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

            rightExpGrammar.SplitRules(
                out ImmutableSortedSet<Rule> terminalSymbolsOnlyRules,
                out ImmutableSortedSet<Rule> otherRules
            );

            HashSet<Rule> newRules = otherRules.Concat(leftExpGrammar.Rules).ToHashSet();

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                HashSet<Chain> newChains = rule
                    .Chains.Select(chain => new Chain(chain.Prepend(leftExpGrammar.Target)))
                    .ToHashSet();
                newRules.Add(new Rule(newChains, rule.Target));
            }

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new Grammar(newRules, rightExpGrammar.Target),
                grammarNumber
            );

            onIterate?.Invoke(new GrammarPostReport(grammarExpressionTuple, dependencies));

            return grammarExpressionTuple;
        }

        public override Expression Optimize()
        {
            return new Concat(ConcatHelper.Iterate([Left, Right]).ToImmutableList()).Optimize();
        }

        public override bool CanBeEmpty()
        {
            return Left.CanBeEmpty() && Right.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new BinaryConcat(Left.TryToLetItBeEmpty(), Right.TryToLetItBeEmpty());
        }
    }
}
