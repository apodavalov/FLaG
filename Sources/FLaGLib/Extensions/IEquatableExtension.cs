using System;

namespace FLaGLib.Extensions
{
    public static class IEquatableExtension
    {
        public static bool EqualsNullable<T>(this T objA, T objB) where T : IEquatable<T>
        {
            if (objA != null && objB != null)
            {
                return objA.Equals(objB);
            }

            if (objA != null || objB != null)
            {
                return false;
            }

            return true;
        }
    }
}
