using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class ClassOfEquivalence : IComparable<ClassOfEquivalence>, IEquatable<ClassOfEquivalence>
    {
        public int SetNum
        {
            get;
            private set;
        }

        public IReadOnlySet<char> Symbols
        {
            get;
            private set;
        }

        public ClassOfEquivalence(int setNum, IReadOnlySet<char> symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            SetNum = setNum;
            Symbols = symbols;
        }

        public static bool operator ==(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(ClassOfEquivalence objA, ClassOfEquivalence objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(ClassOfEquivalence objA, ClassOfEquivalence objB)
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

        public static int Compare(ClassOfEquivalence objA, ClassOfEquivalence objB)
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
            ClassOfEquivalence classOfEquivalence = obj as ClassOfEquivalence;
            return Equals(classOfEquivalence);
        }

        public override int GetHashCode()
        {
            int hash = SetNum.GetHashCode();

            foreach (char symbol in Symbols)
            {
                hash ^= symbol.GetHashCode();
            }

            return hash;
        }

        public bool Equals(ClassOfEquivalence other)
        {
            if (other == null)
            {
                return false;
            }

            if (!SetNum.Equals(other.SetNum))
            {
                return false;
            }

            return Symbols.SequenceEqual(other.Symbols);
        }

        public int CompareTo(ClassOfEquivalence other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = SetNum.CompareTo(other.SetNum);

            if (result != 0)
            {
                return result;
            }

            IEnumerator<char> symbols1 = Symbols.GetEnumerator();
            IEnumerator<char> symbols2 = other.Symbols.GetEnumerator();

            bool hasNext1 = symbols1.MoveNext();
            bool hasNext2 = symbols2.MoveNext();

            while (hasNext1 && hasNext2)
            {
                result = symbols1.Current.CompareTo(symbols2.Current);

                if (result != 0)
                {
                    return result;
                }

                hasNext1 = symbols1.MoveNext();
                hasNext2 = symbols2.MoveNext();
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
