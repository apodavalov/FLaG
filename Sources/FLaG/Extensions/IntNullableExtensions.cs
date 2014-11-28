using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaG.Extensions
{
   public static class IntNullableExtensions
   {
       public static int CompareTo(this int? objA, int? objB)
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
