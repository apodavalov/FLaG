namespace FLaGLib.Extensions
{
    public static class ObjectExtensions
    {
        public static int GetHashCodeNullable(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
