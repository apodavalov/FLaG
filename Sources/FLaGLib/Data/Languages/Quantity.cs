﻿using FLaGLib.Data.RegExps;
using System;
using RegExpConstIteration = FLaGLib.Data.RegExps.ConstIteration;

namespace FLaGLib.Data.Languages
{
    public class Quantity : Exponent, IEquatable<Quantity>, IComparable<Quantity>
    {
        public int Count
        {
            get;
            private set;
        }

        public override ExponentType ExponentType
        {
            get
            {
                return ExponentType.Quantity;
            }
        }

        public Quantity(int count)
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
            if (other == null)
            {
                return false;
            }

            return Count.Equals(other.Count);            
        }

        public int CompareTo(Quantity other)
        {
            if (other == null)
            {
                return 1;
            }

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

        public override string ToString()
        {
            return Count.ToString();
        }

        public override Expression ToRegExp(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new RegExpConstIteration(entity.ToRegExp(), Count);
        }
    }
}
