using FLaGLib.Collections;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegExpBinaryConcat = FLaGLib.Data.RegExps.BinaryConcat;

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
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            
            EntityCollection = new List<Entity>(entities).AsReadOnly();

            if (EntityCollection.AnyNull())
            {
                throw new ArgumentException("There are null items in collection.");
            }

            if (EntityCollection.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }

            _Variables = new Lazy<IReadOnlySet<Variable>>(() => CollectVariables(EntityCollection));
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

        private readonly Lazy<IReadOnlySet<Variable>> _Variables;

        public override IReadOnlySet<Variable> Variables
        {
            get { return _Variables.Value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');

            foreach (Entity entity in EntityCollection)
            {
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
                    expression = new RegExpBinaryConcat(expression, entity.ToRegExp());
                }

                prev = entity;
            }

            return expression;
        }
    }
}

