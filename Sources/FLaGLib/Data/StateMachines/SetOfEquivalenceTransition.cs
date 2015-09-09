using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class SetOfEquivalenceTransition
    {
        public SetOfEquivalence NextSetOfEquivalence
        {
            get;
            private set;
        }

        public IReadOnlySet<char> Symbols
        {
            get;
            private set;
        }

        public SetOfEquivalence CurrentSetOfEquivalence
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
            SetOfEquivalence nextSetOfEquivalence, 
            IEnumerable<char> symbols,
            SetOfEquivalence currentSetOfEquivalence,
            int indexOfCurrentSetOfEquivalence)
        {
            if (nextSetOfEquivalence == null)
            {
                throw new ArgumentNullException("nextSetOfEquivalence");
            }

            if (symbols == null)
            {
                throw new ArgumentNullException("symbols");
            }

            if (currentSetOfEquivalence == null)
            {
                throw new ArgumentNullException("currentSetOfEquivalence");
            }

            NextSetOfEquivalence = nextSetOfEquivalence;
            Symbols = symbols.ToSortedSet().AsReadOnly();
            CurrentSetOfEquivalence = currentSetOfEquivalence;
            IndexOfCurrentSetOfEquivalence = indexOfCurrentSetOfEquivalence;
        }
    }
}
