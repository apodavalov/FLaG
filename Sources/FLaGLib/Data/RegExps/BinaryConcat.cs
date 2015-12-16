using FLaGLib.Collections;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
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

            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Left.AsSequence().Concat(Right));
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Left.AsSequence().Concat(other.Right));

            return expression1.SequenceEqual(expression2);
        }

        public int CompareTo(BinaryConcat other)
        {
            if (other == null)
            {
                return 1;
            }

            IEnumerable<Expression> expression1 = ConcatHelper.Iterate(Left.AsSequence().Concat(Right));
            IEnumerable<Expression> expression2 = ConcatHelper.Iterate(other.Left.AsSequence().Concat(other.Right));

            return expression1.SequenceCompare(expression2);
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
            ConcatHelper.ToString(builder, ConcatHelper.Iterate(Left.AsSequence().Concat(Right)).ToList().AsReadOnly(), Priority);
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

        internal override GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            switch (grammarType)
            {
                case GrammarType.Left:
                    return GenerateLeftGrammar(grammarNumber, ref index, ref additionalGrammarNumber, onIterate, dependencies);
                case GrammarType.Right:
                    return GenerateRightGrammar(grammarNumber, ref index, ref additionalGrammarNumber, onIterate, dependencies);
                default:
                    throw new InvalidOperationException(UnknownGrammarMessage(grammarType));
            }
        }

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 2);
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(int stateMachineNumber, ref int index, ref int additionalStateMachineNumber, Action<StateMachinePostReport> onIterate, params StateMachineExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            StateMachine stateMachine1 = dependencies[0].StateMachineExpression.StateMachine;
            StateMachine stateMachine2 = dependencies[1].StateMachineExpression.StateMachine;

            Label initialState = stateMachine1.InitialState;

            ISet<Label> finalStates = stateMachine2.FinalStates.ToHashSet();

            if (stateMachine2.FinalStates.Contains(stateMachine2.InitialState))
            {
                finalStates.AddRange(stateMachine1.FinalStates);
            }

            ISet<Transition> transitions = stateMachine1.Transitions.ToHashSet();

            foreach (Transition transition in stateMachine2.Transitions)
            {
                if (transition.CurrentState == stateMachine2.InitialState)
                {
                    foreach (Label state in stateMachine1.FinalStates)
                    {
                        transitions.Add(new Transition(state, transition.Symbol, transition.NextState));
                    }
                }

                transitions.Add(transition);
            }

            StateMachineExpressionTuple stateMachineExpressionTuple =
              new StateMachineExpressionTuple(
                  this,
                  new StateMachine(initialState, finalStates, transitions),
                  stateMachineNumber
              );

            if (onIterate != null)
            {
                onIterate(new StateMachinePostReport(stateMachineExpressionTuple, dependencies));
            }

            return stateMachineExpressionTuple;            
        }

        private GrammarExpressionTuple GenerateRightGrammar(int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

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

            GrammarExpressionTuple grammarExpressionTuple = new GrammarExpressionTuple(this, new Grammar(newRules, leftExpGrammar.Target), grammarNumber);

            if (onIterate != null)
            {
                onIterate(new GrammarPostReport(grammarExpressionTuple, dependencies));
            }

            return grammarExpressionTuple;
        }

        private GrammarExpressionTuple GenerateLeftGrammar(int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

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

            GrammarExpressionTuple grammarExpressionTuple = new GrammarExpressionTuple(this, new Grammar(newRules, rightExpGrammar.Target), grammarNumber);

            if (onIterate != null)
            {
                onIterate(new GrammarPostReport(grammarExpressionTuple, dependencies));
            }

            return grammarExpressionTuple;
        }

        public override Expression Optimize()
        {
            return new Concat(ConcatHelper.Iterate(Left.AsSequence().Concat(Right))).Optimize();
        }

        public override bool CanBeEmpty()
        {
            return Left.CanBeEmpty() && Right.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new BinaryConcat(Left.TryToLetItBeEmpty(), Right.TryToLetItBeEmpty());
        }
    }
}
