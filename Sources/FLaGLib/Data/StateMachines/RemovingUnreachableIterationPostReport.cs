using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class RemovingUnreachableStatesIterationPostReport
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

        public IReadOnlySet<Label> CurrentApproachedMinusCurrentReachableStates
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

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public RemovingUnreachableStatesIterationPostReport(
            IEnumerable<Label> currentReachableStates,
            IEnumerable<Label> nextReachableStates,
            IEnumerable<Label> currentApproachedStates,
            IEnumerable<Label> currentApproachedMinusReachableStates,
            int iteration,
            bool isLastIteration)
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

            if (currentApproachedMinusReachableStates == null)
            {
                throw new ArgumentNullException(nameof(currentApproachedMinusReachableStates));
            }

            CurrentReachableStates = currentReachableStates.ToSortedSet().AsReadOnly();
            NextReachableStates = nextReachableStates.ToSortedSet().AsReadOnly();
            CurrentApproachedMinusCurrentReachableStates = currentApproachedMinusReachableStates.ToSortedSet().AsReadOnly();
            CurrentApproachedStates = currentApproachedStates.ToSortedSet().AsReadOnly();
            Iteration = iteration;
            IsLastIteration = isLastIteration;
        }
    }
}
