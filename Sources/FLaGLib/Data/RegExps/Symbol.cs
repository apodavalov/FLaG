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
    public class Symbol : Expression, IComparable<Symbol>, IEquatable<Symbol>
    {
        public char Character
        {
            get;
            private set;
        }

        public Symbol(char character)
        {
            Character = character;
        }

        public static bool operator ==(Symbol objA, Symbol objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Symbol objA, Symbol objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Symbol objA, Symbol objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Symbol objA, Symbol objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Symbol objA, Symbol objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Symbol objA, Symbol objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Symbol objA, Symbol objB)
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

        public static int Compare(Symbol objA, Symbol objB)
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

        public bool Equals(Symbol other)
        {
            if (other == null)
            {
                return false;
            }

            return Character.Equals(other.Character);
        }

        public int CompareTo(Symbol other)
        {
            if (other == null)
            {
                return 1;
            }

            return Character.CompareTo(other.Character);
        }

        public override bool Equals(object obj)
        {
            Symbol symbol = obj as Symbol;
            return Equals(symbol);
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            Symbol symbol = other as Symbol;
            return Equals(symbol);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Symbol)
            {
                return CompareTo((Symbol)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);
            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority
        {
            get 
            {
                return 0;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            builder.Append(Character);
        }

        public override Expression ToRegularSet()
        {
            return this;
        }

        protected override bool GetIsRegularSet()
        {
            return true;
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return Enumerable.Empty<Expression>().ToList().AsReadOnly();
        }

        internal override GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            if (dependencies.Length != 0)
            {
                throw new InvalidOperationException("Expected exactly 0 dependencies.");
            }

            NonTerminalSymbol target = new NonTerminalSymbol(new Label(new SingleLabel('S', index++)));
            TerminalSymbol symbol = new TerminalSymbol(Character);

            GrammarExpressionTuple grammarExpressionTuple =
                new GrammarExpressionTuple(
                    this,
                    new Grammar(
                        EnumerateHelper.Sequence(
                            new Rule(EnumerateHelper.Sequence(new Chain(EnumerateHelper.Sequence(symbol))), target)
                        ),
                        target
                    ),
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
            if (dependencies.Length != 0)
            {
                throw new InvalidOperationException("Expected exactly 0 dependencies.");
            }

            Label currentState = new Label(new SingleLabel('S', index++));
            Label nextState = new Label(new SingleLabel('S', index++));

            StateMachineExpressionTuple stateMachineExpressionTuple =
                new StateMachineExpressionTuple(
                    this,
                    new StateMachine(currentState, nextState.AsSequence(), new Transition(currentState, Character, nextState).AsSequence()),
                    stateMachineNumber
                );
            
            if (onIterate != null)
            {
                onIterate(new StateMachinePostReport(stateMachineExpressionTuple, dependencies));
            }

            return stateMachineExpressionTuple;
        }

        public override Expression Optimize()
        {
            return this;
        }

        public override bool CanBeEmpty()
        {
            return false;
        }

        public override Expression TryToLetItBeEmpty()
        {
            return this;
        }
    }
}
