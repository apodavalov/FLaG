using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class BinaryUnion : Expression, IEquatable<BinaryUnion>, IComparable<BinaryUnion>
    {
        public Expression Left
        {
            get;
            private set;
        }

        public Expression Right
        {
            get;
            private set;
        }

        public BinaryUnion(Expression left, Expression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            Left = left;
            Right = right;
        }

         public static bool operator ==(BinaryUnion objA, BinaryUnion objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(BinaryUnion objA, BinaryUnion objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(BinaryUnion objA, BinaryUnion objB)
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

        public static int Compare(BinaryUnion objA, BinaryUnion objB)
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

        public bool Equals(BinaryUnion other)
        {
            if (other == null)
            {
                return false;
            }

            ISet<Expression> visitedExpressions = new HashSet<Expression>();
            ISet<Expression> expression1 = UnionHelper.Iterate(visitedExpressions,Left.AsSequence().Concat(Right)).ToSortedSet();
            visitedExpressions.Clear();
            ISet<Expression> expression2 = UnionHelper.Iterate(visitedExpressions, other.Left.AsSequence().Concat(other.Right)).ToSortedSet();

            return expression1.SequenceEqual(expression2);
        }

        public int CompareTo(BinaryUnion other)
        {
            if (other == null)
            {
                return 1;
            }

            ISet<Expression> visitedExpressions = new HashSet<Expression>();
            ISet<Expression> expression1 = UnionHelper.Iterate(visitedExpressions, Left.AsSequence().Concat(Right)).ToSortedSet();
            visitedExpressions.Clear();
            ISet<Expression> expression2 = UnionHelper.Iterate(visitedExpressions, other.Left.AsSequence().Concat(other.Right)).ToSortedSet();

            return expression1.SequenceCompare(expression2);
        }

        public override bool Equals(object obj)
        {
            BinaryUnion union = obj as BinaryUnion;
            return Equals(union);
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Right.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            BinaryUnion union = other as BinaryUnion;
            return Equals(union);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is BinaryUnion)
            {
                return CompareTo((BinaryUnion)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (DepthData<Expression> data in Left.WalkInternal())
            {
                yield return data;
            }

            foreach (DepthData<Expression> data in Right.WalkInternal())
            {
                yield return data;
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority
        {
            get 
            {
                return 3;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            UnionHelper.ToString(builder, UnionHelper.Iterate(visitedExpressions,Left.AsSequence().Concat(Right)).ToSortedSet().AsReadOnly(), Priority);
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new BinaryUnion(Left.ToRegularSet(), Right.ToRegularSet());
        }

        protected override bool GetIsRegularSet()
        {
            return Left.IsRegularSet && Right.IsRegularSet;
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return EnumerateHelper.Sequence(Left, Right).ToList().AsReadOnly();
        }

        protected override Grammar GenerateGrammar(GrammarType grammarType)
        {
            Grammar leftExpGrammar;
            Grammar rightExpGrammar;

            int index = _StartIndex;

            switch (grammarType)
            {
                case GrammarType.Left:
                    leftExpGrammar = Left.LeftGrammar.Reorganize(index);
                    index += leftExpGrammar.NonTerminals.Count;
                    rightExpGrammar = Right.LeftGrammar.Reorganize(index);
                    index += rightExpGrammar.NonTerminals.Count;
                    break;
                case GrammarType.Right:
                    leftExpGrammar = Left.RightGrammar.Reorganize(index);
                    index += leftExpGrammar.NonTerminals.Count;
                    rightExpGrammar = Right.RightGrammar.Reorganize(index);
                    index += rightExpGrammar.NonTerminals.Count;
                    break;
                default:
                    throw new InvalidOperationException(UnknownGrammarMessage(grammarType));
            }

            NonTerminalSymbol target = new NonTerminalSymbol(new Label(new SingleLabel('S',index++)));

            Rule rule = new Rule(
                EnumerateHelper.Sequence(
                    new Chain(EnumerateHelper.Sequence(leftExpGrammar.Target)),
                    new Chain(EnumerateHelper.Sequence(rightExpGrammar.Target))
                ), target);

            IEnumerable<Rule> newRules = leftExpGrammar.Rules.Concat(rightExpGrammar.Rules).Concat(rule);

            return new Grammar(newRules, target);
        }

        public override Expression Optimize()
        {
            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            return new Union(UnionHelper.Iterate(visitedExpressions,Left.AsSequence().Concat(Right))).Optimize();
        }

        public override bool CanBeEmpty()
        {
            return Left.CanBeEmpty() || Right.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new BinaryUnion(Left.TryToLetItBeEmpty(), Right.TryToLetItBeEmpty());
        }
    }
}
