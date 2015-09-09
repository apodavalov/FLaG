using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.StateMachines
{
    public class MetaFinalState
    {
        public IReadOnlySet<Label> RequiredStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> OptionalStates
        {
            get;
            private set;
        }

        public MetaFinalState(IEnumerable<Label> requiredStates, IEnumerable<Label> optionalStates)
        {
            if (requiredStates == null)
            {
                throw new ArgumentNullException("requiredStates");
            }

            if (optionalStates == null)
            {
                throw new ArgumentNullException("optionalStates");
            }

            RequiredStates = requiredStates.ToSortedSet().AsReadOnly();
            OptionalStates = optionalStates.ToSortedSet().AsReadOnly();
        }
    }
}
