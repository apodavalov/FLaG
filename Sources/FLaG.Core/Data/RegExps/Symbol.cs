using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class Symbol(char character) : Expression
    {
        public char Character { get; } = character;

        public bool EqualsNonnull(Symbol other) => Character.Equals(other.Character);

        public int CompareToNonnull(Symbol other) => Character.CompareTo(other.Character);

        public override int FetchHashCode() => Character.GetHashCode();

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);
            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority => 0;

        public override ExpressionType ExpressionType => ExpressionType.Symbol;

        internal override void ToString(StringBuilder builder)
        {
            builder.Append(Character);
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

            NonTerminalSymbol target = new(
                new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++))
            );
            TerminalSymbol symbol = new(Character);

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new Grammar([new Rule([new Chain([symbol])], target)], target),
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

            Label currentState = new(new SingleLabel(StateMachine._DefaultStateSymbol, index++));
            Label nextState = new(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            StateMachineExpressionTuple stateMachineExpressionTuple = new(
                this,
                new StateMachine(
                    currentState,
                    [nextState],
                    [new Transition(currentState, Character, nextState)]
                ),
                stateMachineNumber
            );

            onIterate?.Invoke(
                new StateMachinePostReport(stateMachineExpressionTuple, dependencies)
            );

            return stateMachineExpressionTuple;
        }

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 0);
        }

        public override Expression Optimize() => this;

        public override bool CanBeEmpty() => false;

        public override Expression TryToLetItBeEmpty() => this;
    }
}
