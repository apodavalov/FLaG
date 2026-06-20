namespace FLaGLib.Data.StateMachines
{
    internal class LabelSetEqualityComparer : IEqualityComparer<SortedSet<Label>>
    {
        private LabelSetEqualityComparer() { }

        private static LabelSetEqualityComparer? _Instance = null;
        public static LabelSetEqualityComparer Instance =>
            _Instance ??= new LabelSetEqualityComparer();

        public bool Equals(SortedSet<Label>? x, SortedSet<Label>? y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        public int GetHashCode(SortedSet<Label> obj)
        {
            HashCode hashCode = new();
            foreach (Label label in obj)
            {
                hashCode.Add(label);
            }
            return hashCode.ToHashCode();
        }
    }
}
