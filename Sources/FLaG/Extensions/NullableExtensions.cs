using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaG.Extensions
{
    public static class NullableExtensions<T> where T : IComparable<T>
    {
        public static int CompareToNullable(this Nullable<T> objA, Nullable<T> objB)
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
