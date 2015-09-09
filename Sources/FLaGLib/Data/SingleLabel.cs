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

        public int? SubIndex
        {
            get;
            private set;
        }

        public int? SupIndex
        {
            get;
            private set;
        }

        public SingleLabel(char sign, int? signIndex = null, int? supIndex = null, int? subIndex = null)
        {
            Sign = sign;
            SignIndex = signIndex;
            SupIndex = supIndex;
            SubIndex = subIndex;
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
                SignIndex.GetHashCodeNullable() ^ 
                SubIndex.GetHashCodeNullable() ^ 
                SupIndex.GetHashCodeNullable();
        }

        public bool Equals(SingleLabel other)
        {
            if (other == null)
            {
                return false;
            }

            return object.Equals(Sign, other.Sign) &&
                object.Equals(SignIndex, other.SignIndex) &&
                object.Equals(SubIndex, other.SubIndex) &&
                object.Equals(SupIndex, other.SupIndex);
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

            result = SignIndex.CompareTo(other.SignIndex);

            if (result != 0)
            {
                return result;
            }

            result = SupIndex.CompareTo(other.SupIndex);

            if (result != 0)
            {
                return result;
            }

            return SubIndex.CompareTo(other.SubIndex);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}", Sign, 
                (object)SignIndex ?? "null", 
                (object)SupIndex ?? "null", 
                (object)SubIndex ?? "null");
        }

        public SingleLabel Next()
        {
            if (!SubIndex.HasValue)
            {
                throw new InvalidOperationException("Property SubIndex has no value.");
            }
            return new SingleLabel(Sign, SignIndex, SupIndex, SubIndex + 1);
        }
    }
}
