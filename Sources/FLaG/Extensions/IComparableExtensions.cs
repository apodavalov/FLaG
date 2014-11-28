using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaG.Extensions
{
    public static class IComparableExtensions<T> where T : IComparable<T>
    {
        public static int CompareToNullable(this T objA, T objB)
        {
            if (objA != null && objB != null)
            {
                return objA.CompareTo(objB);
            }
            
            if (objA != null)
            {
                return 1;
            }

            if (objB != null)
            {
                return -1;
            }

            return 0;
        }
    }
}
