using System;
using System.Collections.Generic;
using FLaGLib.Helpers;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class BinaryUnion : Expression, IEquatable<BinaryUnion>, IComparable<BinaryUnion>
    {
        public Expression Left
        {
            get;
            private set;
        }

        public Expression Right
        {
            get;
            private set;
        }

        public BinaryUnion(Expression left, Expression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            Left = left;
            Right = right;
        }

         public static bool operator ==(BinaryUnion objA, BinaryUnion objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(BinaryUnion objA, BinaryUnion objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(BinaryUnion objA, BinaryUnion objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(BinaryUnion objA, BinaryUnion objB)
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

        public static int Compare(BinaryUnion objA, BinaryUnion objB)
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

        public bool Equals(BinaryUnion other)
        {
            if (other == null)
            {
                return false;
            }

            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public int CompareTo(BinaryUnion other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Left.CompareTo(other.Left);

            if (result != 0)
            {
                return result;
            }

            return Right.CompareTo(other.Right);
        }

        public override bool Equals(object obj)
        {
            BinaryUnion union = obj as BinaryUnion;
            return Equals(union);
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Right.GetHashCode();
        }

        public override bool Equals(Expression other)
        {
            BinaryUnion union = other as BinaryUnion;
            return Equals(union);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is BinaryUnion)
            {
                return CompareTo((BinaryUnion)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (DepthData<Expression> data in Left.WalkInternal())
            {
                yield return data;
            }

            foreach (DepthData<Expression> data in Right.WalkInternal())
            {
                yield return data;
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        public override int Priority
        {
            get 
            {
                return 3;
            }
        }

        internal override void ToString(StringBuilder builder)
        {
            Left.ToString(Left.Priority > Priority, builder);
            builder.Append(" + ");
            Right.ToString(Right.Priority >= Priority, builder);
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new BinaryUnion(Left.ToRegularSet(), Right.ToRegularSet());
        }

        protected override bool GetIsRegularSet()
        {
            return Left.IsRegularSet && Right.IsRegularSet;
        }
    }
}
