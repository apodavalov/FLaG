using FLaGLib.Collections;
using System;

namespace FLaGLib.Data.StateMachines
{
    public class RemovingUnreachableStatesPostReport
    {
        public IReadOnlySet<Label> CurrentReachableStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> CurrentApproachedStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> NextApproachedStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> NextReachableStates
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
            IReadOnlySet<Label> currentReachableStates,
            IReadOnlySet<Label> nextReachableStates,
            IReadOnlySet<Label> currentApproachedStates,
            IReadOnlySet<Label> nextApproachedStates,             
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
