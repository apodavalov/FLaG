﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Extensions
{
    public static class ISetExtensions
    {
        public static IReadOnlyList<T> ConvertToReadOnlyListAndSort<T>(this ISet<T> obj) where T : IComparable<T>
        {
            if (obj == null)
            {
                throw new ArgumentNullException("this");
            }

            List<T> list = obj.ToList();

            list.Sort();

            return list.AsReadOnly();
        }
    }
}
