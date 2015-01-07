using System;

namespace FLaGLib.Data.Languages
{
    public class Quantity : Exponent, IEquatable<Quantity>, IComparable<Quantity>
    {
        public int Count
        {
            get;
            private set;
        }

        public Quantity(int count)
            : base()
        {
            if (count < 0)
            {
                throw new ArgumentException("Count must be no less than zero.");
            }

            Count = count;
        }

        public static bool operator ==(Quantity objA, Quantity objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Quantity objA, Quantity objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Quantity objA, Quantity objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Quantity objA, Quantity objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Quantity objA, Quantity objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Quantity objA, Quantity objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Quantity objA, Quantity objB)
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

        public static int Compare(Quantity objA, Quantity objB)
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
            Quantity quantity = obj as Quantity;
            return Equals(quantity);
        }

        public override int GetHashCode()
        {
            return Count.GetHashCode();
        }

        public bool Equals(Quantity other)
        {
            return Count.Equals(other.Count);            
        }

        public int CompareTo(Quantity other)
        {
            return Count.CompareTo(other.Count);
        }

        public override bool Equals(Exponent other)
        {
            Quantity quantity = other as Quantity;
            return Equals(quantity);
        }

        public override int CompareTo(Exponent other)
        {
            if (other == null || other is Quantity)
            {
                return CompareTo((Quantity)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
