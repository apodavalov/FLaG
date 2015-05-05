﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Helpers
{
    class WalkDataLabel<T>
    {
        public bool Marked
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public T Value
        {
            get;
            private set;
        }

        public WalkDataLabel(bool marked, int index, T value)
        {
            Marked = marked;
            Index = index;
            Value = value;
        }
    }
}
