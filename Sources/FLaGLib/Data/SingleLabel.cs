using FLaGLib.Extensions;
using System;
using System.Globalization;

namespace FLaGLib.Data
{
    public class SingleLabel : IEquatable<SingleLabel>, IComparable<SingleLabel>
    {
        public char Sign
        {
            get;
            private set;
        }

        public int? SignIndex
        {
            get;
            private set;
        }

        public SingleLabel(char sign, int? signIndex = null)
        {
            Sign = sign;
            SignIndex = signIndex;
        }

        public static bool operator ==(SingleLabel objA, SingleLabel objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(SingleLabel objA, SingleLabel objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(SingleLabel objA, SingleLabel objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(SingleLabel objA, SingleLabel objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(SingleLabel objA, SingleLabel objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(SingleLabel objA, SingleLabel objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(SingleLabel objA, SingleLabel objB)
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

        public static int Compare(SingleLabel objA, SingleLabel objB)
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
            SingleLabel singleLabel = obj as SingleLabel;

            return Equals(singleLabel);
        }

        public override int GetHashCode()
        {
            return Sign.GetHashCode() ^ 
                SignIndex.GetHashCodeNullable();
        }

        public bool Equals(SingleLabel other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(Sign, other.Sign) &&
                object.Equals(SignIndex, other.SignIndex);
        }

        public int CompareTo(SingleLabel other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Sign.CompareTo(other.Sign);

            if (result != 0)
            {
                return result;
            }

            return SignIndex.CompareTo(other.SignIndex);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", Sign, 
                (object)SignIndex ?? "null");
        }
    }
}
