using FLaGLib.Collections;
using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class Union : Expression, IEquatable<Union>, IComparable<Union>
    {
        public IReadOnlySet<Expression> Expressions
        {
            get;
            private set;
        }

        public Union(IEnumerable<Expression> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            Expressions = new SortedSet<Expression>(expressions).AsReadOnly();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Union must have at least two items.");
            }
        }

        public static bool operator ==(Union objA, Union objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Union objA, Union objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Union objA, Union objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Union objA, Union objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Union objA, Union objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Union objA, Union objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Union objA, Union objB)
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

        public static int Compare(Union objA, Union objB)
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

        public bool Equals(Union other)
        {
            if (other == null)
            {
                return false;
            }

            ISet<Expression> visitedExpressions = new HashSet<Expression>();
            ISet<Expression> expression1 = UnionHelper.Iterate(visitedExpressions, Expressions).ToSortedSet();
            visitedExpressions.Clear();
            ISet<Expression> expression2 = UnionHelper.Iterate(visitedExpressions, other.Expressions).ToSortedSet();

            return expression1.SequenceEqual(expression2);
        }

        public int CompareTo(Union other)
        {
            if (other == null)
            {
                return 1;
            }

            ISet<Expression> visitedExpressions = new HashSet<Expression>();
            ISet<Expression> expression1 = UnionHelper.Iterate(visitedExpressions, Expressions).ToSortedSet();
            visitedExpressions.Clear();
            ISet<Expression> expression2 = UnionHelper.Iterate(visitedExpressions, other.Expressions).ToSortedSet();

            return expression1.SequenceCompare(expression2);
        }

        public override bool Equals(object obj)
        {
            Union union = obj as Union;
            return Equals(union);
        }

        public override int GetHashCode()
        {
            return Expressions.GetSequenceHashCode();
        }

        public override bool Equals(Expression other)
        {
            Union union = other as Union;
            return Equals(union);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Union)
            {
                return CompareTo((Union)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        public override int Priority
        {
            get
            {
                return 3;
            }
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new Union(Expressions.Select(e => e.ToRegularSet()));
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
            UnionHelper.ToString(builder, Expressions, Priority);
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return Expressions.ToList().AsReadOnly();
        }

        protected override Grammar GenerateGrammar(GrammarType grammarType)
        {
            throw new NotSupportedException();
        }

        public override Expression Optimize()
        {
            bool somethingChanged;

            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            ISet<Expression> set = UnionHelper.Iterate(visitedExpressions, Expressions.Select(e => e.Optimize())).ToHashSet();

            do
            {
                somethingChanged = false;

                if (set.Any(e => e.CanBeEmpty()))
                {
                    set = set.Select(e => e.TryToLetItBeEmpty().Optimize()).ToHashSet();
                }

                if (set.Any(e => e != Empty.Instance && e.CanBeEmpty()))
                {
                    set = set.Where(e => e != Empty.Instance).ToHashSet();
                }

                IList<Expression> list = set.ToList();
                
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (i != j && IsASuperSetOfB(list[i],list[j]))
                        {
                            list.RemoveAt(j);
                            j--;

                            if (i > j)
                            {
                                i--;
                            }

                            somethingChanged = true;
                        }
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (i != j)
                        {
                            Expression expression = ExtractFromBrackets(list[i], list[j]);

                            if (expression != null)
                            {
                                list.RemoveAt(j);

                                if (i > j)
                                {
                                    i--;
                                }

                                list.RemoveAt(i);

                                list.Add(expression);

                                j--;

                                somethingChanged = true;
                            }
                        }
                    }
                }

                set = list.ToHashSet();

            } while (somethingChanged);

            if (set.Count == 0)
            {
                return Empty.Instance;
            }
            else if (set.Count == 1)
            {
                return set.Single();
            }
            else
            {
                return new Union(set);
            }
        }

        private static Expression ExtractFromBrackets(Expression expressionA, Expression expressionB)
        {
            throw new NotImplementedException();
        }

        private static bool IsASuperSetOfB(Expression expressionA, Expression expressionB)
        {
            throw new NotImplementedException();
        }

        public override bool CanBeEmpty()
        {
            return Expressions.Any(e => e.CanBeEmpty());
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new Union(Expressions.Select(e => e.TryToLetItBeEmpty()));
        }
    }
}
