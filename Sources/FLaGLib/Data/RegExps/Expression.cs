using System;

namespace FLaGLib.Data.RegExps
{
    public abstract class Expression : IEquatable<Expression>, IComparable<Expression>
    {
        internal Expression() { }

        public static bool operator ==(Expression objA, Expression objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Expression objA, Expression objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Expression objA, Expression objB)
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

        public static int Compare(Expression objA, Expression objB)
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

        public abstract bool Equals(Expression other);

        public abstract int CompareTo(Expression other);
    }
}
