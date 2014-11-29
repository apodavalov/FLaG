using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Extensions
{
    public static class IComparableExtensions
    {
        public static int CompareToNullable<T>(this T objA, T objB) where T : IComparable<T>
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
