using FLaGLib.Extensions;
using System;

namespace FLaGLib.Data.RegExps
{
    public class Tree : IEquatable<Tree>, IComparable<Tree>
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public TreeCollection Subtrees
        {
            get;
            private set;
        }

        public Tree(Expression expression, TreeCollection subtrees = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            Expression = expression;
            Subtrees = subtrees;
        }

        public static bool operator ==(Tree objA, Tree objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Tree objA, Tree objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Tree objA, Tree objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Tree objA, Tree objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Tree objA, Tree objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Tree objA, Tree objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Tree objA, Tree objB)
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

        public static int Compare(Tree objA, Tree objB)
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
            Tree tree = obj as Tree;
            return Equals(tree);
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode() ^ Subtrees.GetHashCodeNullable();
        }

        public bool Equals(Tree other)
        {
            if (other == null)
            {
                return false;
            }

            if (!Expression.Equals(other.Expression))
            {
                return false;
            }

            return Subtrees.EqualsNullable(other.Subtrees);
        }

        public int CompareTo(Tree other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Expression.CompareTo(other.Expression);

            if (result != 0)
            {
                return result;
            }

            return Subtrees.CompareToNullable(other.Subtrees);
        }
    }
}
