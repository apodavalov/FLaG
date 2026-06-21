namespace FLaG.Core.Helpers
{
    sealed class WalkDataLabel<T>(bool marked, int index, T value)
    {
        public bool Marked { get; set; } = marked;

        public int Index { get; set; } = index;

        public T Value { get; set; } = value;
    }
}
