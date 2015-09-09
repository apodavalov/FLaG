using System;
using System.Globalization;

namespace FLaGLib.Data.StateMachines
{
    public class Transition : IComparable<Transition>, IEquatable<Transition>
    {
        public Label CurrentState
        {
            get;
            private set;
        }

        public Label NextState
        {
            get;
            private set;
        }

        public char Symbol
        {
            get;
            private set;
        }

        public Transition(Label currentState, char symbol, Label nextState)
        {
            if (currentState == null)
            {
                throw new ArgumentNullException(nameof(currentState));
            }

            if (nextState == null)
            {
                throw new ArgumentNullException(nameof(nextState));
            }

            CurrentState = currentState;
            NextState = nextState;
            Symbol = symbol;
        }

        public static bool operator ==(Transition objA, Transition objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Transition objA, Transition objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Transition objA, Transition objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Transition objA, Transition objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Transition objA, Transition objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Transition objA, Transition objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Transition objA, Transition objB)
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

        public static int Compare(Transition objA, Transition objB)
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

        public override bool Equals(object obj)
        {
            Transition transition = obj as Transition;
            return Equals(transition);
        }

        public override int GetHashCode()
        {
            return CurrentState.GetHashCode() ^ NextState.GetHashCode() ^ Symbol.GetHashCode();
        }

        public bool Equals(Transition other)
        {
            if (other == null)
            {
                return false;
            }

            return CurrentState.Equals(other.CurrentState) &&
                   NextState.Equals(other.NextState) &&
                   Symbol.Equals(other.Symbol);
        }

        public int CompareTo(Transition other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = CurrentState.CompareTo(other.CurrentState);

            if (result != 0)
            {
                return result;
            }

            result = Symbol.CompareTo(other.Symbol);

            if (result != 0)
            {
                return result;
            }

            return NextState.CompareTo(other.NextState);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "δ({0},{1}) -> {2}", CurrentState, Symbol, NextState);
        }
    }
}
