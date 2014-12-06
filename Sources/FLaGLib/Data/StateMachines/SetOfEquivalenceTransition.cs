using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            IReadOnlySet<char> symbols,
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
            Symbols = symbols;
            CurrentSetOfEquivalence = currentSetOfEquivalence;
            IndexOfCurrentSetOfEquivalence = indexOfCurrentSetOfEquivalence;
        }
    }
}
