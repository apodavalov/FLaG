using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammar
{
    public abstract class Symbol : IComparable<Symbol>, IEquatable<Symbol>
    {
        public abstract IEnumerable<TerminalSymbol> Alphabet 
        { 
            get; 
        }

        public abstract IEnumerable<NonTerminalSymbol> NonTerminals 
        { 
            get; 
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

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();

        public abstract bool Equals(Symbol other);

        public abstract int CompareTo(Symbol other);
    }
}
