using System;
using System.Collections.Generic;
using FLaGLib.Helpers;
using System.Text;
using System.Linq;
using FLaGLib.Data.Grammars;
using FLaGLib.Collections;

namespace FLaGLib.Data.RegExps
{
    public class BinaryConcat : Expression, IEquatable<BinaryConcat>, IComparable<BinaryConcat>
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

        public BinaryConcat(Expression left, Expression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            Left = left;
            Right = right;
        }

        public static bool operator ==(BinaryConcat objA, BinaryConcat objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(BinaryConcat objA, BinaryConcat objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(BinaryConcat objA, BinaryConcat objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(BinaryConcat objA, BinaryConcat objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(BinaryConcat objA, BinaryConcat objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(BinaryConcat objA, BinaryConcat objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(BinaryConcat objA, BinaryConcat objB)
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

        public static int Compare(BinaryConcat objA, BinaryConcat objB)
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

        public bool Equals(BinaryConcat other)
        {
            if (other == null)
            {
                return false;
            }

            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public int CompareTo(BinaryConcat other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Left.CompareTo(other.Left);

            if (result != 0)
            {
                return result;
            }

            return Right.CompareTo(other.Right);
        }

        public override bool Equals(object obj)
        {
            BinaryConcat concat = obj as BinaryConcat;
            return Equals(concat);
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Right.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            BinaryConcat concat = other as BinaryConcat;
            return Equals(concat);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is BinaryConcat)
            {
                return CompareTo((BinaryConcat)other);
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
                return 2; 
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            Left.ToString(Left.Priority > Priority, builder);
            Right.ToString(Right.Priority >= Priority, builder);
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new BinaryConcat(Left.ToRegularSet(), Right.ToRegularSet());
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
            switch (grammarType)
            {
                case GrammarType.Left:
                    return GenerateLeftGrammar();
                case GrammarType.Right:
                    return GenerateRightGrammar();
                default:
                    throw new InvalidOperationException(UnknownGrammarMessage(grammarType));
            }          
        }

        private Grammar GenerateRightGrammar()
        {
            int index = _StartIndex;

            Grammar leftExpGrammar = Left.RightGrammar.Reorganize(index);
            index += leftExpGrammar.NonTerminals.Count;
            Grammar rightExpGrammar = Right.RightGrammar.Reorganize(index);

            IReadOnlySet<Rule> terminalSymbolsOnlyRules;
            IReadOnlySet<Rule> otherRules;

            leftExpGrammar.SplitRules(out terminalSymbolsOnlyRules, out otherRules);

            ISet<Rule> newRules = new HashSet<Rule>(otherRules.Concat(rightExpGrammar.Rules));

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                ISet<Chain> newChains = new HashSet<Chain>(
                    rule.Chains.Select(
                        chain => new Chain(
                            chain.Concat(EnumerateHelper.Sequence(rightExpGrammar.Target))
                        )
                    )
                );

                newRules.Add(new Rule(newChains, rule.Target));
            }

            return new Grammar(newRules, leftExpGrammar.Target);
        }

        private Grammar GenerateLeftGrammar()
        {
            int index = _StartIndex;

            Grammar leftExpGrammar = Left.LeftGrammar.Reorganize(index);
            index += leftExpGrammar.NonTerminals.Count;
            Grammar rightExpGrammar = Right.LeftGrammar.Reorganize(index);

            IReadOnlySet<Rule> terminalSymbolsOnlyRules;
            IReadOnlySet<Rule> otherRules;

            rightExpGrammar.SplitRules(out terminalSymbolsOnlyRules, out otherRules);

            ISet<Rule> newRules = new HashSet<Rule>(otherRules.Concat(leftExpGrammar.Rules));

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                ISet<Chain> newChains = new HashSet<Chain>(
                    rule.Chains.Select(
                        chain => new Chain(
                            EnumerateHelper.Sequence(leftExpGrammar.Target).Concat(chain)
                        )
                    )
                );

                newRules.Add(new Rule(newChains, rule.Target));
            }

            return new Grammar(newRules, rightExpGrammar.Target);
        }
    }
}
