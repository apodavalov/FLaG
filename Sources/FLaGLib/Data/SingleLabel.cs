using System.Globalization;

namespace FLaGLib.Data
{
    [ComparableEquatable]
    public sealed partial class SingleLabel(char sign, int? signIndex = null)
    {
        public char Sign { get; } = sign;

        public int? SignIndex { get; } = signIndex;

        public override int GetHashCode() => HashCode.Combine(Sign, SignIndex);

        public bool EqualsNonnull(SingleLabel other) =>
            Sign.Equals(other.Sign) && SignIndex.Equals(other.SignIndex);

        public int CompareToNonnull(SingleLabel other)
        {
            int result = Sign.CompareTo(other.Sign);

            if (result != 0)
            {
                return result;
            }

            if (SignIndex.HasValue && other.SignIndex.HasValue)
            {
                SignIndex.Value.CompareTo(other.SignIndex.Value);
            }

            if (SignIndex.HasValue)
            {
                return 1;
            }

            if (other.SignIndex.HasValue)
            {
                return -1;
            }

            return 0;
        }

        public override string ToString()
        {
            if (SignIndex is null)
            {
                return Sign.ToString();
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", Sign, SignIndex);
        }
    }
}
