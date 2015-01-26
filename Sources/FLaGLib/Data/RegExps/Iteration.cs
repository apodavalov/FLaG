using System;

namespace FLaGLib.Data.RegExps
{
    public class Iteration : Expression, IComparable<Iteration>, IEquatable<Iteration>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public bool IsPositive
        {
            get;
            private set;
        }

        public Iteration(Expression expression, bool isPositive)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            Expression = expression;
            IsPositive = isPositive;
        }

        public static bool operator ==(Iteration objA, Iteration objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Iteration objA, Iteration objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Iteration objA, Iteration objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Iteration objA, Iteration objB)
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

        public static int Compare(Iteration objA, Iteration objB)
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

        public bool Equals(Iteration other)
        {
            if (other == null)
            {
                return false;
            }

            return Expression.Equals(other.Expression) && IsPositive.Equals(other.IsPositive);
        }

        public int CompareTo(Iteration other)
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

            return IsPositive.CompareTo(other.IsPositive);
        }

        public override bool Equals(object obj)
        {
            Iteration iteration = obj as Iteration;
            return Equals(iteration);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode() ^ IsPositive.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            Iteration iteration = other as Iteration;
            return Equals(iteration);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Iteration)
            {
                return CompareTo((Iteration)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
