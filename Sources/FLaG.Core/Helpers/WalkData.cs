namespace FLaG.Core.Helpers
{
    [ComparableEquatable]
    public sealed partial class WalkData<T>(WalkStatus status, int index, T value)
        where T : IEquatable<T>, IComparable<T>
    {
        public WalkStatus Status { get; } = status;

        public int Index { get; } = index;

        public T Value { get; } = value;

        public override int GetHashCode() => HashCode.Combine(Status, Index, Value);

        public bool EqualsNonnull(WalkData<T> other) =>
            Status.Equals(other.Status) && Index.Equals(other.Index) && Value.Equals(other.Value);

        public int CompareToNonnull(WalkData<T> other)
        {
            int result = Status.CompareTo(other.Status);

            if (result != 0)
            {
                return result;
            }

            result = Index.CompareTo(other.Index);

            if (result != 0)
            {
                return result;
            }

            return Value.CompareTo(other.Value);
        }

        public override string ToString() =>
            string.Format("[{0}], [{1}], [{2}]", Status, Index, Value);
    }
}
