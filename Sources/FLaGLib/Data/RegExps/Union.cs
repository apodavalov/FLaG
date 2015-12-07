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

            return UnionHelper.MakeExpression(set);
        }

        private static Expression ExtractFromBrackets(Expression expressionA, Expression expressionB)
        {
            IReadOnlyList<Expression> concatA = expressionA.As<Concat>()?.Expressions;

            if (concatA == null)
            {
                concatA = expressionA.AsSequence().ToList().AsReadOnly();
            }

            IReadOnlyList<Expression> concatB = expressionB.As<Concat>()?.Expressions;

            if (concatB == null)
            {
                concatB = expressionB.AsSequence().ToList().AsReadOnly();
            }

            int left = 0;
            int right = 0;

            int count = Math.Min(concatA.Count, concatB.Count);

            while (left < count && concatA[left] == concatB[left])
            {
                left++;
            }

            while (right < count - left && concatA[concatA.Count - 1 - right] == concatB[concatB.Count - 1 - right])
            {
                right++;
            }

            if (left == 0 && right == 0)
            {
                return null;
            }

            IList<Expression> newConcat = new List<Expression>();
            IList<Expression> leftConcat = new List<Expression>();
            IList<IList<Expression>> middleUnion = new List<IList<Expression>>();
            IList<Expression> rightConcat = new List<Expression>();

            for (int i = 0; i < left; i++)
            {
                leftConcat.Add(concatA[i]);
            }

            for (int i = 0; i < right; i++)
            {
                rightConcat.Add(concatB[concatB.Count - 1 - i]);
            }

            rightConcat = rightConcat.AsReadOnly().FastReverse().ToList();

            IList<Expression> middleConcat = new List<Expression>();

            for (int i = left; i < concatA.Count - right; i++)
            {
                middleConcat.Add(concatA[i]);
            }

            middleUnion.Add(middleConcat);

            middleConcat = new List<Expression>();

            for (int i = left; i < concatB.Count - right; i++)
            {
                middleConcat.Add(concatB[i]);
            }

            middleUnion.Add(middleConcat);

            newConcat.AddRange(leftConcat);
            newConcat.Add(
                UnionHelper.MakeExpression(
                    middleUnion.Select(
                        c => ConcatHelper.MakeExpression(c.Where(e => e != Empty.Instance).ToList())
                    ).ToList()
                )
            );
            newConcat.AddRange(rightConcat);

            return ConcatHelper.MakeExpression(newConcat).Optimize();
        }

        private static bool IsASuperSetOfB(Expression expressionA, Expression expressionB)
        {
            IReadOnlyList<Expression> expressionsA = expressionA.As<Concat>()?.Expressions;

            if (expressionsA == null)
            {
                expressionsA = expressionA.AsSequence().ToList().AsReadOnly();
            }

            IReadOnlyList<Expression> expressionsB = expressionB.As<Concat>()?.Expressions;

            if (expressionsB == null)
            {
                expressionsB = expressionB.AsSequence().ToList().AsReadOnly();
            }

            int[,] matrix = new int[expressionsA.Count + 1,expressionsB.Count + 1];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                matrix[i,0] = 0;
            }

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                matrix[0,i] = 0;
            }

            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                for (int j = 1; j < matrix.GetLength(1); j++)
                {
                    if (expressionsA[i - 1] == expressionsB[j - 1])
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i, j - 1], matrix[i - 1, j]);
                    }
                }
            }

            LinkedList<Expression> commonExpressions = new LinkedList<Expression>();

            int x = matrix.GetLength(0) - 1;
            int y = matrix.GetLength(1) - 1;

            while (matrix[x,y] != 0)
            {
                if (expressionsA[x - 1] == expressionsB[y - 1])
                {
                    commonExpressions.AddFirst(expressionsA[x - 1]);
                    x--;
                    y--;
                }
                else
                {
                    if (matrix[x - 1, y] == matrix[x, y])
                    {
                        x--;
                    }
                    else
                    {
                        y--;
                    }
                }
            }

            int expressionsACounter = 0;
            int expressionsBCounter = 0;

            IList<IList<Expression>> listOfListsToRemove = new List<IList<Expression>>();
            IList<IList<Expression>> listOfListsToAdd = new List<IList<Expression>>();

            IList<Expression> listToRemove;
            IList<Expression> listToAdd;

            foreach (Expression expression in commonExpressions)
            {
                listToRemove = new List<Expression>();
                listToAdd = new List<Expression>();

                while (expression != expressionsA[expressionsACounter])
                {
                    listToRemove.Add(expressionsA[expressionsACounter]);
                    expressionsACounter++;
                }

                while (expression != expressionsB[expressionsBCounter])
                {
                    listToAdd.Add(expressionsB[expressionsBCounter]);
                    expressionsBCounter++;
                }

                if (listToRemove.Count > 0 || listToAdd.Count > 0)
                {
                    listOfListsToRemove.Add(listToRemove);
                    listOfListsToAdd.Add(listToAdd);
                }

                expressionsACounter++;
                expressionsBCounter++;
            }

            listToRemove = new List<Expression>();
            listToAdd = new List<Expression>();

            while (expressionsACounter < expressionsA.Count)
            {
                listToRemove.Add(expressionsA[expressionsACounter]);
                expressionsACounter++;
            }

            while (expressionsBCounter < expressionsB.Count)
            {
                listToAdd.Add(expressionsB[expressionsBCounter]);
                expressionsBCounter++;
            }

            if (listToRemove.Count > 0 || listToAdd.Count > 0)
            {
                listOfListsToRemove.Add(listToRemove);
                listOfListsToAdd.Add(listToAdd);
            }

            bool allExpressionsAAreSupersetsOfExpressionsB = true;

            int count = listOfListsToRemove.Count;

            for (int i = 0; i < count; i++)
            {
                if (!IsASuperSetOfB(listOfListsToRemove[i], listOfListsToAdd[i]))
                {
                    allExpressionsAAreSupersetsOfExpressionsB = false;
                    break;
                }
            }

            return allExpressionsAAreSupersetsOfExpressionsB;
        }

        private static bool IsASuperSetOfB(IList<Expression> expressionsA, IList<Expression> expressionsB)
        {
            if (expressionsB.Count > expressionsA.Count)
            {
                return false;
            }

            int minLength = expressionsB.Count;

            for (int i = 0; i < minLength; i++)
            {
                bool res = false;

                if (expressionsA[i] == expressionsB[i])
                {
                    res = true;
                }
                else
                {
                    Iteration iterationA = expressionsA[i].As<Iteration>();

                    if (iterationA != null)
                    {
                        if (!iterationA.IsPositive)
                        {
                            Iteration iterationB = expressionsB[i].As<Iteration>();

                            if (iterationB != null)
                            {
                                if (iterationA.Expression == iterationB.Expression)
                                {
                                    res = true;
                                }
                            }
                            else if (iterationA.Expression == expressionsB[i])
                            {
                                res = true;
                            }
                        }
                        else if (iterationA.Expression == expressionsB[i])
                        {
                            res = true;
                        }
                    }
                }

                if (!res)
                {
                    return false;
                }
            }

            for (int i = minLength; i < expressionsA.Count; i++)
            {
                if (!expressionsA[i].CanBeEmpty())
                {
                    return false;
                }
            }

            return true;
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
