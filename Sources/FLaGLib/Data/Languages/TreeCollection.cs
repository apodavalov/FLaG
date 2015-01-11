using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Languages
{
    public class TreeCollection : IEnumerable<Tree>, IEquatable<TreeCollection>, IComparable<TreeCollection>
    {
        private IEnumerable<Tree> _Internal;

        public TreeOperator Operator
        {
            get;
            private set;
        }

        public TreeCollection(IEnumerable<Tree> subtrees, TreeOperator @operator)
        {
            Operator = @operator;

            if (Operator == TreeOperator.Concat)
            {
                _Internal = new List<Tree>(subtrees);
            }
            else
            {
                _Internal = new SortedSet<Tree>(subtrees);
            }

            if (_Internal.Count() < 2)
            {
                throw new ArgumentException("Parameter subtrees must have at least two item.");
            }
        }

        public IEnumerator<Tree> GetEnumerator()
        {
            return _Internal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_Internal).GetEnumerator();
        }

        public static bool operator ==(TreeCollection objA, TreeCollection objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(TreeCollection objA, TreeCollection objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(TreeCollection objA, TreeCollection objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(TreeCollection objA, TreeCollection objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(TreeCollection objA, TreeCollection objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(TreeCollection objA, TreeCollection objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(TreeCollection objA, TreeCollection objB)
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

        public static int Compare(TreeCollection objA, TreeCollection objB)
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
            TreeCollection treeCollection = obj as TreeCollection;
            return Equals(treeCollection);
        }

        public override int GetHashCode()
        {
            return Operator.GetHashCode() ^ _Internal.GetSequenceHashCode();
        }

        public bool Equals(TreeCollection other)
        {
            if (other == null)
            {
                return false;
            }

            if (!Operator.Equals(other.Operator))
            {
                return false;
            }

            return _Internal.SequenceEqual(other._Internal);
        }

        public int CompareTo(TreeCollection other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Operator.CompareTo(other.Operator);

            if (result != 0)
            {
                return result;
            }

            return _Internal.SequenceCompare(other._Internal);
        }
    }
}
