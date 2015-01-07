using System;

namespace FLaGLib.Data.Languages
{
    public class Variable : Exponent, IEquatable<Variable>, IComparable<Variable>
    {
        public char Name
        {
            get;
            private set;
        }

        public Variable(char name)
            : base()
        {
            if (name < 'a' || name > 'z')
            {
                throw new ArgumentException("Variable name must be lower latin letter.");
            }

            Name = name;
        }

        public static bool operator ==(Variable objA, Variable objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Variable objA, Variable objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Variable objA, Variable objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Variable objA, Variable objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Variable objA, Variable objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Variable objA, Variable objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Variable objA, Variable objB)
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

        public static int Compare(Variable objA, Variable objB)
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
            Variable variable = obj as Variable;
            return Equals(variable);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(Variable other)
        {
            return Name.Equals(other.Name);            
        }

        public int CompareTo(Variable other)
        {
            return Name.CompareTo(other.Name);
        }

        public override bool Equals(Exponent other)
        {
            Variable variable = other as Variable;
            return Equals(variable);
        }

        public override int CompareTo(Exponent other)
        {
            if (other == null || other is Variable)
            {
                return CompareTo((Variable)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
