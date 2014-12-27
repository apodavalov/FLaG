using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.StateMachines
{
    internal class LabelSetEqualityComparer : IEqualityComparer<SortedSet<Label>>
    {
        private LabelSetEqualityComparer() { }

        private static LabelSetEqualityComparer _Instance = null;
        public static LabelSetEqualityComparer Instance 
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LabelSetEqualityComparer();
                }

                return _Instance;
            }
        }

        public bool Equals(SortedSet<Label> x, SortedSet<Label> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(SortedSet<Label> obj)
        {
            return obj.GetSequenceHashCode();
        }
    }
}
