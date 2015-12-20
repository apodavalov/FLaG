using FLaGLib.Data.RegExps;
using System;
using RegExpConcat = FLaGLib.Data.RegExps.BinaryConcat;
using RegExpConstIteration = FLaGLib.Data.RegExps.ConstIteration;
using RegExpIteration = FLaGLib.Data.RegExps.Iteration;

namespace FLaGLib.Data.Languages
{
    public class Variable : Exponent, IEquatable<Variable>, IComparable<Variable>
    {
        public char Name
        {
            get;
            private set;
        }

        public Sign Sign
        {
            get;
            private set;
        }

        public int Number
        {
            get;
            private set;
        }

        public override ExponentType ExponentType
        {
            get
            {
                return ExponentType.Variable;
            }
        }

        public Variable(char name, Sign sign, int number)
        {
            if (name < 'a' || name > 'z')
            {
                throw new ArgumentException("Variable name must be lower latin letter.");
            }

            if (number < 0)
            {
                throw new ArgumentException("Number must be greater or equal to zero.");
            }

            Name = name;
            Sign = sign;
            Number = number;
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
            if (other == null)
            {
                return false;
            }

            return Name.Equals(other.Name);            
        }

        public int CompareTo(Variable other)
        {
            if (other == null)
            {
                return 1;
            }

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

        public override string ToString()
        {
            return Name.ToString();
        }

        public override Expression ToRegExp(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            int number = Number;
            Sign sign = Sign;

            if (number > 0 && sign == Sign.MoreThanOrEqual)
            {
                number--;
                sign = Sign.MoreThan;
            }

            if (sign == Sign.MoreThanOrEqual)
            {
                return new RegExpIteration(entity.ToRegExp(), false);
            }

            if (number < 1)
            {
                return new RegExpIteration(entity.ToRegExp(), true);
            }

            Expression expression = entity.ToRegExp();

            Expression left = new RegExpConstIteration(expression, number);
            Expression right = new RegExpIteration(expression, true);

            return new RegExpConcat(left, right);
        }
    }
}
