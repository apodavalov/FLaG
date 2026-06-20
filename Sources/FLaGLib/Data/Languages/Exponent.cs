using FLaGLib.Data.RegExps;

namespace FLaGLib.Data.Languages
{
    [ComparableEquatable]
    public abstract partial class Exponent
    {
        public abstract Expression ToRegExp(Entity entity);

        public abstract override int GetHashCode();

        public virtual bool EqualsNonnull(Exponent other) =>
            ExponentType.Equals(other.ExponentType);

        public virtual int CompareToNonnull(Exponent other) =>
            ExponentType.CompareTo(other.ExponentType);

        public abstract ExponentType ExponentType { get; }
    }
}
