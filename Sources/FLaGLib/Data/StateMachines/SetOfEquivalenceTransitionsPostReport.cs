using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SetOfEquivalenceTransitionsPostReport(IReadOnlyList<SetOfEquivalenceTransition> setOfEquivalenceTransitions, int iteration)
        {
            if (setOfEquivalenceTransitions == null)
            {
                throw new ArgumentNullException("setOfEquivalenceTransitions");
            }

            SetOfEquivalenceTransitions = setOfEquivalenceTransitions;
            Iteration = iteration;
        }
    }
}
