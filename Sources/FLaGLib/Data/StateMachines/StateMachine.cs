using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FLaGLib.Extensions;

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

        public StateMachine RemoveUnreachableStates(
            Action<RemovingUnreachableStatesPostReport> onBegin = null, 
            Action<RemovingUnreachableStatesPostReport> onIterate = null, 
            Action<RemovingUnreachableStatesPostReport> onEnd = null)
        {
            ISet<Label> currentReachableStatesSet = new HashSet<Label>();
            ISet<Label> currentApproachedStatesSet = new HashSet<Label>();

            int i = 0;

            currentReachableStatesSet.Add(InitialState);
            currentApproachedStatesSet.Add(InitialState);

            ISet<Label> nextApproachedStatesSet = new HashSet<Label>();

            if (onBegin != null)
            {
                IReadOnlyList<Label> currentReachableStatesList = currentReachableStatesSet.ConvertToReadOnlyListAndSort();
                IReadOnlyList<Label> nextApproachedStatesList = currentApproachedStatesSet.ConvertToReadOnlyListAndSort();

                onBegin(new RemovingUnreachableStatesPostReport(
                    currentReachableStatesList,
                    currentReachableStatesList,
                    nextApproachedStatesList,
                    nextApproachedStatesList,
                    i));
            }

            do
            {
                i++;

                foreach (Transition transition in Transitions)
                {
                    if (currentApproachedStatesSet.Contains(transition.CurrentState))
                    {
                       nextApproachedStatesSet.Add(transition.NextState);
                    }
                }

                IReadOnlyList<Label> currentReachableStates = currentReachableStatesSet.ConvertToReadOnlyListAndSort();
                IReadOnlyList<Label> currentApproachedStates = nextApproachedStatesSet.ConvertToReadOnlyListAndSort();

                nextApproachedStatesSet.ExceptWith(currentReachableStatesSet);
                currentReachableStatesSet.UnionWith(nextApproachedStatesSet);

                IReadOnlyList<Label> nextReachableStates = currentReachableStatesSet.ConvertToReadOnlyListAndSort();
                IReadOnlyList<Label> nextApproachedStates = nextApproachedStatesSet.ConvertToReadOnlyListAndSort();

                if (nextApproachedStatesSet.Count > 0)
                {
                    if (onIterate != null)
                    {
                        onIterate(new RemovingUnreachableStatesPostReport(
                            currentReachableStates,
                            nextReachableStates,
                            currentApproachedStates,
                            nextApproachedStates,
                            i));
                    }
                }
                else
                {
                    if (onEnd != null)
                    {
                        onEnd(new RemovingUnreachableStatesPostReport(
                            currentReachableStates,
                            nextReachableStates,
                            currentApproachedStates,
                            nextApproachedStates,
                            i));
                    }
                }

                ISet<Label> temp = currentApproachedStatesSet;                
                currentApproachedStatesSet = nextApproachedStatesSet;
                nextApproachedStatesSet = temp;

                nextApproachedStatesSet.Clear();
            } while (currentApproachedStatesSet.Count > 0);

            ISet<Transition> newTransitions = new HashSet<Transition>();

            foreach (Transition transition in Transitions)
            {
                if (currentReachableStatesSet.Contains(transition.CurrentState))
                {
                    newTransitions.Add(transition);
                }
            }

            currentReachableStatesSet.IntersectWith(FinalStates);

            ISet<Label> newFinalStates = currentReachableStatesSet;

            return new StateMachine(InitialState, newFinalStates, newTransitions);            
        }

        public Tuple<StateMachine,IReadOnlyDictionary<Label,Label>> Reorganize()
        {
            Dictionary<Label, Label> dictionary = new Dictionary<Label, Label>();

            int i = 1;

            foreach (Label state in States)
            {
                dictionary.Add(state, new Label(new SingleLabel('S', subIndex: i++)));
            }

            Label newInitialState = dictionary[InitialState];
            ISet<Label> newFinalStates = new HashSet<Label>();
            
            foreach (Label state in FinalStates)
            {
                newFinalStates.Add(dictionary[state]);
            }

            ISet<Transition> newTransitions = new HashSet<Transition>();

            foreach (Transition transition in Transitions)
            {
                newTransitions.Add(new Transition(dictionary[transition.CurrentState],transition.Symbol,dictionary[transition.NextState]));
            }

            return new Tuple<StateMachine, IReadOnlyDictionary<Label, Label>>(
                new StateMachine(newInitialState, newFinalStates, newTransitions), 
                new ReadOnlyDictionary<Label,Label>(dictionary));
        }

        public StateMachine ConvertToDeterministicIfNot()
        {
            if (IsDeterministic())
            {
                return this;
            }

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

            return new StateMachine(newInitialState,newFinalStates,newTransitions);
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

        public StateMachine(Label initialState, ISet<Label> finalStates, ISet<Transition> transitions)
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

            States = states.ConvertToReadOnlyListAndSort();

            FinalStates = finalStates.ConvertToReadOnlyListAndSort();

            Transitions = transitions.ConvertToReadOnlyListAndSort();

            Alphabet = alphabet.ConvertToReadOnlyListAndSort();

            InitialState = initialState;
        }
    }
}
