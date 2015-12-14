using FLaGLib.Data.Grammars;
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
            _IsRegularSet = new Lazy<bool>(GetIsRegularSet);
            _WalkData = new Lazy<IReadOnlyList<WalkData<Expression>>>(GetWalkData);
            _SubexpressionsInCalculateOrder = new Lazy<IReadOnlyList<Expression>>(GetSubexpressionsInCalculateOrder);
            _DirectDependencies = new Lazy<IReadOnlyList<Expression>>(GetDirectDependencies);
            _DependencyMap = new Lazy<ILookup<int, int>>(GetDependencyMap);
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

        private Lazy<IReadOnlyList<WalkData<Expression>>> _WalkData;
        private Lazy<IReadOnlyList<Expression>> _SubexpressionsInCalculateOrder;
        private Lazy<ILookup<int, int>> _DependencyMap;
        private Lazy<IReadOnlyList<Expression>> _DirectDependencies;

        public IReadOnlyList<Expression> DirectDependencies
        {
            get
            {
                return _DirectDependencies.Value;
            }
        }

        public IReadOnlyList<WalkData<Expression>> WalkData
        {
            get
            {
                return _WalkData.Value;
            }
        }

        protected string UnknownGrammarMessage(GrammarType grammarType)
        {
            return string.Format(_UnknownGrammarTemplate, grammarType);
        }

        protected abstract IReadOnlyList<Expression> GetDirectDependencies();
        
        private ILookup<int, int> GetDependencyMap()
        {
            Stack<int> stack = new Stack<int>();
            IDictionary<int, ISet<int>> dependencyMap = new Dictionary<int, ISet<int>>();

            foreach (WalkData<Expression> data in WalkData)
            {
                switch (data.Status)
                {
                    case WalkStatus.Begin:
                        int dependencyIndex = data.Index - _StartIndex;

                        if (stack.Count > 0)
                        {
                            int index = stack.Peek();

                            ISet<int> set;

                            if (dependencyMap.ContainsKey(index))
                            {
                                set = dependencyMap[index];
                            }
                            else
                            {
                                dependencyMap[index] = set = new HashSet<int>();
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

            return dependencyMap.ToLookup<int,int,ISet<int>>();
        }

        public abstract Expression Optimize();

        public abstract bool CanBeEmpty();

        public abstract Expression TryToLetItBeEmpty();

        private IReadOnlyList<Expression> GetSubexpressionsInCalculateOrder()
        {
            return WalkData.Where(wd => wd.Status == WalkStatus.Begin).OrderBy(wd => wd.Index).Select(wd => wd.Value).ToList().AsReadOnly();
        }

        internal abstract GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber, 
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies);

        public Grammar MakeGrammar(GrammarType grammarType, Action<GrammarPostReport> onIterate = null)
        {
            ILookup<int, int> dependencyMap = _DependencyMap.Value;
            IReadOnlyList<Expression> expressions = _SubexpressionsInCalculateOrder.Value;
            GrammarExpressionWithOriginal[] grammars = new GrammarExpressionWithOriginal[expressions.Count];

            int index = _StartIndex;
            int additionalGrammarNumber = _StartIndex + expressions.Count + 1;
                
            for (int i = 0; i < expressions.Count; i++)
            {
                Expression expression = expressions[i];
                IEnumerable<GrammarExpressionWithOriginal> dependencies = dependencyMap[i].OrderBy(item => item).Select(item => grammars[item]);
                grammars[i] = new GrammarExpressionWithOriginal(expression.GenerateGrammar(grammarType, _StartIndex + i, ref index, ref additionalGrammarNumber, onIterate, dependencies.ToArray()));
            }

            return grammars[grammars.Length - 1].GrammarExpression.Grammar;
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

        public abstract Expression ToRegularSet();

        private Lazy<bool> _IsRegularSet;
        public bool IsRegularSet
        {
            get
            {
                return _IsRegularSet.Value;
            }
        } 

        protected abstract bool GetIsRegularSet();
        
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
