using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class Empty : Expression
    {
        public bool EqualsNonnull(Empty unused) => true;

        public int CompareToNonnull(Empty unused) => 0;

        public override int GetHashCode() => 1565116477;

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);
            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority => 0;

        public override ExpressionType ExpressionType => ExpressionType.Empty;

        internal override void ToString(StringBuilder builder)
        {
            builder.Append('ε');
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
                new(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++))
            );

            GrammarExpressionTuple grammarExpressionTuple = new(
                this,
                new([new Rule([new Chain([])], target)], target),
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

            Label state = new(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            StateMachineExpressionTuple stateMachineExpressionTuple = new(
                this,
                new StateMachine(state, [state], []),
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

        public override bool CanBeEmpty() => true;

        public override Expression TryToLetItBeEmpty() => this;
    }
}
