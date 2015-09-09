using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Grammars
{
    public class NonTerminalSymbol : Symbol, IComparable<NonTerminalSymbol>, IEquatable<NonTerminalSymbol>
    {
        public Label Label
        {
            get;
            private set;
        }

        public override IEnumerable<TerminalSymbol> Alphabet
        {
            get 
            {
                return Enumerable.Empty<TerminalSymbol>();
            }
        }

        public override IEnumerable<NonTerminalSymbol> NonTerminals
        {
            get 
            {
                return EnumerateHelper.Sequence(this);
            }
        }

        public NonTerminalSymbol(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            Label = label;
        }

        public static bool operator ==(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(NonTerminalSymbol objA, NonTerminalSymbol objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(NonTerminalSymbol objA, NonTerminalSymbol objB)
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

        public static int Compare(NonTerminalSymbol objA, NonTerminalSymbol objB)
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

        public bool Equals(NonTerminalSymbol other)
        {
            if (other == null)
            {
                return false;
            }

            return Label.Equals(other.Label);
        }

        public int CompareTo(NonTerminalSymbol other)
        {
            if (other == null)
            {
                return 1;
            }

            return Label.CompareTo(other.Label);
        }

        public override bool Equals(object obj)
        {
            NonTerminalSymbol nonTerminalSymbol = obj as NonTerminalSymbol;
            return Equals(nonTerminalSymbol);
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public override bool Equals(Symbol other)
        {
            NonTerminalSymbol nonTerminalSymbol = other as NonTerminalSymbol;
            return Equals(nonTerminalSymbol);
        }

        public override int CompareTo(Symbol other)
        {
            if (other == null || other is NonTerminalSymbol)
            {
                return CompareTo((NonTerminalSymbol)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        public override Symbol Reorganize(IDictionary<NonTerminalSymbol, NonTerminalSymbol> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            return map.ValueOrDefault(this, this);
        }

        public override SymbolType SymbolType
        {
            get
            {
                return SymbolType.NonTerminal;
            }
        }

        public override string ToString()
        {
            return Label.ToString();
        }
    }
}
