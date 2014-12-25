using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FLaGLib.Extensions;
using FLaGLib.Collections;
using FLaGLib.Helpers;

namespace FLaGLib.Data.StateMachines
{
    public class StateMachine
    {
        public Label InitialState
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> FinalStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Transition> Transitions
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> States
        {
            get;
            private set;
        }

        public IReadOnlySet<char> Alphabet
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
            ISet<Label> currentReachableStatesSet = new SortedSet<Label>();
            ISet<Label> currentApproachedStatesSet = new SortedSet<Label>();

            int i = 0;

            currentReachableStatesSet.Add(InitialState);
            currentApproachedStatesSet.Add(InitialState);

            ISet<Label> nextApproachedStatesSet = new SortedSet<Label>();

            if (onBegin != null)
            {
                IReadOnlySet<Label> currentReachableStatesList = new SortedSet<Label>(currentReachableStatesSet).AsReadOnly();
                IReadOnlySet<Label> nextApproachedStatesList = new SortedSet<Label>(currentApproachedStatesSet).AsReadOnly();

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

                IReadOnlySet<Label> currentReachableStates = new SortedSet<Label>(currentReachableStatesSet).AsReadOnly();
                IReadOnlySet<Label> currentApproachedStates = new SortedSet<Label>(nextApproachedStatesSet).AsReadOnly();

                nextApproachedStatesSet.ExceptWith(currentReachableStatesSet);
                currentReachableStatesSet.UnionWith(nextApproachedStatesSet);

                IReadOnlySet<Label> nextReachableStates = new SortedSet<Label>(currentReachableStatesSet).AsReadOnly();
                IReadOnlySet<Label> nextApproachedStates = new SortedSet<Label>(nextApproachedStatesSet).AsReadOnly();

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

        public StateMachine Reorganize(char stateSign, Action<IReadOnlyDictionary<Label,Label>> onStateMap = null)
        {
            Dictionary<Label, Label> dictionary = new Dictionary<Label, Label>();

            int i = 1;

            foreach (Label state in States)
            {
                dictionary.Add(state, new Label(new SingleLabel(stateSign, subIndex: i++)));
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

            if (onStateMap != null)
            {
                onStateMap(new ReadOnlyDictionary<Label,Label>(dictionary));
            }

            return new StateMachine(newInitialState, newFinalStates, newTransitions);
        }

        public StateMachine ConvertToDeterministicIfNot()
        {
            if (IsDeterministic())
            {
                return this;
            }

            SortedSet<Transition> newTransitions = new SortedSet<Transition>();

            SortedSet<Label> visitedNewStates = new SortedSet<Label>();

            Label newInitialState = InitialState.ConvertToComplex();

            visitedNewStates.Add(newInitialState);

            Queue<Label> queue = new Queue<Label>();

            queue.Enqueue(newInitialState);

            do
            {
                Label currentState = queue.Dequeue();

                Dictionary<char, HashSet<SingleLabel>> symbolSingleLabelsDictionary = new Dictionary<char, HashSet<SingleLabel>>();

                foreach (Transition transition in Transitions)
                {
                    SingleLabel currentStateSingleLabel = transition.CurrentState.ExtractSingleLabel();

                    if (currentState.Sublabels.Contains(currentStateSingleLabel))
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
                bool atLeastOneFromFinalStates = false;

                foreach (Label finalState in FinalStates)
                {
                    if (state.Sublabels.Contains(finalState.ExtractSingleLabel()))
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

        public StateMachine Minimize(
            Action<SetsOfEquivalencePostReport> onSetsOfEquivalence = null,
            Action<SetOfEquivalenceTransitionsPostReport> onSetOfEquivalenceTransitions = null,
            Action<SetsOfEquivalenceResult> onSetsOfEquivalenceResult = null)
        {
            ILookup<Label, Transition> transitionsByCurrentState = Transitions.ToLookup(transition => transition.CurrentState);

            ISet<Label> nonFinalStates = new SortedSet<Label>(States);
            nonFinalStates.ExceptWith(FinalStates);

            SetsOfEquivalence setsOfEquivalence = new SetsOfEquivalence(
                new SortedSet<SetOfEquivalence>(
                    EnumerateHelper.Sequence(
                    new SetOfEquivalence(new SortedSet<Label>(nonFinalStates)), 
                    new SetOfEquivalence(new SortedSet<Label>(FinalStates)))));

            int i = 0;

            if (onSetsOfEquivalence != null)
            {
                onSetsOfEquivalence(new SetsOfEquivalencePostReport(setsOfEquivalence, i));
            }

            bool changed;

            do
            {
                changed = false;

                i++;

                List<SetOfEquivalence> buildingSetsOfEquivalence = new List<SetOfEquivalence>();

                List<SetOfEquivalenceTransition> setOfEquivalenceTransitions = new List<SetOfEquivalenceTransition>();

                IDictionary<Label, int> statesMap = GetStatesMap(setsOfEquivalence);

                List<SetOfEquivalence> setsOfEquivalenceList = setsOfEquivalence.ToList();

                foreach (SetOfEquivalence setOfEquivalence in setsOfEquivalenceList)
                {
                    IDictionary<ClassOfEquivalenceSet, ISet<Label>> classEquivalenceSetMap = new Dictionary<ClassOfEquivalenceSet, ISet<Label>>();
                    
                    foreach (Label state in setOfEquivalence)
                    {
                        IDictionary<int, ISet<char>> groupMap = new Dictionary<int, ISet<char>>();

                        foreach (Transition transition in transitionsByCurrentState[state])
                        {
                            int groupNum = statesMap[transition.NextState];

                            ISet<char> equality;

                            if (groupMap.ContainsKey(groupNum))
                            {
                                equality = groupMap[groupNum];
                            }
                            else
                            {
                                groupMap[groupNum] = equality = new SortedSet<char>();
                            }

                            equality.Add(transition.Symbol);
                        }

                        ClassOfEquivalenceSet classOfEquivalenceSet =
                            new ClassOfEquivalenceSet(new SortedSet<ClassOfEquivalence>(groupMap.Select(item => new ClassOfEquivalence(item.Key, item.Value.AsReadOnly()))));

                        ISet<Label> states;

                        if (classEquivalenceSetMap.ContainsKey(classOfEquivalenceSet))
                        {
                            states = classEquivalenceSetMap[classOfEquivalenceSet];
                        }
                        else
                        {
                            classEquivalenceSetMap[classOfEquivalenceSet] = states = new SortedSet<Label>();
                        }

                        states.Add(state); 
                    }

                    foreach (KeyValuePair<ClassOfEquivalenceSet,ISet<Label>> entry in classEquivalenceSetMap)
                    {
                        SetOfEquivalence set = new SetOfEquivalence(entry.Value);
                        buildingSetsOfEquivalence.Add(set);

                        foreach (ClassOfEquivalence clazz in entry.Key)
                        {
                            setOfEquivalenceTransitions.Add(new SetOfEquivalenceTransition(set, clazz.Symbols, setsOfEquivalenceList[clazz.SetNum], clazz.SetNum));
                        }
                    }
                }

                if (onSetOfEquivalenceTransitions != null)
                {
                    onSetOfEquivalenceTransitions(new SetOfEquivalenceTransitionsPostReport(setOfEquivalenceTransitions.AsReadOnly(), i));
                }

                SetsOfEquivalence nextSetsOfEquivalence = new SetsOfEquivalence(new SortedSet<SetOfEquivalence>(buildingSetsOfEquivalence));

                changed = setsOfEquivalence != nextSetsOfEquivalence;

                setsOfEquivalence = nextSetsOfEquivalence;

                if (onSetsOfEquivalence != null)
                {
                    onSetsOfEquivalence(new SetsOfEquivalencePostReport(setsOfEquivalence, i));
                }
            } while (changed);

            if (onSetsOfEquivalenceResult != null)
            {
                onSetsOfEquivalenceResult(new SetsOfEquivalenceResult(setsOfEquivalence.Count != States.Count,i));
            }

            if (setsOfEquivalence.Count == States.Count)
            {
                return this;
            }

            IDictionary<Label, Label> newStatesMap = GetOldNewStatesMap(setsOfEquivalence);

            ISet<Transition> transitions = new HashSet<Transition>(
                Transitions.Select(
                item => new Transition(newStatesMap[item.CurrentState], item.Symbol, newStatesMap[item.NextState])));

            ISet<Label> finalStates = new HashSet<Label>(FinalStates.Select(item => newStatesMap[item]));

            Label initialState = newStatesMap[InitialState];

            return new StateMachine(initialState, finalStates, transitions);
        }

        private IDictionary<Label, int> GetStatesMap(SetsOfEquivalence setsOfEquivalence)
        {
            return setsOfEquivalence.SelectMany((set, index) => set.Select(item => new Tuple<Label, int>(item, index))).
                    ToDictionary(item => item.Item1, item => item.Item2);
        }

        private IDictionary<Label, Label> GetOldNewStatesMap(SetsOfEquivalence setsOfEquivalence)
        {
            return setsOfEquivalence.SelectMany(item => item.Select(subitem => new Tuple<Label, Label>(subitem, item.First()))).
               ToDictionary(item => item.Item1, item => item.Item2);
        }

        private ISet<Label> ExtractStates(IEnumerable<Transition> transitions)
        {
            ISet<Label> states = new SortedSet<Label>();

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

            states.Add(initialState);

            if (!states.IsSupersetOf(finalStates))
            {
                throw new ArgumentException("Set of states isn't the superset of final states.");
            }

            ISet<char> alphabet = new SortedSet<char>();

            foreach (Transition transition in transitions)
            {
                alphabet.Add(transition.Symbol);
            }

            States = states.AsReadOnly();

            FinalStates = new SortedSet<Label>(finalStates).AsReadOnly();

            Transitions = new SortedSet<Transition>(transitions).AsReadOnly();

            Alphabet = alphabet.AsReadOnly();

            InitialState = initialState;
        }
    }
}
