﻿using FLaGLib.Collections;
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
    public class Iteration : Expression, IComparable<Iteration>, IEquatable<Iteration>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public bool IsPositive
        {
            get;
            private set;
        }

        public Iteration(Expression expression, bool isPositive)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            Expression = expression;
            IsPositive = isPositive;
        }

        public static bool operator ==(Iteration objA, Iteration objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Iteration objA, Iteration objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Iteration objA, Iteration objB)
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

        public static int Compare(Iteration objA, Iteration objB)
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

        public bool Equals(Iteration other)
        {
            if (other == null)
            {
                return false;
            }

            return Expression.Equals(other.Expression) && IsPositive.Equals(other.IsPositive);
        }

        public int CompareTo(Iteration other)
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

            return IsPositive.CompareTo(other.IsPositive);
        }

        public override bool Equals(object obj)
        {
            Iteration iteration = obj as Iteration;
            return Equals(iteration);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode() ^ IsPositive.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            Iteration iteration = other as Iteration;
            return Equals(iteration);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Iteration)
            {
                return CompareTo((Iteration)other);
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

        public override ExpressionType ExpressionType
        {
            get
            {
                return ExpressionType.Iteration;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            Expression.ToString(Expression.Priority > Priority, builder);

            builder.Append('^');

            if (IsPositive)
            {
                builder.Append("(+)");
            }
            else
            {
                builder.Append("(*)");
            }
        }

        internal override GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            Func<Chain, NonTerminalSymbol, IEnumerable<Chain>> chainEnumerator;
            Grammar expGrammar = dependencies[0].GrammarExpression.Grammar;

            switch (grammarType)
            {
                case GrammarType.Left:
                    chainEnumerator = LeftChainEnumerator;
                    break;
                case GrammarType.Right:
                    chainEnumerator = RightChainEnumerator;
                    break;
                default:
                    throw new InvalidOperationException(UnknownGrammarMessage(grammarType));
            }


            IReadOnlySet<Rule> terminalSymbolsOnlyRules;
            IReadOnlySet<Rule> otherRules;

            expGrammar.SplitRules(out terminalSymbolsOnlyRules, out otherRules);

            ISet<Rule> newRules = new HashSet<Rule>(otherRules);

            foreach (Rule rule in terminalSymbolsOnlyRules)
            {
                ISet<Chain> newChains = new HashSet<Chain>(
                    rule.Chains.SelectMany(chain => chainEnumerator(chain,expGrammar.Target))
                );

                newRules.Add(new Rule(newChains, rule.Target));
            }

            NonTerminalSymbol symbol = new NonTerminalSymbol(new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++)));

            IEnumerable<Chain> chains = EnumerateHelper.Sequence(new Chain(EnumerateHelper.Sequence(expGrammar.Target)));

            if (!IsPositive)
            {
                chains = chains.Concat(new Chain(Enumerable.Empty<Grammars.Symbol>()));
            }

            newRules.Add(new Rule(chains, symbol));

            GrammarExpressionTuple grammarExpressionTuple =
                new GrammarExpressionTuple(
                    this,
                    new Grammar(newRules, symbol),
                    grammarNumber
                );

            if (onIterate != null)
            {
                onIterate(new GrammarPostReport(grammarExpressionTuple, dependencies));
            }

            return grammarExpressionTuple;
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(int stateMachineNumber, ref int index, ref int additionalStateMachineNumber, Action<StateMachinePostReport> onIterate, params StateMachineExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            StateMachine original = dependencies[0].StateMachineExpression.StateMachine;

            Label initialState = new Label(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            ISet<Transition> transitions = original.Transitions.ToHashSet();

            ISet<Label> finalStates = original.FinalStates.ToHashSet();

            if (!IsPositive)
            {
                finalStates.Add(initialState);
            }

            foreach (Transition transition in original.Transitions)
            {
                if (transition.CurrentState == original.InitialState)
                {
                    transitions.AddRange(original.FinalStates.Select(fs => new Transition(fs, transition.Symbol, transition.NextState)));
                    transitions.Add(new Transition(initialState, transition.Symbol, transition.NextState));
                }
            }

            IEnumerable<Label> states = original.States.Concat(initialState);

            StateMachineExpressionTuple stateMachineExpressionTuple =
                new StateMachineExpressionTuple(
                    this,
                    new StateMachine(initialState, finalStates, transitions, states),
                    stateMachineNumber
                );

            if (onIterate != null)
            {
                onIterate(new StateMachinePostReport(stateMachineExpressionTuple, dependencies));
            }

            return stateMachineExpressionTuple;
        }

        private static void CheckDependencies<T>(T[] dependencies)
        {
            CheckDependencies(dependencies, 1);
        }

        private static IEnumerable<Chain> LeftChainEnumerator(Chain chain, NonTerminalSymbol target)
        {
            return EnumerateHelper.Sequence(
                new Chain(
                    EnumerateHelper.Sequence(target).Concat(chain)
                ),
                chain
            );
        }

        private static IEnumerable<Chain> RightChainEnumerator(Chain chain, NonTerminalSymbol target)
        {
            return EnumerateHelper.Sequence(
                new Chain(
                    chain.Concat(EnumerateHelper.Sequence(target))
                ),
                chain
            );
        }

        public override Expression Optimize()
        {
            Expression expression = Expression.Optimize();

            Iteration iteration = expression.As<Iteration>();

            if (iteration != null)
            {
                return new Iteration(iteration.Expression, !iteration.Expression.CanBeEmpty() && IsPositive && iteration.IsPositive);
            }

            Empty empty = expression.As<Empty>();

            if (empty != null)
            {
                return empty;
            }

            if (expression.CanBeEmpty() && IsPositive)
            {
                return new Iteration(expression, false);
            }

            return this;
        }

        public override bool CanBeEmpty()
        {
            if (!IsPositive)
            {
                return true;
            }

            return Expression.CanBeEmpty();
        }

        public override Expression TryToLetItBeEmpty()
        {
            return new Iteration(Expression.TryToLetItBeEmpty(), false);
        }
    }
}
