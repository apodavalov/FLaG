namespace FLaG.Core.Helpers
{
    sealed record DepthData<T>(T Value, WalkStatus Status)
    {
        public override string ToString()
        {
            return string.Format("[{0}], [{1}]", Status, Value);
        }
    }
}
