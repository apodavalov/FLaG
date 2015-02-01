using FLaGLib.Collections;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FLaGLib.Data.Languages
{
    public class Degree : Entity, IEquatable<Degree>, IComparable<Degree>
	{
		public Entity Entity
		{
			get;
			private set;
		}

        public Exponent Exponent
        {
            get;
            private set;
        }

        public Degree(Entity entity, Exponent exponent)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (exponent == null)
            {
                throw new ArgumentNullException("exponent");
            }

            Entity = entity;
            Exponent = exponent;

            _Variables = new Lazy<IReadOnlySet<Variable>>(CollectVariables);
        }

        private IReadOnlySet<Variable> CollectVariables()
        {
            SortedSet<Variable> variables = new SortedSet<Variable>();

            if (Exponent is Variable)
            {
                variables.Add((Variable)Exponent);
            }

            variables.UnionWith(Entity.Variables);

            return variables.AsReadOnly();
        }

        public static bool operator ==(Degree objA, Degree objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Degree objA, Degree objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Degree objA, Degree objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Degree objA, Degree objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Degree objA, Degree objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Degree objA, Degree objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Degree objA, Degree objB)
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

        public static int Compare(Degree objA, Degree objB)
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

        public bool Equals(Degree other)
        {
            if (other == null)
            {
                return false;
            }

            return Entity.Equals(other.Entity) && Exponent.Equals(other.Exponent);
        }

        public int CompareTo(Degree other)
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

            return Exponent.CompareTo(other.Exponent);
        }

        public override bool Equals(object obj)
        {
            Degree degree = obj as Degree;
            return Equals(degree);
        }

        public override int GetHashCode()
        {
            return Entity.GetHashCode() ^ Exponent.GetHashCode();
        }

        public override bool Equals(Entity other)
        {
            Degree degree = other as Degree;
            return Equals(degree);
        }

        public override int CompareTo(Entity other)
        {
            if (other == null || other is Degree)
            {
                return CompareTo((Degree)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        private readonly Lazy<IReadOnlySet<Variable>> _Variables;

        public override IReadOnlySet<Variable> Variables
        {
            get { return _Variables.Value; }
        }

        public override Tree Split()
        {
            return new Tree(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Entity.ToString());

            sb.Append("^(");
            sb.Append(Exponent.ToString());
            sb.Append(')');

            return sb.ToString();
        }

        public override Expression ToRegExp()
        {
            throw new NotImplementedException();
        }
    }
}
