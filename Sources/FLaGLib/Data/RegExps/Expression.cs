﻿using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public abstract class Expression : IEquatable<Expression>, IComparable<Expression>
    {
        private const int _StartIndex = 1;
        private const string _UnknownGrammarTemplate = "Unknown grammar type: {0}";

        internal Expression() 
        {
            _WalkData = new Lazy<IReadOnlyList<WalkData<Expression>>>(GetWalkData);
            _DependencyMap = new Lazy<IReadOnlyList<DependencyCollection>>(GetDependencyMap);
        }

        public static bool operator ==(Expression objA, Expression objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Expression objA, Expression objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Expression objA, Expression objB)
        {
            if ((object)objA == null)
            {
                if ((object)objB == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return objA.Equals(objB);
        }

        public static int Compare(Expression objA, Expression objB)
        {
            if (objA == null)
            {
                if (objB == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return objA.CompareTo(objB);
        }

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();

        public abstract bool Equals(Expression other);

        public abstract int CompareTo(Expression other);

        public abstract int Priority { get; }

        public abstract ExpressionType ExpressionType { get; }

        private Lazy<IReadOnlyList<WalkData<Expression>>> _WalkData;
        private Lazy<IReadOnlyList<DependencyCollection>> _DependencyMap;

        public IReadOnlyList<DependencyCollection> DependencyMap
        {
            get
            {
                return _DependencyMap.Value;
            }
        }

        protected string UnknownGrammarMessage(GrammarType grammarType)
        {
            return string.Format(_UnknownGrammarTemplate, grammarType);
        }
        
        private ILookup<int,int> GetDependencyIndexMap()
        {
            Stack<int> stack = new Stack<int>();
            IDictionary<int, IList<int>> dependencyMap = new Dictionary<int, IList<int>>();

            foreach (WalkData<Expression> data in _WalkData.Value)
            {
                switch (data.Status)
                {
                    case WalkStatus.Begin:
                        int dependencyIndex = data.Index - _StartIndex;

                        if (stack.Count > 0)
                        {
                            int index = stack.Peek();

                            IList<int> set;

                            if (dependencyMap.ContainsKey(index))
                            {
                                set = dependencyMap[index];
                            }
                            else
                            {
                                dependencyMap[index] = set = new List<int>();
                            }

                            set.Add(dependencyIndex);
                        }
                        stack.Push(dependencyIndex);
                        break;
                    case WalkStatus.End:
                        stack.Pop();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unknown status: {0}.", data.Status));
                }
            }

            return dependencyMap.ToLookup<int, int, IList<int>>();
        }

        internal static void CheckDependencies<T>(T[] dependencies, int expectedCount)
        {
            if (dependencies.Length != expectedCount)
            {
                throw new InvalidOperationException(string.Format("Expected exactly {0} dependencies.", expectedCount));
            }
        }

        public abstract Expression Optimize();

        public abstract bool CanBeEmpty();

        public abstract Expression TryToLetItBeEmpty();

        private IReadOnlyList<Expression> GetSubexpressionsInCalculateOrder()
        {
            return _WalkData.Value.Where(wd => wd.Status == WalkStatus.Begin).OrderBy(wd => wd.Index).Select(wd => wd.Value).ToList().AsReadOnly();
        }

        private IReadOnlyList<DependencyCollection> GetDependencyMap()
        {
            ILookup<int, int> map = GetDependencyIndexMap();
            IReadOnlyList<Expression> subexpressions = GetSubexpressionsInCalculateOrder();

            return subexpressions.Select((e, i) => new DependencyCollection(e, map[i].ToList())).ToList().AsReadOnly();
        }

        internal abstract GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber, 
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies);

        private LanguageExpressionTuple CheckRegular(int languageNumber, Action<LanguagePostReport> onIterate, params LanguageExpressionTuple[] dependencies)
        {
            LanguageExpressionTuple languageExpression = new LanguageExpressionTuple(this, languageNumber);

            if (onIterate != null)
            {
                onIterate(new LanguagePostReport(languageExpression, dependencies));
            }

            return languageExpression;
        }

        public void CheckRegular(Action<LanguagePostReport> onIterate = null)
        {
            IReadOnlyList<DependencyCollection> dependencyMap = DependencyMap;
            LanguageExpressionTuple[] languages = new LanguageExpressionTuple[dependencyMap.Count];

            for (int i = 0; i < dependencyMap.Count; i++)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<LanguageExpressionTuple> dependencies = dependencyMap[i].OrderBy(item => item).Select(item => languages[item]);
                languages[i] = expression.CheckRegular(_StartIndex + i, onIterate, dependencies.ToArray());
            }
        }

        public Grammar MakeGrammar(GrammarType grammarType, Action<GrammarPostReport> onIterate = null)
        {
            IReadOnlyList<DependencyCollection> dependencyMap = DependencyMap;
            GrammarExpressionWithOriginal[] grammars = new GrammarExpressionWithOriginal[dependencyMap.Count];

            int index = _StartIndex;
            int additionalGrammarNumber = _StartIndex + dependencyMap.Count + 1;
                
            for (int i = 0; i < dependencyMap.Count; i++)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<GrammarExpressionWithOriginal> dependencies = dependencyMap[i].Select(item => grammars[item]);
                grammars[i] = new GrammarExpressionWithOriginal(expression.GenerateGrammar(grammarType, _StartIndex + i, ref index, ref additionalGrammarNumber, onIterate, dependencies.ToArray()));
            }

            return grammars[grammars.Length - 1].GrammarExpression.Grammar;
        }

        internal abstract StateMachineExpressionTuple GenerateStateMachine(int stateMachineNumber, ref int index, ref int additionalStateMachineNumber, Action<StateMachinePostReport> onIterate, params StateMachineExpressionWithOriginal[] dependencies);

        public StateMachine MakeStateMachine(Action<StateMachinePostReport> onIterate = null)
        {
            IReadOnlyList<DependencyCollection> dependencyMap = DependencyMap;
            StateMachineExpressionWithOriginal[] stateMachines = new StateMachineExpressionWithOriginal[dependencyMap.Count];

            int index = _StartIndex;
            int additionalStateMachineNumber = _StartIndex + dependencyMap.Count + 1;

            for (int i = 0; i < dependencyMap.Count; i++)
            {
                Expression expression = dependencyMap[i].Expression;
                IEnumerable<StateMachineExpressionWithOriginal> dependencies = dependencyMap[i].Select(item => stateMachines[item]);
                stateMachines[i] = new StateMachineExpressionWithOriginal(expression.GenerateStateMachine(_StartIndex + i, ref index, ref additionalStateMachineNumber, onIterate, dependencies.ToArray()));
            }

            return stateMachines[stateMachines.Length - 1].StateMachineExpression.StateMachine;
        }

        private IReadOnlyList<WalkData<Expression>> GetWalkData()
        {
            WalkDataLabel<DepthData<Expression>>[] depthData = 
               WalkInternal().Select(data => new WalkDataLabel<DepthData<Expression>>(false,0,data)).ToArray();

            int maxDepth = 0;
            int currentDepth = 0;

            foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
            {
                if (data.Value.Status == WalkStatus.Begin)
                {
                    currentDepth++;
                }
                else
                {
                    currentDepth--;
                }

                if (maxDepth < currentDepth)
                {
                    maxDepth = currentDepth;
                }
            }

            int currentIndex = _StartIndex;
          
            while (maxDepth > 0)
            {
                WalkDataLabel<DepthData<Expression>> prev = null;

                foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
                {
                    if (prev != null)
                    {
                        if (prev.Value.Status == WalkStatus.Begin && data.Value.Status == WalkStatus.End && !data.Marked)
                        {
                            prev.Marked = true;
                            data.Marked = true;
                            prev.Index = currentIndex;
                            data.Index = currentIndex;

                            currentIndex++;

                            prev = null;
                        }
                    }

                    if (!data.Marked)
                    {
                        prev = data;
                    }
                }

                maxDepth--;
            }

            return depthData.Select(data => new WalkData<Expression>(data.Value.Status, data.Index, data.Value.Value)).ToList().AsReadOnly();
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
            StringBuilder sb = new StringBuilder();

            ToString(sb);

            return sb.ToString();
        }
    }
}
