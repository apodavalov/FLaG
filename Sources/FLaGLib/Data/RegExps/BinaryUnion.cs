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

        public override ExpressionType ExpressionType
        {
            get
            {
                return ExpressionType.BinaryUnion;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            ISet<Expression> visitedExpressions = new HashSet<Expression>();

            UnionHelper.ToString(builder, UnionHelper.Iterate(visitedExpressions,Left.AsSequence().Concat(Right)).ToSortedSet().AsReadOnly(), Priority);
        }

        internal override GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            Grammar leftExpGrammar = dependencies[0].GrammarExpression.Grammar;
            Grammar rightExpGrammar = dependencies[1].GrammarExpression.Grammar;

            NonTerminalSymbol target = new NonTerminalSymbol(new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol,index++)));

            Rule rule = new Rule(
                EnumerateHelper.Sequence(
                    new Chain(EnumerateHelper.Sequence(leftExpGrammar.Target)),
                    new Chain(EnumerateHelper.Sequence(rightExpGrammar.Target))
                ), target);

            IEnumerable<Rule> newRules = leftExpGrammar.Rules.Concat(rightExpGrammar.Rules).Concat(rule);

            GrammarExpressionTuple grammarExpressionTuple = new GrammarExpressionTuple(this, new Grammar(newRules, target), grammarNumber);

            if (onIterate != null)
            {
                onIterate(new GrammarPostReport(grammarExpressionTuple, dependencies));
            }

            return grammarExpressionTuple;
        }

        internal override StateMachineExpressionTuple GenerateStateMachine(int stateMachineNumber, ref int index, ref int additionalStateMachineNumber, Action<StateMachinePostReport> onIterate, params StateMachineExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            StateMachine stateMachine1 = dependencies[0].StateMachineExpression.StateMachine;
            StateMachine stateMachine2 = dependencies[1].StateMachineExpression.StateMachine;

            Label initialState = new Label(new SingleLabel(StateMachine._DefaultStateSymbol, index++));
            
            ISet<Label> finalStates = new HashSet<Label>();

            ISet<Transition> transitions = new HashSet<Transition>();

            if (stateMachine1.FinalStates.Contains(stateMachine1.InitialState) ||
                stateMachine2.FinalStates.Contains(stateMachine2.InitialState))
            {
                finalStates.Add(initialState);
            }

            finalStates.AddRange(stateMachine1.FinalStates);
            finalStates.AddRange(stateMachine2.FinalStates);

            transitions.AddRange(stateMachine1.Transitions);
            transitions.AddRange(stateMachine2.Transitions);

            transitions.AddRange(stateMachine1.Transitions.Where(t => t.CurrentState == stateMachine1.InitialState).Select(t => new Transition(initialState, t.Symbol, t.NextState)));
            transitions.AddRange(stateMachine2.Transitions.Where(t => t.CurrentState == stateMachine2.InitialState).Select(t => new Transition(initialState, t.Symbol, t.NextState)));

            IEnumerable<Label> states = stateMachine1.States.Concat(stateMachine2.States).Concat(initialState);

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
            CheckDependencies(dependencies, 2);
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
