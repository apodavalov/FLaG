using System;

namespace FLaGLib.Data.StateMachines
{
    public class MinimizingBeginPostReport
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

        public MinimizingBeginPostReport(SetsOfEquivalence setsOfEquivalence, int iteration)
        {
            if (setsOfEquivalence == null)
            {
                throw new ArgumentNullException(nameof(setsOfEquivalence));
            }

            SetsOfEquivalence = setsOfEquivalence;
            Iteration = iteration;
        }
    }
}
