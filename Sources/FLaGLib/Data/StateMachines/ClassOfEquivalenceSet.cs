using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.StateMachines
{
    public class ClassOfEquivalenceSet : ReadOnlySet<ClassOfEquivalence>, IComparable<ClassOfEquivalenceSet>, IEquatable<ClassOfEquivalenceSet>
    {
        public ClassOfEquivalenceSet(ISet<ClassOfEquivalence> set) : base(set) { }

        public static bool operator ==(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
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

        public static int Compare(ClassOfEquivalenceSet objA, ClassOfEquivalenceSet objB)
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
            ClassOfEquivalenceSet classOfEquivalenceSet = obj as ClassOfEquivalenceSet;
            return Equals(classOfEquivalenceSet);
        }

        public override int GetHashCode()
        {
            return this.GetSequenceHashCode();
        }

        public bool Equals(ClassOfEquivalenceSet other)
        {
            if (other == null)
            {
                return false;
            }

            return this.SequenceEqual(other);
        }

        public int CompareTo(ClassOfEquivalenceSet other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.SequenceCompare(other);
        }
    }
}
