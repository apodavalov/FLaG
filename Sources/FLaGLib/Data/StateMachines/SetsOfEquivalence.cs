using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;
using System.Collections.ObjectModel;
using FLaGLib.Collections;

namespace FLaGLib.Data.StateMachines
{
    public class SetsOfEquivalence : ReadOnlySet<SetOfEquivalence>, IComparable<SetsOfEquivalence>, IEquatable<SetsOfEquivalence>
    {
        internal SetsOfEquivalence(ISet<SetOfEquivalence> set) : base(set) { }

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
            int hash = 0;

            foreach (SetOfEquivalence set in this)
            {
                hash ^= set.GetHashCodeNullable();
            }

            return hash;
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
            if (other == null)
            {
                return 1;
            }

            int result = 0;

            IEnumerator<SetOfEquivalence> states1 = GetEnumerator();
            IEnumerator<SetOfEquivalence> states2 = other.GetEnumerator();

            bool hasNext1 = states1.MoveNext();
            bool hasNext2 = states2.MoveNext();

            while (hasNext1 && hasNext2)
            {
                result = states1.Current.CompareTo(states2.Current);

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

        internal IDictionary<Label, int> GetStatesMap()
        {
            return this.SelectMany((set, index) => set.Select(item => new Tuple<Label, int>(item, index))).
                    ToDictionary(item => item.Item1, item => item.Item2);
        }

        internal IDictionary<Label, Label> GetOldNewStatesMap()
        {
            return this.SelectMany(item => item.Select(subitem => new Tuple<Label,Label>(subitem, item.First()))).
               ToDictionary(item => item.Item1, item => item.Item2);
        }
    }
}
