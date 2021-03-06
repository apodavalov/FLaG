﻿using FLaGLib.Data.Grammars;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class Empty : Expression, IComparable<Empty>, IEquatable<Empty>
    {
        private static Empty _Instance;
        public static Empty Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Empty();
                }

                return _Instance;
            }
        }

        private Empty() { }

        public static bool operator ==(Empty objA, Empty objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Empty objA, Empty objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Empty objA, Empty objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Empty objA, Empty objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Empty objA, Empty objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Empty objA, Empty objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Empty objA, Empty objB)
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

        public static int Compare(Empty objA, Empty objB)
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

        public bool Equals(Empty other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(Empty other)
        {
            if (other == null)
            {
                return 1;
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            Empty empty = obj as Empty;
            return Equals(empty);
        }

        public override int GetHashCode()
        {
            const int randomHashCode = 1565116477;

            return randomHashCode;
        }

        public override bool Equals(Expression other)
        {
            Empty empty = other as Empty;
            return Equals(empty);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Empty)
            {
                return CompareTo((Empty)other);
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

        public override ExpressionType ExpressionType
        {
            get
            {
                return ExpressionType.Empty;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            builder.Append('ε');
        }

        internal override GrammarExpressionTuple GenerateGrammar(GrammarType grammarType, int grammarNumber,
            ref int index, ref int additionalGrammarNumber, Action<GrammarPostReport> onIterate, params GrammarExpressionWithOriginal[] dependencies)
        {
            CheckDependencies(dependencies);

            NonTerminalSymbol target = new NonTerminalSymbol(new Label(new SingleLabel(Grammar._DefaultNonTerminalSymbol, index++)));

            GrammarExpressionTuple grammarExpressionTuple =
                new GrammarExpressionTuple(
                    this,
                    new Grammar(
                        EnumerateHelper.Sequence(
                            new Rule(EnumerateHelper.Sequence(new Chain(Enumerable.Empty<Grammars.Symbol>())), target)
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
            CheckDependencies(dependencies);

            Label state = new Label(new SingleLabel(StateMachine._DefaultStateSymbol, index++));

            StateMachineExpressionTuple stateMachineExpressionTuple =
                new StateMachineExpressionTuple(
                    this,
                    new StateMachine(state, state.AsSequence(), Enumerable.Empty<Transition>()),
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
            CheckDependencies(dependencies, 0);
        }

        public override Expression Optimize()
        {
            return this;
        }

        public override bool CanBeEmpty()
        {
            return true;
        }

        public override Expression TryToLetItBeEmpty()
        {
            return this;
        }
    }
}
