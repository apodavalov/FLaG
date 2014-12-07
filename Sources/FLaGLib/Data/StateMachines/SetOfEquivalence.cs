using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.StateMachines
{
    public class SetOfEquivalence : ReadOnlySet<Label>, IComparable<SetOfEquivalence>, IEquatable<SetOfEquivalence>
    {
        public SetOfEquivalence(ISet<Label> set) : base(set) { }

        public static bool operator ==(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(SetOfEquivalence objA, SetOfEquivalence objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(SetOfEquivalence objA, SetOfEquivalence objB)
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

        public static int Compare(SetOfEquivalence objA, SetOfEquivalence objB)
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
            SetOfEquivalence setOfEquivalence = obj as SetOfEquivalence;
            return Equals(setOfEquivalence);
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (Label set in this)
            {
                hash ^= set.GetHashCodeNullable();
            }

            return hash;
        }

        public bool Equals(SetOfEquivalence other)
        {
            if (other == null)
            {
                return false;
            }

            return this.SequenceEqual(other);
        }

        public int CompareTo(SetOfEquivalence other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = 0;

            IEnumerator<Label> states1 = GetEnumerator();
            IEnumerator<Label> states2 = other.GetEnumerator();

            bool hasNext1 = states1.MoveNext();
            bool hasNext2 = states2.MoveNext();

            while (hasNext1 && hasNext2)
            {
                result = states1.Current.CompareToNullable(states2.Current);

                if (result != 0)
                {
                    return result;
                }

                hasNext1 = states1.MoveNext();
                hasNext2 = states2.MoveNext();
            }

            if (hasNext1)
            {
                return 1;
            }

            if (hasNext2)
            {
                return -1;
            }

            return 0;
        }
    }
}
