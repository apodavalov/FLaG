using System.Collections.Immutable;
using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class Iteration(Expression expression, bool isPositive) : Expression
    {
        public Expression Expression { get; } = expression;

        public bool IsPositive { get; } = isPositive;

        public bool EqualsNonnull(Iteration other)
        {
            return Expression.Equals(other.Expression) && IsPositive.Equals(other.IsPositive);
        }

        public int CompareToNonnull(Iteration other)
        {
            int result = Expression.CompareToNonnull(other.Expression);

            if (result != 0)
            {
                return result;
            }

            return IsPositive.CompareTo(other.IsPositive);
        }

        public override int GetHashCode() => HashCode.Combine(Expression, IsPositive);

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

        public override ExpressionType ExpressionType => ExpressionType.Iteration;

        internal override void ToString(StringBuilder builder)
        {
            Expression.ToString(Expression.Priority > Priority, builder);

            builder.Append('^');

            if (IsPositive)
            {
                builder.Append("(+)");
            }
            else
            {
                builder.Append("(*)");
            }
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
            Grammar expGrammar = dependencies[0].GrammarExpression.Grammar;

            Func<Chain, NonTerminalSymbol, IEnumerable<Chain>> chainEnumerator =
                GrammarDispatcher.Dispatch(grammarType, LeftChainEnumerator, RightChainEnumerator);

            expGrammar.SplitRules(
                out ImmutableSortedSet<Rule> terminalSymbolsOnlyRules,
                out ImmutableSortedSet<Rule> otherRules
            );

            HashSet<Rule> newRules = [];

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                HashSet<Chain> newChains = rule
                    .Chains.SelectMany(chain => chainEnumerator(chain, expGrammar.Target))
                    .ToHashSet();
                newRules.Add(new Rule(newChains, rule.Target));
            }

            NonTerminalSymbol symbol = new(
                new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++))
            );

            IEnumerable<Chain> chains = [new Chain([expGrammar.Target])];

            if (!IsPositive)
            {
                chains = chains.Append(new Chain([]));
            }

            newRules.Add(new Rule(chains, symbol));

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new Grammar(newRules, symbol),
                grammarNumber
            );

            onIterate?.Invoke(
                new GrammarPostReport(grammarExpressionTuple, dependencies.ToImmutableList())
            );

            return grammarExpressionTuple;
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

            StateMachine original = dependencies[0].StateMachineExpression.StateMachine;

            Label initialState = new(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            HashSet<Transition> transitions = original.Transitions.ToHashSet();

            HashSet<Label> finalStates = original.FinalStates.ToHashSet();

            if (!IsPositive)
            {
                finalStates.Add(initialState);
            }

            foreach (Transition transition in original.Transitions)
            {
                if (transition.CurrentState == original.InitialState)
                {
                    transitions.UnionWith(
                        original.FinalStates.Select(fs => new Transition(
                            fs,
                            transition.Symbol,
                            transition.NextState
                        ))
                    );
                    transitions.Add(
                        new Transition(initialState, transition.Symbol, transition.NextState)
                    );
                }
            }

            IEnumerable<Label> states = original.States.Append(initialState);

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

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 1);
        }

        private static IEnumerable<Chain> LeftChainEnumerator(
            Chain chain,
            NonTerminalSymbol target
        ) => [new Chain(chain.Prepend(target)), chain];

        private static IEnumerable<Chain> RightChainEnumerator(
            Chain chain,
            NonTerminalSymbol target
        ) => [new Chain(chain.Append(target)), chain];

        public override Expression Optimize()
        {
            Expression expression = Expression.Optimize();

            if (expression is Iteration iteration)
            {
                return new Iteration(
                    iteration.Expression,
                    !iteration.Expression.CanBeEmpty() && IsPositive && iteration.IsPositive
                );
            }

            if (expression is Empty empty)
            {
                return empty;
            }

            if (expression.CanBeEmpty() && IsPositive)
            {
                return new Iteration(expression, false);
            }

            return this;
        }

        public override bool CanBeEmpty()
        {
            if (!IsPositive)
            {
                return true;
            }

            return Expression.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty() =>
            new Iteration(Expression.TryToLetItBeEmpty(), false);
    }
}
