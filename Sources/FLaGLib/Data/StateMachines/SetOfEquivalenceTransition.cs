using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class SetOfEquivalenceTransition
    {
        public IReadOnlySet<char> Symbols
        {
            get;
            private set;
        }

        public int IndexOfCurrentSetOfEquivalence
        {
            get;
            private set;
        }

        public SetOfEquivalenceTransition(
            IEnumerable<char> symbols,
            int indexOfCurrentSetOfEquivalence)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            Symbols = symbols.ToSortedSet().AsReadOnly();
            IndexOfCurrentSetOfEquivalence = indexOfCurrentSetOfEquivalence;
        }
    }
}
