using FLaGLib.Collections;
using System;

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

        public MetaFinalState(IReadOnlySet<Label> requiredStates, IReadOnlySet<Label> optionalStates)
        {
            if (requiredStates == null)
            {
                throw new ArgumentNullException("requiredStates");
            }

            if (optionalStates == null)
            {
                throw new ArgumentNullException("optionalStates");
            }

            RequiredStates = requiredStates;
            OptionalStates = optionalStates;
        }
    }
}
