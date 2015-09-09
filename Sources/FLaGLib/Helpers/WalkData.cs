using System;

namespace FLaGLib.Helpers
{
    public class WalkData<T> : IEquatable<WalkData<T>>, IComparable<WalkData<T>> where T : IEquatable<T>, IComparable<T>
    {
        public WalkStatus Status
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public T Value
        {
            get;
            private set;
        }


        public static bool operator ==(WalkData<T> objA, WalkData<T> objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(WalkData<T> objA, WalkData<T> objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(WalkData<T> objA, WalkData<T> objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(WalkData<T> objA, WalkData<T> objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(WalkData<T> objA, WalkData<T> objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(WalkData<T> objA, WalkData<T> objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(WalkData<T> objA, WalkData<T> objB)
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

        public static int Compare(WalkData<T> objA, WalkData<T> objB)
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
            WalkData<T> walkData = obj as WalkData<T>;
            return Equals(walkData);
        }

        public override int GetHashCode()
        {
            return
                Status.GetHashCode() ^
                Index.GetHashCode() ^
                Value.GetHashCode();
        }

        public bool Equals(WalkData<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Status.Equals(other.Status) &&
                Index.Equals(other.Index) &&
                Value.Equals(other.Value);
        }

        public int CompareTo(WalkData<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Status.CompareTo(other.Status);

            if (result != 0)
            {
                return result;
            }

            result = Index.CompareTo(other.Index);

            if (result != 0)
            {
                return result;
            }

            return Value.CompareTo(other.Value);
        }

        public WalkData(WalkStatus status, int index, T value)
        {
            Status = status;
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("[{0}], [{1}], [{2}]",Status,Index,Value);
        }
    }
}
