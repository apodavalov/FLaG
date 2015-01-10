using FLaGLib.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Languages
{
	public class Union : Entity, IEquatable<Union>, IComparable<Union>
	{
        public IReadOnlySet<Entity> EntityCollection
        {
            get;
            private set;
        }

        public Union(IEnumerable<Entity> entities) 
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            EntityCollection = new SortedSet<Entity>(entities).AsReadOnly();

            _Variables = new Lazy<IReadOnlySet<Variable>>(() => CollectVariables(EntityCollection));
        }

        public static bool operator ==(Union objA, Union objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Union objA, Union objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Union objA, Union objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Union objA, Union objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Union objA, Union objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Union objA, Union objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Union objA, Union objB)
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

        public static int Compare(Union objA, Union objB)
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

        public bool Equals(Union other)
        {
            if (other == null)
            {
                return false;
            }

            return EntityCollection.SequenceEqual(other.EntityCollection);
        }

        public int CompareTo(Union other)
        {
            if (other == null)
            {
                return 1;
            }

            return EntityCollection.SequenceCompare(other.EntityCollection);
        }

        public override bool Equals(object obj)
        {
            Union union = obj as Union;
            return Equals(union);
        }

        public override int GetHashCode()
        {
            return EntityCollection.GetSequenceHashCode();
        }

        public override bool Equals(Entity other)
        {
            Union union = other as Union;
            return Equals(union);
        }

        public override int CompareTo(Entity other)
        {
            if (other == null || other is Union)
            {
                return CompareTo((Union)other);
            }

            return string.Compare(GetType().FullName,other.GetType().FullName);
        }

        private readonly Lazy<IReadOnlySet<Variable>> _Variables;

        public override IReadOnlySet<Variable> Variables
        {
            get { return _Variables.Value; }
        }
    }
}

