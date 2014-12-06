using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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
            int hash = 0;

            foreach (ClassOfEquivalence classOfEquivalence in this)
            {
                hash ^= classOfEquivalence.GetHashCode();
            }

            return hash;
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

            int result;

            IEnumerator<ClassOfEquivalence> classOfEquivalence1 = GetEnumerator();
            IEnumerator<ClassOfEquivalence> classOfEquivalence2 = other.GetEnumerator();

            bool hasNext1 = classOfEquivalence1.MoveNext();
            bool hasNext2 = classOfEquivalence2.MoveNext();

            while (hasNext1 && hasNext2)
            {
                result = classOfEquivalence1.Current.CompareTo(classOfEquivalence2.Current);

                if (result != 0)
                {
                    return result;
                }

                hasNext1 = classOfEquivalence1.MoveNext();
                hasNext2 = classOfEquivalence2.MoveNext();
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
