using FLaGLib.Collections;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using RegExpSymbol = FLaGLib.Data.RegExps.Symbol;

namespace FLaGLib.Data.Languages
{
    public class Symbol : Entity, IEquatable<Symbol>, IComparable<Symbol>
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

        public override bool Equals(Entity other)
        {
            Symbol symbol = other as Symbol;
            return Equals(symbol);
        }

        public override int CompareTo(Entity other)
        {
            if (other == null || other is Symbol)
            {
                return CompareTo((Symbol)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        private readonly Lazy<IReadOnlySet<Variable>> _Variables = 
            new Lazy<IReadOnlySet<Variable>>(() => new SortedSet<Variable>().AsReadOnly());

        public override IReadOnlySet<Variable> Variables
        {
            get { return _Variables.Value; }
        }

        public override string ToString()
        {
            return Character.ToString();
        }

        public override Expression ToRegExp()
        {
            return new RegExpSymbol(Character);
        }
    }
}
