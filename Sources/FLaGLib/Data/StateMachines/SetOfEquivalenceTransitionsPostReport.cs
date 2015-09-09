using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class SetOfEquivalenceTransitionsPostReport
    {
        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlyList<SetOfEquivalenceTransition> SetOfEquivalenceTransitions
        {
            get;
            private set;
        }

        public SetOfEquivalenceTransitionsPostReport(IEnumerable<SetOfEquivalenceTransition> setOfEquivalenceTransitions, int iteration)
        {
            if (setOfEquivalenceTransitions == null)
            {
                throw new ArgumentNullException(nameof(setOfEquivalenceTransitions));
            }

            SetOfEquivalenceTransitions = setOfEquivalenceTransitions.ToList().AsReadOnly();
            Iteration = iteration;
        }
    }
}
