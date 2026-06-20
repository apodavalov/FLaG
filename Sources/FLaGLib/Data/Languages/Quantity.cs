using FLaGLib.Data.RegExps;
using RegExpConstIteration = FLaGLib.Data.RegExps.ConstIteration;

namespace FLaGLib.Data.Languages
{
    [ComparableEquatable]
    public sealed partial class Quantity : Exponent
    {
        public int Count { get; private set; }

        public override ExponentType ExponentType => ExponentType.Quantity;

        public Quantity(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("Count must be no less than zero.");
            }

            Count = count;
        }

        public bool EqualsNonnull(Quantity other) => Count.Equals(other.Count);

        public override int GetHashCode() => Count.GetHashCode();

        public int CompareToNonnull(Quantity other) => Count.CompareTo(other.Count);

        public override string ToString() => Count.ToString();

        public override Expression ToRegExp(Entity entity) =>
            new RegExpConstIteration(entity.ToRegExp(), Count);
    }
}
