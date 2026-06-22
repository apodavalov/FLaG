using System.Collections.Immutable;
using System.Text;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Extensions;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.RegExps
{
    [ComparableEquatable]
    public sealed partial class Union : Expression
    {
        public IImmutableSet<Expression> Expressions { get; }

        public Union(IEnumerable<Expression> expressions)
        {
            Expressions = expressions.ToImmutableSortedSet();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Union must have at least two items.");
            }
        }

        public bool EqualsNonnull(Union other)
        {
            HashSet<Expression> visitedExpressions = [];
            ImmutableSortedSet<Expression> expression1 = UnionHelper
                .Iterate(visitedExpressions, Expressions)
                .ToImmutableSortedSet();
            visitedExpressions.Clear();
            ImmutableSortedSet<Expression> expression2 = UnionHelper
                .Iterate(visitedExpressions, other.Expressions)
                .ToImmutableSortedSet();

            return expression1.SequenceEqual(expression2);
        }

        public int CompareToNonnull(Union other)
        {
            HashSet<Expression> visitedExpressions = [];
            ImmutableSortedSet<Expression> expression1 = UnionHelper
                .Iterate(visitedExpressions, Expressions)
                .ToImmutableSortedSet();
            visitedExpressions.Clear();
            ImmutableSortedSet<Expression> expression2 = UnionHelper
                .Iterate(visitedExpressions, other.Expressions)
                .ToImmutableSortedSet();
            return expression1.SequenceCompare(expression2);
        }

        public override int FetchHashCode()
        {
            HashCode hashCode = new();
            foreach (Expression expression in Expressions)
            {
                hashCode.Add(expression);
            }
            return hashCode.ToHashCode();
        }

        public override int Priority => 3;

        public override ExpressionType ExpressionType => ExpressionType.Union;

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

        internal override void ToString(StringBuilder builder) =>
            UnionHelper.ToString(builder, Expressions, Priority);

        internal override GrammarExpressionTuple GenerateGrammar(
            GrammarType grammarType,
            int grammarNumber,
            ref int index,
            ref int additionalGrammarNumber,
            Action<GrammarPostReport>? onIterate,
            params GrammarExpressionWithOriginal[] dependencies
        )
        {
            throw new NotSupportedException();
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(
            int stateMachineNumber,
            ref int index,
            ref int additionalStateMachineNumber,
            Action<StateMachinePostReport>? onIterate,
            params StateMachineExpressionWithOriginal[] dependencies
        )
        {
            throw new NotSupportedException();
        }

        public override Expression Optimize()
        {
            bool somethingChanged;

            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            ISet<Expression> set = UnionHelper
                .Iterate(visitedExpressions, Expressions.Select(e => e.Optimize()))
                .ToHashSet();

            do
            {
                somethingChanged = false;

                if (set.Any(e => e.CanBeEmpty()))
                {
                    set = set.Select(e => e.TryToLetItBeEmpty().Optimize()).ToHashSet();
                }

                if (set.Any(e => e.ExpressionType != ExpressionType.Empty && e.CanBeEmpty()))
                {
                    set = set.Where(e => e.ExpressionType != ExpressionType.Empty).ToHashSet();
                }

                List<Expression> list = set.ToList();

                for (int i = 0; i < list.Count; ++i)
                {
                    for (int j = 0; j < list.Count; ++j)
                    {
                        if (i != j && IsASuperSetOfB(list[i], list[j]))
                        {
                            list.RemoveAt(j);
                            --j;

                            if (i > j)
                            {
                                --i;
                            }

                            somethingChanged = true;
                        }
                    }
                }

                for (int i = 0; i < list.Count; ++i)
                {
                    for (int j = 0; j < list.Count; ++j)
                    {
                        if (i != j)
                        {
                            Expression? expression = ExtractFromBrackets(list[i], list[j]);

                            if (expression is not null)
                            {
                                list.RemoveAt(j);

                                if (i > j)
                                {
                                    --i;
                                }

                                list.RemoveAt(i);

                                list.Add(expression);

                                --j;

                                somethingChanged = true;
                            }
                        }
                    }
                }

                set = list.ToHashSet();
            } while (somethingChanged);

            return UnionHelper.MakeExpression(set);
        }

        private static Expression? ExtractFromBrackets(
            Expression expressionA,
            Expression expressionB
        )
        {
            IImmutableList<Expression> concatA =
                (expressionA as Concat)?.Expressions ?? [expressionA];
            IImmutableList<Expression> concatB =
                (expressionB as Concat)?.Expressions ?? [expressionB];

            int left = 0;
            int right = 0;

            int count = Math.Min(concatA.Count, concatB.Count);

            while (left < count && concatA[left] == concatB[left])
            {
                ++left;
            }

            while (
                right < count - left
                && concatA[concatA.Count - 1 - right] == concatB[concatB.Count - 1 - right]
            )
            {
                ++right;
            }

            if (left == 0 && right == 0)
            {
                return null;
            }

            List<Expression> newConcat = [];
            List<Expression> leftConcat = [];
            List<IList<Expression>> middleUnion = [];
            List<Expression> rightConcat = [];

            for (int i = 0; i < left; ++i)
            {
                leftConcat.Add(concatA[i]);
            }

            for (int i = 0; i < right; ++i)
            {
                rightConcat.Add(concatB[concatB.Count - 1 - i]);
            }

            rightConcat.Reverse();

            List<Expression> middleConcat = [];
            for (int i = left; i < concatA.Count - right; ++i)
            {
                middleConcat.Add(concatA[i]);
            }

            middleUnion.Add(middleConcat);
            middleConcat = [];

            for (int i = left; i < concatB.Count - right; ++i)
            {
                middleConcat.Add(concatB[i]);
            }

            middleUnion.Add(middleConcat);

            newConcat.AddRange(leftConcat);
            newConcat.Add(
                UnionHelper.MakeExpression(
                    middleUnion.Select(c =>
                        ConcatHelper.MakeExpression(
                            c.Where(e => e.ExpressionType != ExpressionType.Empty)
                        )
                    )
                )
            );
            newConcat.AddRange(rightConcat);

            return ConcatHelper.MakeExpression(newConcat).Optimize();
        }

        private static bool IsASuperSetOfB(Expression expressionA, Expression expressionB)
        {
            IImmutableList<Expression> expressionsA =
                (expressionA as Concat)?.Expressions ?? [expressionA];

            IImmutableList<Expression> expressionsB =
                (expressionB as Concat)?.Expressions ?? [expressionB];

            int[,] matrix = new int[expressionsA.Count + 1, expressionsB.Count + 1];

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                matrix[i, 0] = 0;
            }

            for (int i = 0; i < matrix.GetLength(1); ++i)
            {
                matrix[0, i] = 0;
            }

            for (int i = 1; i < matrix.GetLength(0); ++i)
            {
                for (int j = 1; j < matrix.GetLength(1); ++j)
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

            LinkedList<Expression> commonExpressions = [];

            int x = matrix.GetLength(0) - 1;
            int y = matrix.GetLength(1) - 1;

            while (matrix[x, y] != 0)
            {
                if (expressionsA[x - 1] == expressionsB[y - 1])
                {
                    commonExpressions.AddFirst(expressionsA[x - 1]);
                    --x;
                    --y;
                }
                else
                {
                    if (matrix[x - 1, y] == matrix[x, y])
                    {
                        --x;
                    }
                    else
                    {
                        --y;
                    }
                }
            }

            int expressionsACounter = 0;
            int expressionsBCounter = 0;

            List<List<Expression>> listOfListsToRemove = [];
            List<List<Expression>> listOfListsToAdd = [];

            List<Expression> listToRemove;
            List<Expression> listToAdd;

            foreach (Expression expression in commonExpressions)
            {
                listToRemove = [];
                listToAdd = [];

                while (expression != expressionsA[expressionsACounter])
                {
                    listToRemove.Add(expressionsA[expressionsACounter]);
                    ++expressionsACounter;
                }

                while (expression != expressionsB[expressionsBCounter])
                {
                    listToAdd.Add(expressionsB[expressionsBCounter]);
                    ++expressionsBCounter;
                }

                if (listToRemove.Count > 0 || listToAdd.Count > 0)
                {
                    listOfListsToRemove.Add(listToRemove);
                    listOfListsToAdd.Add(listToAdd);
                }

                ++expressionsACounter;
                ++expressionsBCounter;
            }

            listToRemove = [];
            listToAdd = [];

            while (expressionsACounter < expressionsA.Count)
            {
                listToRemove.Add(expressionsA[expressionsACounter]);
                ++expressionsACounter;
            }

            while (expressionsBCounter < expressionsB.Count)
            {
                listToAdd.Add(expressionsB[expressionsBCounter]);
                ++expressionsBCounter;
            }

            if (listToRemove.Count > 0 || listToAdd.Count > 0)
            {
                listOfListsToRemove.Add(listToRemove);
                listOfListsToAdd.Add(listToAdd);
            }

            bool allExpressionsAAreSupersetsOfExpressionsB = true;
            int count = listOfListsToRemove.Count;

            for (int i = 0; i < count; ++i)
            {
                if (!IsASuperSetOfB(listOfListsToRemove[i], listOfListsToAdd[i]))
                {
                    allExpressionsAAreSupersetsOfExpressionsB = false;
                    break;
                }
            }

            return allExpressionsAAreSupersetsOfExpressionsB;
        }

        private static bool IsASuperSetOfB(
            List<Expression> expressionsA,
            List<Expression> expressionsB
        )
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
                    if (expressionsA[i] is Iteration iterationA)
                    {
                        if (!iterationA.IsPositive)
                        {
                            if (expressionsB[i] is Iteration iterationB)
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
