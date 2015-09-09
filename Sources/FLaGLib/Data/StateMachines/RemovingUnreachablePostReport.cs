using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

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
            IEnumerable<Label> currentReachableStates,
            IEnumerable<Label> nextReachableStates,
            IEnumerable<Label> currentApproachedStates,
            IEnumerable<Label> nextApproachedStates,             
            int iteration)
        {
            if (currentReachableStates == null)
            {
                throw new ArgumentNullException(nameof(currentReachableStates));
            }

            if (nextReachableStates == null)
            {
                throw new ArgumentNullException(nameof(nextReachableStates));
            }

            if (currentApproachedStates == null)
            {
                throw new ArgumentNullException(nameof(currentApproachedStates));
            }

            if (nextApproachedStates == null)
            {
                throw new ArgumentNullException(nameof(nextApproachedStates));
            }

            CurrentReachableStates = currentReachableStates.ToHashSet().AsReadOnly();
            NextReachableStates = nextReachableStates.ToHashSet().AsReadOnly();
            NextApproachedStates = nextApproachedStates.ToHashSet().AsReadOnly();
            CurrentApproachedStates = currentApproachedStates.ToHashSet().AsReadOnly();
            Iteration = iteration;
        }
    }
}
