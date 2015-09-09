using FLaGLib.Collections;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegExpBinaryUnion = FLaGLib.Data.RegExps.BinaryUnion;

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
                throw new ArgumentNullException(nameof(entities));
            }

            EntityCollection = new SortedSet<Entity>(entities).AsReadOnly();

            if (EntityCollection.AnyNull())
            {
                throw new ArgumentException("There are null items in collection.");
            }

            if (EntityCollection.Count < 2)
            {
                throw new ArgumentException("Union must have at least two items.");
            }

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

        public override Tree Split()
        {
            if (Variables.Count <= 1)
            {
                return new Tree(this);
            }

            return new Tree(this, new TreeCollection(
                EntityCollection.Select(e => e.Split()).ToList(), 
                TreeOperator.Union));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');

            bool first = true;

            foreach (Entity entity in EntityCollection)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(',');
                }

                sb.Append(entity.ToString());
            }

            sb.Append(')');

            return sb.ToString();
        }

        public override Expression ToRegExp()
        {
            Entity prev = null;
            Expression expression = null;

            foreach (Entity entity in EntityCollection)
            {
                if (prev == null)
                {
                    expression = entity.ToRegExp();
                }
                else
                {
                    expression = new RegExpBinaryUnion(expression, entity.ToRegExp());
                }

                prev = entity;
            }

            return expression;        
        }
    }
}

