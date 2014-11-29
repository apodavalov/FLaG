using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Extensions
{
    public static class NullableExtensions 
    {
        public static int CompareToNullable<T>(this Nullable<T> objA, Nullable<T> objB) where T : struct, IComparable<T>
        {
            if (objA.HasValue && objB.HasValue)
            {
                return objA.Value.CompareTo(objB.Value);
            }

            if (objA.HasValue)
            {
                return 1;
            }

            if (objB.HasValue)
            {
                return -1;
            }

            return 0;
        }
    }
}
