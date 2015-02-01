using FLaGLib.Data.RegExps;
using System;

namespace FLaGLib.Data.Languages
{
    public abstract class Exponent : IEquatable<Exponent>, IComparable<Exponent>
    {
        public static bool operator ==(Exponent objA, Exponent objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Exponent objA, Exponent objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Exponent objA, Exponent objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Exponent objA, Exponent objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Exponent objA, Exponent objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Exponent objA, Exponent objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Exponent objA, Exponent objB)
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

        public static int Compare(Exponent objA, Exponent objB)
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

        public abstract bool Equals(Exponent other);

        public abstract int CompareTo(Exponent other);

        public abstract Expression ToRegExp(Entity entity);
    }
}
