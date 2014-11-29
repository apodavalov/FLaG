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

        public IReadOnlyList<Label> FinalStates
        {
            get;
            private set;
        }

        public IReadOnlyList<Transition> Transitions
        {
            get;
            private set;
        }

        public IReadOnlyList<Label> States
        {
            get;
            private set;
        }

        public IReadOnlyList<char> Alphabet
        {
            get;
            private set;
        }

        public bool IsDeterministic()
        {
            Transition prevTransition = null;

            foreach (Transition transition in Transitions)
            {
                if (prevTransition != null && prevTransition.CurrentState == transition.CurrentState && prevTransition.Symbol == transition.Symbol)
                {
                    return false;
                }

                prevTransition = transition;
            }

            return true;
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
            ISet<char> alphabet = new HashSet<char>();

            foreach (Transition transition in transitions)
            {
                states.Add(transition.CurrentState);
                states.Add(transition.NextState);
                alphabet.Add(transition.Symbol);
            }

            if (!states.Contains(initialState))
            {
                throw new ArgumentException("Set of states doesn't contain initial state.");
            }

            if (!states.IsSupersetOf(finalStates))
            {
                throw new ArgumentException("Set of states doesn't the superset of final states.");
            }

            List<Label> stateList = states.ToList();

            stateList.Sort();

            States = stateList.AsReadOnly();

            List<Label> finalStateList = finalStates.ToList();

            FinalStates = finalStateList.AsReadOnly();

            List<Transition> transitionList = transitions.ToList();

            transitionList.Sort();

            Transitions = transitionList.AsReadOnly();

            List<char> alphabetList = alphabet.ToList();

            alphabetList.Sort();

            Alphabet = alphabetList.AsReadOnly();
        }
    }
}
