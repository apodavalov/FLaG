using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FLaGLib.Data.Languages
{
    public abstract class Entity : IEquatable<Entity>, IComparable<Entity>
    {   
        public static bool operator ==(Entity objA, Entity objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Entity objA, Entity objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Entity objA, Entity objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Entity objA, Entity objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Entity objA, Entity objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Entity objA, Entity objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Entity objA, Entity objB)
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

        public static int Compare(Entity objA, Entity objB)
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

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();

        public abstract bool Equals(Entity other);

        public abstract int CompareTo(Entity other);

        public abstract IReadOnlySet<VariableLink> VariableLinks { get; }

        protected IReadOnlySet<VariableLink> CollectVariableLinks(IEnumerable<Entity> entities)
        {
            return new SortedSet<VariableLink>(entities.SelectMany(entity => entity.VariableLinks)).AsReadOnly();
        }
    }
}
