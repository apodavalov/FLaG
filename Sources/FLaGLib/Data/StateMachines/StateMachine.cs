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

        public int Number
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

        public StateMachine ConvertToDeterministicIfNot(int? number = null)
        {
            if (IsDeterministic())
            {
                return this;
            }

            int newNumber = number == null ? Number + 1 : number.Value;

            HashSet<Transition> newTransitions = new HashSet<Transition>();

            HashSet<Label> visitedNewStates = new HashSet<Label>();

            Label newInitialState = InitialState.ConvertToComplex();

            visitedNewStates.Add(newInitialState);

            Queue<Label> queue = new Queue<Label>();

            queue.Enqueue(newInitialState);

            do
            {
                Label currentState = queue.Dequeue();
                List<SingleLabel> currentStateSingleLabels = currentState.Sublabels.ToList();

                Dictionary<char, HashSet<SingleLabel>> symbolSingleLabelsDictionary = new Dictionary<char, HashSet<SingleLabel>>();

                foreach (Transition transition in Transitions)
                {
                    SingleLabel currentStateSingleLabel = transition.CurrentState.ExtractSingleLabel();

                    if (currentStateSingleLabels.BinarySearch(currentStateSingleLabel) >= 0)
                    {
                        HashSet<SingleLabel> singleLabels;

                        if (symbolSingleLabelsDictionary.ContainsKey(transition.Symbol))
                        {
                            singleLabels = symbolSingleLabelsDictionary[transition.Symbol];
                        }
                        else
                        {
                            symbolSingleLabelsDictionary[transition.Symbol] = singleLabels = new HashSet<SingleLabel>();
                        }

                        SingleLabel nextStateSingleLabel = transition.NextState.ExtractSingleLabel();

                        singleLabels.Add(nextStateSingleLabel);
                    }
                }

                foreach (KeyValuePair<char, HashSet<SingleLabel>> entry in symbolSingleLabelsDictionary)
                {
                    Label nextState = new Label(entry.Value);
                    newTransitions.Add(new Transition(currentState, entry.Key, nextState));

                    if (visitedNewStates.Add(nextState))
                    {
                        queue.Enqueue(nextState);
                    }
                }

            } while (queue.Count > 0);

            HashSet<Label> newFinalStates = new HashSet<Label>();

            foreach (Label state in ExtractStates(newTransitions))
            {
                List<SingleLabel> stateSingleLabels = state.Sublabels.ToList();

                bool atLeastOneFromFinalStates = false;

                foreach (Label finalState in FinalStates)
                {
                    if (stateSingleLabels.BinarySearch(finalState.ExtractSingleLabel()) >= 0)
                    {
                        atLeastOneFromFinalStates = true;
                        break;
                    }
                }

                if (atLeastOneFromFinalStates)
                {
                    newFinalStates.Add(state);
                }
            }

            return new StateMachine(newInitialState,newFinalStates,newTransitions,newNumber);
        }

        private ISet<Label> ExtractStates(IEnumerable<Transition> transitions)
        {
            ISet<Label> states = new HashSet<Label>();

            foreach (Transition transition in transitions)
            {
                states.Add(transition.CurrentState);
                states.Add(transition.NextState);
            }

            return states;
        }

        public StateMachine(Label initialState, ISet<Label> finalStates, ISet<Transition> transitions, int number = 0)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException("initialState");
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

            ISet<Label> states = ExtractStates(transitions);

            if (!states.Contains(initialState))
            {
                throw new ArgumentException("Set of states doesn't contain initial state.");
            }

            if (!states.IsSupersetOf(finalStates))
            {
                throw new ArgumentException("Set of states isn't the superset of final states.");
            }

            ISet<char> alphabet = new HashSet<char>();

            foreach (Transition transition in transitions)
            {
                alphabet.Add(transition.Symbol);
            }

            List<Label> stateList = states.ToList();

            stateList.Sort();

            States = stateList.AsReadOnly();

            List<Label> finalStateList = finalStates.ToList();

            finalStateList.Sort();

            FinalStates = finalStateList.AsReadOnly();

            List<Transition> transitionList = transitions.ToList();

            transitionList.Sort();

            Transitions = transitionList.AsReadOnly();

            List<char> alphabetList = alphabet.ToList();

            alphabetList.Sort();

            Alphabet = alphabetList.AsReadOnly();

            InitialState = initialState;

            Number = number;
        }
    }
}
