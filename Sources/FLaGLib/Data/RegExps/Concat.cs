using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.RegExps
{
    public class Concat : Expression, IEquatable<Concat>, IComparable<Concat>
    {
        public IReadOnlyList<Expression> Expressions
        {
            get;
            private set;
        }

        public Concat(IEnumerable<Expression> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            Expressions = new List<Expression>(expressions).AsReadOnly();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }
        }

        public static bool operator ==(Concat objA, Concat objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Concat objA, Concat objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Concat objA, Concat objB)
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

        public static int Compare(Concat objA, Concat objB)
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

        public bool Equals(Concat other)
        {
            if (other == null)
            {
                return false;
            }

            return Expressions.SequenceEqual(other.Expressions);
        }

        public int CompareTo(Concat other)
        {
            if (other == null)
            {
                return 1;
            }

            return Expressions.SequenceCompare(other.Expressions);
        }

        public override bool Equals(object obj)
        {
            Concat concat = obj as Concat;
            return Equals(concat);
        }

        public override int GetHashCode()
        {
            return Expressions.GetSequenceHashCode();
        }

        public override bool Equals(Expression other)
        {
            Concat concat = other as Concat;
            return Equals(concat);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Concat)
            {
                return CompareTo((Concat)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
