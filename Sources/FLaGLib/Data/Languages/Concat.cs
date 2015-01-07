using System;
using System.Linq;
using System.Collections.Generic;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Languages
{
	public class Concat : Entity, IEquatable<Concat>, IComparable<Concat>
	{
        public IReadOnlyList<Entity> EntityCollection
        {
            get;
            private set;
        }

        public Concat(IEnumerable<Entity> entities) 
            : base()
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            EntityCollection = new List<Entity>(entities).AsReadOnly();
        }

        public static bool operator ==(Concat objA, Concat objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Concat objA, Concat objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Concat objA, Concat objB)
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

        public static int Compare(Concat objA, Concat objB)
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

        public bool Equals(Concat other)
        {
            if (other == null)
            {
                return false;
            }

            return EntityCollection.SequenceEqual(other.EntityCollection);
        }

        public int CompareTo(Concat other)
        {
            if (other == null)
            {
                return 1;
            }

            return EntityCollection.SequenceCompare(other.EntityCollection);
        }

        public override bool Equals(object obj)
        {
            Concat concat = obj as Concat;
            return Equals(concat);
        }

        public override int GetHashCode()
        {
            return EntityCollection.GetSequenceHashCode();
        }

        public override bool Equals(Entity other)
        {
            Concat concat = other as Concat;
            return Equals(concat);
        }

        public override int CompareTo(Entity other)
        {
            if (other == null || other is Concat)
            {
                return CompareTo((Concat)other);
            }

            return string.Compare(GetType().FullName,other.GetType().FullName);
        }
    }
}

