using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class SetsOfEquivalence : ReadOnlyList<SetOfEquivalence>, IComparable<SetsOfEquivalence>, IEquatable<SetsOfEquivalence>
    {
        public SetsOfEquivalence(IEnumerable<SetOfEquivalence> set) : base(set?.OrderBy(c => c).ToList()) { }

        public static bool operator ==(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(SetsOfEquivalence objA, SetsOfEquivalence objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(SetsOfEquivalence objA, SetsOfEquivalence objB)
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

        public static int Compare(SetsOfEquivalence objA, SetsOfEquivalence objB)
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
            SetsOfEquivalence setsOfEquivalence = obj as SetsOfEquivalence;
            return Equals(setsOfEquivalence);
        }

        public override int GetHashCode()
        {
            return this.GetSequenceHashCode();
        }

        public bool Equals(SetsOfEquivalence other)
        {
            if (other == null)
            {
                return false;
            }

            return this.SequenceEqual(other);
        }

        public int CompareTo(SetsOfEquivalence other)
        {
            return this.SequenceCompare(other);
        }
    }
}
