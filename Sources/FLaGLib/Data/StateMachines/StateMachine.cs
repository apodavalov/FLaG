using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class StateMachine
    {
        public Label InitialState
        {
            get;
            private set;
        }

        public IReadOnlyCollection<Label> FinalStates
        {
            get;
            private set;
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get;
            private set;
        }

        public IReadOnlyCollection<Label> States
        {
            get;
            private set;
        }

        public StateMachine(Label initialState, ISet<Label> finalStates, ISet<Transition> transitions)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException("state");
            }

            if (finalStates == null)
            {
                throw new ArgumentNullException("finalStates");
            }

            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }

            foreach (Transition transition in transitions)
            {
                if (transition == null)
                {
                    throw new ArgumentException("One of the transitions is null.");
                }
            }

            foreach (Label finalState in finalStates)
            {
                if (finalState == null)
                {
                    throw new ArgumentException("One of the final state is null.");
                }
            }

            ISet<Label> states = new HashSet<Label>();

            foreach (Transition transition in transitions)
            {
                states.Add(transition.CurrentState);
                states.Add(transition.NextState);
            }

            if (!states.Contains(initialState))
            {
                throw new ArgumentException("Set of states doesn't contain initial state.");
            }

            if (!states.IsSupersetOf(finalStates))
            {
                throw new ArgumentException("Set of states doesn't the superset of final states.");
            }
            
            States = states.ToList().AsReadOnly();

            FinalStates = finalStates.ToList().AsReadOnly();
            Transitions = transitions.ToList().AsReadOnly();
        }
    }
}
