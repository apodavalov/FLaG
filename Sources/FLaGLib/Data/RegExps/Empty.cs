﻿using System;

namespace FLaGLib.Data.RegExps
{
    public class Empty : Expression, IComparable<Empty>, IEquatable<Empty>
    {
        private static Empty _Instance;
        public static Empty Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Empty();
                }

                return _Instance;
            }
        }

        private Empty() { }

        public static bool operator ==(Empty objA, Empty objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Empty objA, Empty objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Empty objA, Empty objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Empty objA, Empty objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Empty objA, Empty objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Empty objA, Empty objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Empty objA, Empty objB)
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

        public static int Compare(Empty objA, Empty objB)
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

        public bool Equals(Empty other)
        {
            if (other == null)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(Empty other)
        {
            if (other == null)
            {
                return 1;
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            Empty empty = obj as Empty;
            return Equals(empty);
        }

        public override int GetHashCode()
        {
            const int randomHashCode = 1565116477;

            return randomHashCode;
        }

        public override bool Equals(Expression other)
        {
            Empty empty = other as Empty;
            return Equals(empty);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Empty)
            {
                return CompareTo((Empty)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}