using FLaGLib.Extensions;
using System;
using RegExpTree = FLaGLib.Data.RegExps.Tree;
using RegExpTreeCollection = FLaGLib.Data.RegExps.TreeCollection;

namespace FLaGLib.Data.Languages
{
    public class Tree : IEquatable<Tree>, IComparable<Tree>
    {
        public Entity Entity
        {
            get;
            private set;
        }

        public TreeCollection Subtrees
        {
            get;
            private set;
        }

        public Tree(Entity entity, TreeCollection subtrees = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Entity = entity;
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
            return Entity.GetHashCode() ^ Subtrees.GetHashCodeNullable();
        }

        public bool Equals(Tree other)
        {
            if (other == null)
            {
                return false;
            }

            if (!Entity.Equals(other.Entity))
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

            int result = Entity.CompareTo(other.Entity);

            if (result != 0)
            {
                return result;
            }

            return Subtrees.CompareToNullable(other.Subtrees);
        }

        public RegExpTree ToRegExp()
        {
            RegExpTreeCollection subtrees = null;

            if (Subtrees != null)
            {
                subtrees = Subtrees.ToRegExp();
            }

            return new RegExpTree(Entity.ToRegExp(), subtrees);
        }
    }
}
