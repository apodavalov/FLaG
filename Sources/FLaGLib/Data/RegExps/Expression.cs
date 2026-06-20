using System.Collections.Immutable;
using System.Text;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Data.RegExps
{
    [ComparableEquatable]
    public abstract partial class Expression
    {
        private const int _StartIndex = 1;

        internal Expression()
        {
            _WalkData = new Lazy<IImmutableList<WalkData<Expression>>>(GetWalkData);
            _DependencyMap = new Lazy<IImmutableList<DependencyCollection>>(GetDependencyMap);
        }

        public abstract override int GetHashCode();

        public virtual bool EqualsNonnull(Expression other)
        {
            return ExpressionType == other.ExpressionType;
        }

        public virtual int CompareToNonnull(Expression other)
        {
            return ExpressionType.CompareTo(other.ExpressionType);
        }

        public abstract int Priority { get; }

        public abstract ExpressionType ExpressionType { get; }

        private Lazy<IImmutableList<WalkData<Expression>>> _WalkData;
        private Lazy<IImmutableList<DependencyCollection>> _DependencyMap;

        public IImmutableList<DependencyCollection> DependencyMap
        {
            get { return _DependencyMap.Value; }
        }

        private ILookup<int, int> GetDependencyIndexMap()
        {
            Stack<int> stack = [];
            Dictionary<int, List<int>> dependencyMap = [];

            foreach (WalkData<Expression> data in _WalkData.Value)
            {
                switch (data.Status)
                {
                    case WalkStatus.Begin:
                        int dependencyIndex = data.Index - _StartIndex;

                        if (stack.Count > 0)
                        {
                            int index = stack.Peek();

                            List<int> set;

                            if (dependencyMap.TryGetValue(index, out List<int>? value))
                            {
                                set = value;
                            }
                            else
                            {
                                dependencyMap[index] = set = [];
                            }

                            set.Add(dependencyIndex);
                        }
                        stack.Push(dependencyIndex);
                        break;
                    case WalkStatus.End:
                        stack.Pop();
                        break;
                    default:
                        throw new InvalidOperationException(
                            string.Format("Unknown status: {0}.", data.Status)
                        );
                }
            }

            return dependencyMap.ToLookup<int, int, List<int>>();
        }

        internal static void CheckDependencies<T>(T[] dependencies, int expectedCount)
        {
            if (dependencies.Length != expectedCount)
            {
                throw new InvalidOperationException(
                    string.Format("Expected exactly {0} dependencies.", expectedCount)
                );
            }
        }

        public abstract Expression Optimize();

        public abstract bool CanBeEmpty();

        public abstract Expression TryToLetItBeEmpty();

        private ImmutableList<Expression> GetSubexpressionsInCalculateOrder()
        {
            return _WalkData
                .Value.Where(wd => wd.Status == WalkStatus.Begin)
                .OrderBy(wd => wd.Index)
                .Select(wd => wd.Value)
                .ToImmutableList();
        }

        private ImmutableList<DependencyCollection> GetDependencyMap()
        {
            ILookup<int, int> map = GetDependencyIndexMap();
            IImmutableList<Expression> subexpressions = GetSubexpressionsInCalculateOrder();
            return subexpressions
                .Select((e, i) => new DependencyCollection(e, map[i].ToImmutableList()))
                .ToImmutableList();
        }

        internal abstract GrammarExpressionTuple GenerateGrammar(
            GrammarType grammarType,
            int grammarNumber,
            ref int index,
            ref int additionalGrammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        );

        private LanguageExpressionTuple CheckRegular(
            int languageNumber,
            Action<LanguagePostReport>? onIterate,
            params LanguageExpressionTuple[] dependencies
        )
        {
            LanguageExpressionTuple languageExpression = new(this, languageNumber);
            onIterate?.Invoke(new LanguagePostReport(languageExpression, dependencies));
            return languageExpression;
        }

        public void CheckRegular(Action<LanguagePostReport>? onIterate = null)
        {
            IImmutableList<DependencyCollection> dependencyMap = DependencyMap;
            LanguageExpressionTuple[] languages = new LanguageExpressionTuple[dependencyMap.Count];

            for (int i = 0; i < dependencyMap.Count; ++i)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<LanguageExpressionTuple> dependencies = dependencyMap[i]
                    .Order()
                    .Select(item => languages[item]);
                languages[i] = expression.CheckRegular(
                    _StartIndex + i,
                    onIterate,
                    dependencies.ToArray()
                );
            }
        }

        public Grammar MakeGrammar(
            GrammarType grammarType,
            Action<GrammarPostReport>? onIterate = null
        )
        {
            IImmutableList<DependencyCollection> dependencyMap = DependencyMap;
            GrammarExpressionWithOriginal[] grammars = new GrammarExpressionWithOriginal[
                dependencyMap.Count
            ];

            int index = _StartIndex;
            int additionalGrammarNumber = _StartIndex + dependencyMap.Count + 1;

            for (int i = 0; i < dependencyMap.Count; ++i)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<GrammarExpressionWithOriginal> dependencies = dependencyMap[i]
                    .Select(item => grammars[item]);
                grammars[i] = new GrammarExpressionWithOriginal(
                    expression.GenerateGrammar(
                        grammarType,
                        _StartIndex + i,
                        ref index,
                        ref additionalGrammarNumber,
                        onIterate,
                        dependencies.ToArray()
                    )
                );
            }

            return grammars[^1].GrammarExpression.Grammar;
        }

        internal abstract StateMachineExpressionTuple GenerateStateMachine(
            int stateMachineNumber,
            ref int index,
            ref int additionalStateMachineNumber,
            Action<StateMachinePostReport>? onIterate,
            params StateMachineExpressionWithOriginal[] dependencies
        );

        public StateMachine MakeStateMachine(Action<StateMachinePostReport>? onIterate = null)
        {
            IImmutableList<DependencyCollection> dependencyMap = DependencyMap;
            StateMachineExpressionWithOriginal[] stateMachines =
                new StateMachineExpressionWithOriginal[dependencyMap.Count];

            int index = _StartIndex;
            int additionalStateMachineNumber = _StartIndex + dependencyMap.Count + 1;

            for (int i = 0; i < dependencyMap.Count; ++i)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<StateMachineExpressionWithOriginal> dependencies = dependencyMap[i]
                    .Select(item => stateMachines[item]);
                stateMachines[i] = new StateMachineExpressionWithOriginal(
                    expression.GenerateStateMachine(
                        _StartIndex + i,
                        ref index,
                        ref additionalStateMachineNumber,
                        onIterate,
                        dependencies.ToArray()
                    )
                );
            }

            return stateMachines[^1].StateMachineExpression.StateMachine;
        }

        private ImmutableList<WalkData<Expression>> GetWalkData()
        {
            WalkDataLabel<DepthData<Expression>>[] depthData = WalkInternal()
                .Select(data => new WalkDataLabel<DepthData<Expression>>(false, 0, data))
                .ToArray();

            int maxDepth = 0;
            int currentDepth = 0;

            foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
            {
                if (data.Value.Status == WalkStatus.Begin)
                {
                    ++currentDepth;
                }
                else
                {
                    --currentDepth;
                }

                if (maxDepth < currentDepth)
                {
                    maxDepth = currentDepth;
                }
            }

            int currentIndex = _StartIndex;

            while (maxDepth > 0)
            {
                WalkDataLabel<DepthData<Expression>>? prev = null;

                foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
                {
                    if (prev is not null)
                    {
                        if (
                            prev.Value.Status == WalkStatus.Begin
                            && data.Value.Status == WalkStatus.End
                            && !data.Marked
                        )
                        {
                            prev.Marked = true;
                            data.Marked = true;
                            prev.Index = currentIndex;
                            data.Index = currentIndex;

                            ++currentIndex;

                            prev = null;
                        }
                    }

                    if (!data.Marked)
                    {
                        prev = data;
                    }
                }

                --maxDepth;
            }

            return depthData
                .Select(data => new WalkData<Expression>(
                    data.Value.Status,
                    data.Index,
                    data.Value.Value
                ))
                .ToImmutableList();
        }

        internal abstract IEnumerable<DepthData<Expression>> WalkInternal();

        internal abstract void ToString(StringBuilder builder);

        internal void ToString(bool needBrackets, StringBuilder builder)
        {
            if (needBrackets)
            {
                builder.Append('(');
            }

            ToString(builder);

            if (needBrackets)
            {
                builder.Append(')');
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            ToString(sb);
            return sb.ToString();
        }
    }
}
