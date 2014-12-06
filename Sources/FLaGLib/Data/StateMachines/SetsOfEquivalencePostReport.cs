using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Data.StateMachines
{
    public class SetsOfEquivalencePostReport
    {
        public SetsOfEquivalence SetsOfEquivalence
        {
            get;
            private set;
        }

        public int Iteration
        {
            get;
            private set;
        }

        public SetsOfEquivalencePostReport(SetsOfEquivalence setsOfEquivalence, int iteration)
        {
            if (setsOfEquivalence == null)
            {
                throw new ArgumentNullException("setsOfEquivalence");
            }

            SetsOfEquivalence = setsOfEquivalence;
            Iteration = iteration;
        }
    }
}
