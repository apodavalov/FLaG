using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class SetOfEquivalence : ReadOnlySet<Label>, IComparable<SetOfEquivalence>, IEquatable<SetOfEquivalence>
    {
        public IReadOnlyList<SetOfEquivalenceTransition> Transitions
        {
            get;
            private set;
        }

        public SetOfEquivalence(IEnumerable<Label> set) : this(set, Enumerable.Empty<SetOfEquivalenceTransition>()) { }

        public SetOfEquivalence(IEnumerable<Label> set, IEnumerable<SetOfEquivalenceTransition> transitions)
            : base(set?.ToSortedSet())
        {
            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            Transitions = transitions.ToList().AsReadOnly();
        }

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
            return this.GetSequenceHashCode();
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
            return this.SequenceCompare(other);
        }
    }
}
