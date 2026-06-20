using System.Collections.Immutable;
using System.Text;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class BinaryUnion(Expression left, Expression right) : Expression
    {
        public Expression Left { get; } = left;

        public Expression Right { get; } = right;

        public bool EqualsNonnull(BinaryUnion other)
        {
            return GetOrderedSet(this).SequenceEqual(GetOrderedSet(other));
        }

        public int CompareToNonnull(BinaryUnion other)
        {
            return GetOrderedSet(this).SequenceCompare(GetOrderedSet(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Right);
        }

        private static ImmutableSortedSet<Expression> GetOrderedSet(BinaryUnion binaryUnion)
        {
            HashSet<Expression> visitedExpressions = [];
            return UnionHelper
                .Iterate(visitedExpressions, [binaryUnion.Left, binaryUnion.Right])
                .ToImmutableSortedSet();
        }

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

        public override int Priority => 3;

        public override ExpressionType ExpressionType => ExpressionType.BinaryUnion;

        internal override void ToString(StringBuilder builder)
        {
            HashSet<Expression> visitedExpressions = [];
            UnionHelper.ToString(
                builder,
                UnionHelper.Iterate(visitedExpressions, [Left, Right]),
                Priority
            );
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

            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

            NonTerminalSymbol target = new(
                new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++))
            );

            Rule rule = new(
                [new Chain([leftExpGrammar.Target]), new Chain([rightExpGrammar.Target])],
                target
            );

            IEnumerable<Rule> newRules = leftExpGrammar
                .Rules.Concat(rightExpGrammar.Rules)
                .Append(rule);

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new Grammar(newRules, target),
                grammarNumber
            );

            onIterate?.Invoke(new GrammarPostReport(grammarExpressionTuple, dependencies));

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

            StateMachine stateMachine1 = dependencies[0].StateMachineExpression.StateMachine;
            StateMachine stateMachine2 = dependencies[1].StateMachineExpression.StateMachine;

            Label initialState = new(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            HashSet<Label> finalStates = [];

            HashSet<Transition> transitions = [];

            if (
                stateMachine1.FinalStates.Contains(stateMachine1.InitialState)
                || stateMachine2.FinalStates.Contains(stateMachine2.InitialState)
            )
            {
                finalStates.Add(initialState);
            }

            finalStates.UnionWith(stateMachine1.FinalStates);
            finalStates.UnionWith(stateMachine2.FinalStates);

            transitions.UnionWith(stateMachine1.Transitions);
            transitions.UnionWith(stateMachine2.Transitions);

            transitions.UnionWith(
                stateMachine1
                    .Transitions.Where(t => t.CurrentState == stateMachine1.InitialState)
                    .Select(t => new Transition(initialState, t.Symbol, t.NextState))
            );
            transitions.UnionWith(
                stateMachine2
                    .Transitions.Where(t => t.CurrentState == stateMachine2.InitialState)
                    .Select(t => new Transition(initialState, t.Symbol, t.NextState))
            );

            IEnumerable<Label> states = stateMachine1
                .States.Concat(stateMachine2.States)
                .Append(initialState);

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
            CheckDependencies(dependencies, 2);
        }

        public override Expression Optimize()
        {
            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            return new Union(UnionHelper.Iterate(visitedExpressions, [Left, Right])).Optimize();
        }

        public override bool CanBeEmpty()
        {
            return Left.CanBeEmpty() || Right.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new BinaryUnion(Left.TryToLetItBeEmpty(), Right.TryToLetItBeEmpty());
        }
    }
}
