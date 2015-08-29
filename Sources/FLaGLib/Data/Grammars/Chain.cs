using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;
using FLaGLib.Collections;
using System.Collections;

namespace FLaGLib.Data.Grammars
{
    public class Chain : IComparable<Chain>, IEquatable<Chain>, IReadOnlyList<Symbol>
    {
        public IReadOnlyList<Symbol> Sequence
        {
            get;
            private set;
        }

        public IReadOnlySet<TerminalSymbol> Alphabet
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> NonTerminals
        {
            get;
            private set;
        }

        public Chain(IEnumerable<Symbol> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            Sequence = sequence.ToList().AsReadOnly();
            Alphabet = sequence.SelectMany(symbol => symbol.Alphabet).ToSortedSet().AsReadOnly();
            NonTerminals = sequence.SelectMany(symbol => symbol.NonTerminals).ToSortedSet().AsReadOnly();
        }

        public Chain Reorganize(IDictionary<NonTerminalSymbol, NonTerminalSymbol> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            return new Chain(Sequence.Select(symbol => symbol.Reorganize(map)));
        }

        public static bool operator ==(Chain objA, Chain objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Chain objA, Chain objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Chain objA, Chain objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Chain objA, Chain objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Chain objA, Chain objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Chain objA, Chain objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Chain objA, Chain objB)
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

        public static int Compare(Chain objA, Chain objB)
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
            Chain chain = obj as Chain;
            return Equals(chain);
        }

        public override int GetHashCode()
        {
            return Sequence.GetSequenceHashCode();
        }

        public bool Equals(Chain other)
        {
            if (other == null)
            {
                return false;
            }

            return Sequence.SequenceEqual(other.Sequence);
        }

        public int CompareTo(Chain other)
        {
            if (other == null)
            {
                return 1;
            }

            return Sequence.SequenceCompare(other.Sequence);
        }

        public Symbol this[int index]
        {
            get 
            {
                return Sequence[index];
            }
        }

        public int Count
        {
            get
            {
                return Sequence.Count;
            }
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return Sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Sequence.Of<IEnumerable>().GetEnumerator();
        }

        public override string ToString()
        {
            if (Sequence.Count > 0)
            {
                return string.Join<Symbol>(string.Empty, Sequence);
            }
            else
            {
                return "ε";
            }
        }
    }
}
