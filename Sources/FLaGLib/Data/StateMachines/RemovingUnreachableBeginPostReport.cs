using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class RemovingUnreachableStatesBeginPostReport
    {
        public IReadOnlySet<Label> ReachableStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> ApproachedStates
        {
            get;
            private set;
        }

        public int Iteration
        {
            get;
            private set;
        }

        public RemovingUnreachableStatesBeginPostReport(
            IEnumerable<Label> reachableStates,
            IEnumerable<Label> approachedStates,
            int iteration)
        {
            if (reachableStates == null)
            {
                throw new ArgumentNullException(nameof(reachableStates));
            }

            if (approachedStates == null)
            {
                throw new ArgumentNullException(nameof(approachedStates));
            }

            ReachableStates = reachableStates.ToSortedSet().AsReadOnly();
            ApproachedStates = approachedStates.ToSortedSet().AsReadOnly();
            Iteration = iteration;
        }
    }
}
