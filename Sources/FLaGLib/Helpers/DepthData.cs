namespace FLaGLib.Helpers
{
    class DepthData<T>
    {
        public WalkStatus Status
        {
            get;
            private set;
        }
 
        public T Value
        {
            get;
            private set;
        }

        public DepthData(T value, WalkStatus walkStatus)
        {
            Value = value;
            Status = walkStatus;
        }

        public override string ToString()
        {
            return string.Format("[{0}], [{1}]", Status, Value);
        }
    }
}
