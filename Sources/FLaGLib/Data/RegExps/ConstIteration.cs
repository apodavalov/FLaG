using System;

namespace FLaGLib.Data.RegExps
{
    public class ConstIteration : Expression, IEquatable<ConstIteration>, IComparable<ConstIteration>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public int Count
        {
            get;
            private set;
        }

        public ConstIteration(Expression expression, int count)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (count < 0)
            {
                throw new ArgumentException("Count must be no less than zero.");
            }

            Expression = expression;
            Count = count;
        }

        public static bool operator ==(ConstIteration objA, ConstIteration objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(ConstIteration objA, ConstIteration objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(ConstIteration objA, ConstIteration objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(ConstIteration objA, ConstIteration objB)
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

        public static int Compare(ConstIteration objA, ConstIteration objB)
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

        public bool Equals(ConstIteration other)
        {
            if (other == null)
            {
                return false;
            }

            return Expression.Equals(other.Expression) && Count.Equals(other.Count);
        }

        public int CompareTo(ConstIteration other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Expression.CompareTo(other.Expression);

            if (result != 0)
            {
                return result;
            }

            return Count.CompareTo(other.Count);
        }

        public override bool Equals(object obj)
        {
            ConstIteration constIteration = obj as ConstIteration;
            return Equals(constIteration);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode() ^ Count.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            ConstIteration constIteration = other as ConstIteration;
            return Equals(constIteration);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is ConstIteration)
            {
                return CompareTo((ConstIteration)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
