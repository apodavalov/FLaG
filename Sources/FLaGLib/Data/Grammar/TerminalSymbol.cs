using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Helpers;

namespace FLaGLib.Data.Grammar
{
    public class TerminalSymbol : Symbol, IComparable<TerminalSymbol>, IEquatable<TerminalSymbol>
    {
        public char Symbol
        {
            get;
            private set;
        }

        public TerminalSymbol(char symbol)
        {
            Symbol = symbol;
        }

        public override IEnumerable<TerminalSymbol> Alphabet
        {
            get 
            {
                return EnumerateHelper.Sequence(this);
            }
        }

        public override IEnumerable<NonTerminalSymbol> NonTerminals
        {
            get 
            {
                return Enumerable.Empty<NonTerminalSymbol>();
            }
        }

        public static bool operator ==(TerminalSymbol objA, TerminalSymbol objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(TerminalSymbol objA, TerminalSymbol objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(TerminalSymbol objA, TerminalSymbol objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(TerminalSymbol objA, TerminalSymbol objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(TerminalSymbol objA, TerminalSymbol objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(TerminalSymbol objA, TerminalSymbol objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(TerminalSymbol objA, TerminalSymbol objB)
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

        public static int Compare(TerminalSymbol objA, TerminalSymbol objB)
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

        public bool Equals(TerminalSymbol other)
        {
            if (other == null)
            {
                return false;
            }

            return Symbol.Equals(other.Symbol);
        }

        public int CompareTo(TerminalSymbol other)
        {
            if (other == null)
            {
                return 1;
            }

            return Symbol.CompareTo(other.Symbol);
        }

        public override bool Equals(object obj)
        {
            TerminalSymbol terminalSymbol = obj as TerminalSymbol;
            return Equals(terminalSymbol);
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public override bool Equals(Symbol other)
        {
            TerminalSymbol terminalSymbol = other as TerminalSymbol;
            return Equals(terminalSymbol);
        }

        public override int CompareTo(Symbol other)
        {
            if (other == null || other is TerminalSymbol)
            {
                return CompareTo((TerminalSymbol)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
