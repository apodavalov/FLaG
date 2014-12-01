using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class RemovingUnreachableStatesPostReport
    {
        public IReadOnlyList<Label> CurrentReachableStates
        {
            get;
            private set;
        }

        public IReadOnlyList<Label> CurrentApproachedStates
        {
            get;
            private set;
        }

        public IReadOnlyList<Label> NextApproachedStates
        {
            get;
            private set;
        }

        public IReadOnlyList<Label> NextReachableStates
        {
            get;
            private set;
        }

        public int Iteration
        {
            get;
            private set;
        }

        public RemovingUnreachableStatesPostReport(
            IReadOnlyList<Label> currentReachableStates,
            IReadOnlyList<Label> nextReachableStates,
            IReadOnlyList<Label> currentApproachedStates,
            IReadOnlyList<Label> nextApproachedStates,             
            int iteration)
        {
            if (currentReachableStates == null)
            {
                throw new ArgumentNullException("currentReachableStates");
            }

            if (nextReachableStates == null)
            {
                throw new ArgumentNullException("nextReachableStates");
            }

            if (currentApproachedStates == null)
            {
                throw new ArgumentNullException("currentApproachedStates");
            }

            if (nextApproachedStates == null)
            {
                throw new ArgumentNullException("nextApproachedStates");
            }

            CurrentReachableStates = currentReachableStates;
            NextReachableStates = nextReachableStates;
            NextApproachedStates = nextApproachedStates;
            CurrentApproachedStates = currentApproachedStates;
            Iteration = iteration;
        }
    }
}
