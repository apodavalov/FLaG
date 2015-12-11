using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class ConstIteration : Expression, IEquatable<ConstIteration>, IComparable<ConstIteration>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public int IterationCount
        {
            get;
            private set;
        }

        public ConstIteration(Expression expression, int iterationCount)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (iterationCount < 0)
            {
                throw new ArgumentException("Count must not be less than zero.");
            }

            Expression = expression;
            IterationCount = iterationCount;
        }

        public static bool operator ==(ConstIteration objA, ConstIteration objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(ConstIteration objA, ConstIteration objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(ConstIteration objA, ConstIteration objB)
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

        public static int Compare(ConstIteration objA, ConstIteration objB)
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

        public bool Equals(ConstIteration other)
        {
            if (other == null)
            {
                return false;
            }

            return Expression.Equals(other.Expression) && IterationCount.Equals(other.IterationCount);
        }

        public int CompareTo(ConstIteration other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Expression.CompareTo(other.Expression);

            if (result != 0)
            {
                return result;
            }

            return IterationCount.CompareTo(other.IterationCount);
        }

        public override bool Equals(object obj)
        {
            ConstIteration constIteration = obj as ConstIteration;
            return Equals(constIteration);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode() ^ IterationCount.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            ConstIteration constIteration = other as ConstIteration;
            return Equals(constIteration);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is ConstIteration)
            {
                return CompareTo((ConstIteration)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (DepthData<Expression> data in Expression.WalkInternal())
            {
                yield return data;
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority
        {
            get 
            {
                return 1;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            Expression.ToString(Expression.Priority > Priority,builder);
            builder.Append("^(");
            builder.Append(IterationCount);
            builder.Append(')');
        }

        public override Expression ToRegularSet()
        {
            if (IterationCount == 0)
            {
                return Empty.Instance.ToRegularSet();
            }

            Expression expression = Expression.ToRegularSet();
            Expression result = Expression.ToRegularSet();

            for (int i = 1; i < IterationCount; i++)
            {
                result = new BinaryConcat(result, expression);
            }

            return result;
        }

        protected override bool GetIsRegularSet()
        {
            return false;
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return EnumerateHelper.Sequence(Expression).ToList().AsReadOnly();
        }

        internal override Grammar GenerateGrammar(GrammarType grammarType, ref int index, params Grammar[] dependencies)
        {
            if (dependencies.Length != 1)
            {
                throw new InvalidOperationException("Expected exactly 1 dependency.");
            }

            if (IterationCount == 0)
            {
                return Empty.Instance.GenerateGrammar(grammarType, ref index);
            }

            Expression expression = Expression;
            Grammar dependency1 = dependencies[0]; 

            for (int i = 1; i < IterationCount; i++)
            {
                Grammar dependency2 = Mirror(dependencies[0], ref index);
                expression = new BinaryConcat(expression, Expression);
                dependency1 = expression.GenerateGrammar(grammarType, ref index, dependency1, dependency2);
            }

            return dependency1;
        }

        private Grammar Mirror(Grammar grammar, ref int index)
        {
            Grammar newGrammar = grammar.Reorganize(index);
            index += newGrammar.NonTerminals.Count;

            return newGrammar;
        }

        public override Expression Optimize()
        {
            switch (IterationCount)
            {
                case 0:
                    return Empty.Instance;
                case 1:
                    return Expression.Optimize();
                default:
                    Expression expression = Expression.Optimize();

                    ConstIteration constIteration = expression.As<ConstIteration>();

                    if (constIteration != null)
                    {
                        return new ConstIteration(constIteration.Expression, constIteration.IterationCount * IterationCount);
                    }

                    Iteration iteration = expression.As<Iteration>();

                    if (iteration != null && !iteration.IsPositive)
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
