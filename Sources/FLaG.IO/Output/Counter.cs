namespace FLaG.IO.Output
{
    public class Counter
    {
        private int _CurrentValue = 0;

        public int Next()
        {
            return _CurrentValue++;
        }
    }
}
