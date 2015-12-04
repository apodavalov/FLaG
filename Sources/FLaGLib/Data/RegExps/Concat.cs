﻿using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class Concat : Expression, IEquatable<Concat>, IComparable<Concat>
    {
        public IReadOnlyList<Expression> Expressions
        {
            get;
            private set;
        }

        public Concat(IEnumerable<Expression> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            Expressions = new List<Expression>(expressions).AsReadOnly();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }
        }

        public static bool operator ==(Concat objA, Concat objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Concat objA, Concat objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Concat objA, Concat objB)
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

        public static int Compare(Concat objA, Concat objB)
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

        public bool Equals(Concat other)
        {
            if (other == null)
            {
                return false;
            }

            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Expressions);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Expressions);

            return expression1.SequenceEqual(expression2);
        }

        public int CompareTo(Concat other)
        {
            if (other == null)
            {
                return 1;
            }

            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Expressions);
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Expressions);

            return expression1.SequenceCompare(expression2);
        }

        public override bool Equals(object obj)
        {
            Concat concat = obj as Concat;
            return Equals(concat);
        }

        public override int GetHashCode()
        {
            return Expressions.GetSequenceHashCode();
        }

        public override bool Equals(Expression other)
        {
            Concat concat = other as Concat;
            return Equals(concat);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Concat)
            {
                return CompareTo((Concat)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        public override int Priority
        {
            get 
            {
                return 2;
            }
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new Concat(Expressions.Select(e => e.ToRegularSet()));
        }

        protected override bool GetIsRegularSet()
        {
            return Expressions.All(e => e.IsRegularSet);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (Expression expression in Expressions)
            {
                foreach (DepthData<Expression> data in expression.WalkInternal())
                {
                    yield return data;
                }
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        internal override void ToString(StringBuilder builder)
        {
            ConcatHelper.ToString(builder, Expressions, Priority);
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return Expressions;
        }

        protected override Grammar GenerateGrammar(GrammarType grammarType)
        {
            throw new NotSupportedException();
        }

        public override Expression Optimize()
        {
            bool somethingChanged;

            Expression previous = null;

            IList<Expression> oldList = ConcatHelper.Iterate(Expressions.Select(e => e.Optimize())).ToList();
            IList<Expression> newList;

            do
            {
                somethingChanged = false;
                newList = new List<Expression>();

                foreach (Expression expression in oldList)
                {
                    if (expression == Empty.Instance)
                    {
                        somethingChanged = true;
                        continue;
                    }

                    if (previous == null)
                    {
                        previous = expression;
                        continue;
                    }

                    Iteration iteration1 = previous.As<Iteration>();
                    Iteration iteration2 = expression.As<Iteration>();

                    if (iteration1 != null || iteration2 != null)
                    {
                        if (iteration1 != null && iteration2 != null && iteration1.Expression == iteration2.Expression)
                        {
                            if (iteration1.IsPositive && iteration2.IsPositive)
                            {
                                newList.Add(iteration1.Expression);
                            }

                            previous = new Iteration(iteration1.Expression, iteration1.IsPositive || iteration2.IsPositive);
                            somethingChanged = true;
                            continue;
                        }

                        Iteration iteration;
                        Expression newExpression;

                        if (iteration1 != null)
                        {
                            iteration = iteration1;
                            newExpression = expression;
                        }
                        else
                        {
                            iteration = iteration2;
                            newExpression = previous;
                        }

                        if (iteration.Expression == newExpression && !iteration.IsPositive)
                        {
                            previous = new Iteration(newExpression, true);
                            somethingChanged = true;
                            continue;
                        }
                    }

                    ConstIteration constIteration1 = previous.As<ConstIteration>();
                    ConstIteration constIteration2 = expression.As<ConstIteration>();

                    if (constIteration1 != null || constIteration2 != null)
                    {
                        if (constIteration1 != null && constIteration2 != null && constIteration1.Expression == constIteration2.Expression)
                        {
                            previous = new ConstIteration(constIteration1.Expression, constIteration1.IterationCount + constIteration2.IterationCount);
                            somethingChanged = true;
                            continue;
                        }

                        ConstIteration constIteration;
                        Expression newExpression;

                        if (constIteration1 != null)
                        {
                            constIteration = constIteration1;
                            newExpression = expression;
                        }
                        else
                        {
                            constIteration = constIteration2;
                            newExpression = previous;
                        }

                        if (constIteration.Expression == newExpression)
                        {
                            previous = new ConstIteration(newExpression, constIteration.IterationCount + 1);
                            somethingChanged = true;
                            continue;
                        }
                    }

                    if (constIteration1 != null && iteration2 != null && constIteration1.Expression == iteration2.Expression)
                    {
                        if (constIteration1.IterationCount > 2)
                        {
                            newList.Add(new ConstIteration(constIteration1.Expression, constIteration1.IterationCount - 1));
                        }
                        else
                        {
                            newList.Add(constIteration1.Expression);
                        }

                        previous = new Iteration(iteration2.Expression, true);
                        somethingChanged = true;
                        continue;
                    }

                    if (iteration1 != null && constIteration2 != null && iteration1.Expression == constIteration2.Expression)
                    {
                        newList.Add(new Iteration(iteration1.Expression, true));

                        if (constIteration2.IterationCount > 2)
                        {
                            previous = new ConstIteration(constIteration2.Expression, constIteration2.IterationCount - 1);
                        }
                        else
                        {
                            previous = constIteration2.Expression;
                        }

                        somethingChanged = true;
                        continue;
                    }

                    newList.Add(previous);
                    previous = expression;
                }

                if (previous != null)
                {
                    newList.Add(previous);
                }

                oldList = newList;

                newList = new List<Expression>();

                previous = null;
                Expression previousPrevious = null;

                foreach (Expression expression in oldList)
                {
                    if (previousPrevious == null)
                    {
                        previousPrevious = previous;
                        previous = expression;
                        continue;
                    }

                    Iteration iteration = previous.As<Iteration>();

                    if (iteration != null)
                    {
                        Concat concat = iteration.Expression.As<Concat>();

                        if (concat != null && concat.Expressions.Count == 2 && concat.Expressions[0] == expression && concat.Expressions[1] == previousPrevious)
                        {
                            concat = new Concat(concat.Expressions.FastReverse());

                            if (iteration.IsPositive)
                            {
                                previousPrevious = concat;
                            }
                            else
                            {
                                previousPrevious = null;
                            }

                            previous = new Iteration(concat, true);

                            somethingChanged = true;
                            continue;
                        }
                    }

                    newList.Add(previousPrevious);

                    previousPrevious = previous;
                    previous = expression;
                }

                if (previousPrevious != null)
                {
                    newList.Add(previousPrevious);
                }

                if (previous != null)
                {
                    newList.Add(previous);
                }

                oldList = newList;

            } while (somethingChanged);

            if (newList.Count == 0)
            {
                return Empty.Instance;
            }
            else if (newList.Count == 1)
            {
                return newList.Single();
            }
            else
            {
                return new Concat(newList);
            }
        }

        public override bool CanBeEmpty()
        {
            return Expressions.All(e => e.CanBeEmpty());
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new Concat(Expressions.Select(e => e.TryToLetItBeEmpty()));
        }
    }
}
