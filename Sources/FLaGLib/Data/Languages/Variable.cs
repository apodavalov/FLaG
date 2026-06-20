using FLaGLib.Data.RegExps;
using RegExpConcat = FLaGLib.Data.RegExps.BinaryConcat;
using RegExpConstIteration = FLaGLib.Data.RegExps.ConstIteration;
using RegExpIteration = FLaGLib.Data.RegExps.Iteration;

namespace FLaGLib.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Variable : Exponent
    {
        public char Name { get; }

        public Sign Sign { get; }

        public int Number { get; }

        public override ExponentType ExponentType => ExponentType.Variable;

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

        public override int GetHashCode() => Name.GetHashCode();

        public bool EqualsNonnull(Variable other) => Name.Equals(other.Name);

        public int CompareToNonnull(Variable other) => Name.CompareTo(other.Name);

        public override string ToString() => Name.ToString();

        public override Expression ToRegExp(Entity entity)
        {
            int number = Number;
            Sign sign = Sign;

            if (number > 0 && sign == Sign.MoreThanOrEqual)
            {
                --number;
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
