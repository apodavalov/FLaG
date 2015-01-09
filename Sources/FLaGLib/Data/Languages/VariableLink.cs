using System;

namespace FLaGLib.Data.Languages
{
    public class VariableLink : Exponent, IEquatable<VariableLink>, IComparable<VariableLink>
    {
        public char Name
        {
            get;
            private set;
        }

        public VariableLink(char name)
        {
            if (name < 'a' || name > 'z')
            {
                throw new ArgumentException("Variable name must be lower latin letter.");
            }

            Name = name;
        }

        public static bool operator ==(VariableLink objA, VariableLink objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(VariableLink objA, VariableLink objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(VariableLink objA, VariableLink objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(VariableLink objA, VariableLink objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(VariableLink objA, VariableLink objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(VariableLink objA, VariableLink objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(VariableLink objA, VariableLink objB)
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

        public static int Compare(VariableLink objA, VariableLink objB)
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
            VariableLink variableLink = obj as VariableLink;
            return Equals(variableLink);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(VariableLink other)
        {
            return Name.Equals(other.Name);            
        }

        public int CompareTo(VariableLink other)
        {
            return Name.CompareTo(other.Name);
        }

        public override bool Equals(Exponent other)
        {
            VariableLink variable = other as VariableLink;
            return Equals(variable);
        }

        public override int CompareTo(Exponent other)
        {
            if (other == null || other is VariableLink)
            {
                return CompareTo((VariableLink)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}
