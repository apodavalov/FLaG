using FLaGLib.Collections;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.Helpers;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FLaGLib.Data.StateMachines
{
    public class StateMachine
    {
        internal const char _DefaultStateSymbol = 'S';

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

        public RegExps.Expression MakeExpression(GrammarType grammarType, Action<Matrix> onBegin = null, Action<Matrix> onIterate = null)
        {
            return MakeGrammar(grammarType).MakeExpression(grammarType, onBegin, onIterate);
        }

        public Grammar MakeGrammar(GrammarType grammarType)
        {
            switch (grammarType)
            {
                case GrammarType.Left:
                    return MakeGrammarLeft();
                case GrammarType.Right:
                    return MakeGrammarRight();
                default:
                    throw new InvalidOperationException(Grammar.GrammarIsNotSupportedMessage);
            }
        }

        private Grammar MakeGrammarLeft()
        {
            IDictionary<char, TerminalSymbol> charTerminalMap = Alphabet.ToDictionary(c => c, c => new TerminalSymbol(c));
            IDictionary<Label, NonTerminalSymbol> stateNonTerminalMap = States.ToDictionary(s => s, s => new NonTerminalSymbol(s));

            NonTerminalSymbol target = Grammar.GetNewNonTerminal(stateNonTerminalMap.Values);
            
            ISet<Rule> rules = new HashSet<Rule>();

            foreach (Transition transition in Transitions)
            {
                Chain chain = new Chain(
                    EnumerateHelper.Sequence<Symbol>(
                        stateNonTerminalMap[transition.CurrentState], charTerminalMap[transition.Symbol]
                    )
                );

                rules.Add(new Rule(chain.AsSequence(), stateNonTerminalMap[transition.NextState]));
            }

            rules.Add(new Rule(Chain.Empty.AsSequence(), stateNonTerminalMap[InitialState]));

            IEnumerable<Chain> chains = FinalStates.Select(fs => new Chain(stateNonTerminalMap[fs].AsSequence()));

            rules.Add(new Rule(chains, target));

            return new Grammar(rules, target);
        }

        private Grammar MakeGrammarRight()
        {
            IDictionary<char, TerminalSymbol> charTerminalMap = Alphabet.ToDictionary(c => c, c => new TerminalSymbol(c));
            IDictionary<Label, NonTerminalSymbol> stateNonTerminalMap = States.ToDictionary(s => s, s => new NonTerminalSymbol(s));

            NonTerminalSymbol target = stateNonTerminalMap[InitialState];

            ISet<Rule> rules = new HashSet<Rule>();

            foreach (Transition transition in Transitions)
            {
                Chain chain = new Chain(
                    EnumerateHelper.Sequence<Symbol>(
                        charTerminalMap[transition.Symbol], stateNonTerminalMap[transition.NextState]
                    )
                );

                rules.Add(new Rule(chain.AsSequence(), stateNonTerminalMap[transition.CurrentState]));
            }

            rules.AddRange(FinalStates.Select(fs => new Rule(Chain.Empty.AsSequence(), stateNonTerminalMap[fs])));

            return new Grammar(rules, target);
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

        public StateMachine Reorganize(IDictionary<Label, Label> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            Label initialState = map[InitialState];
            IEnumerable<Label> finalStates = FinalStates.Select(fs => map[fs]);
            IEnumerable<Transition> transitions = Transitions.Select(t => new Transition(map[t.CurrentState], t.Symbol, map[t.NextState]));

            return new StateMachine(initialState, finalStates, transitions);
        }

        public StateMachine Reorganize(int firstIndex)
        {
            int index = firstIndex;

            IDictionary<Label, Label> map = new Dictionary<Label, Label>();

            foreach (Label symbol in States)
            {
                map.Add(symbol, new Label(new SingleLabel('S', index++)));
            }

            return Reorganize(map);
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

        public IReadOnlySet<Label> GetMetaState()
        {
            CheckStatesSimple("Meta state cannot be obtained for non simple states.");

            return States;
        }

        public MetaFinalState GetMetaFinalState()
        {
            CheckStatesSimple("Meta final state cannot be obtained for non simple states.");

            SortedSet<Label> optionalStates = new SortedSet<Label>(States);
            
            optionalStates.ExceptWith(FinalStates);

            return new MetaFinalState(FinalStates, optionalStates.AsReadOnly());
        }

        private void CheckStatesSimple(string message)
        {
            foreach (Label state in States)
            {
                if (state.LabelType != LabelType.Simple)
                {
                    throw new InvalidOperationException(message);
                }
            }
        }

        public IReadOnlySet<MetaTransition> GetMetaTransitions()
        {
            CheckStatesSimple("Meta transitions cannot be obtained for non simple states.");

            IDictionary<char, IDictionary<Label, SortedSet<Label>>> map = 
                new Dictionary<char, IDictionary<Label, SortedSet<Label>>>();

            foreach (Transition transition in Transitions)
            {
                IDictionary<Label, SortedSet<Label>> value;
                if (map.ContainsKey(transition.Symbol))
                {
                    value = map[transition.Symbol];
                }
                else
                {
                    map[transition.Symbol] = value = new Dictionary<Label, SortedSet<Label>>();
                }

                SortedSet<Label> set;

                if (value.ContainsKey(transition.CurrentState))
                {
                    set = value[transition.CurrentState];
                }
                else
                {
                    value[transition.CurrentState] = set = new SortedSet<Label>();
                }

                set.Add(transition.NextState);
            }

            IDictionary<char, IDictionary<SortedSet<Label>, IList<SortedSet<Label>>>> metaTransitions = 
                new Dictionary<char, IDictionary<SortedSet<Label>, IList<SortedSet<Label>>>>();

            IDictionary<char, IList<Tuple<SortedSet<Label>,SortedSet<Label>>>> values = 
                new Dictionary<char,IList<Tuple<SortedSet<Label>,SortedSet<Label>>>>();

            foreach (KeyValuePair<char, IDictionary<Label, SortedSet<Label>>> symbolStateStatesMap in map)
            {
                foreach (KeyValuePair<Label,SortedSet<Label>> stateStatesMap in symbolStateStatesMap.Value)
                {
                    IList<Tuple<SortedSet<Label>, SortedSet<Label>>> list;

                    if (values.ContainsKey(symbolStateStatesMap.Key))
                    {
                        list = values[symbolStateStatesMap.Key];
                    }
                    else
                    {
                        values[symbolStateStatesMap.Key] = list = new List<Tuple<SortedSet<Label>, SortedSet<Label>>>();
                    }

                    Tuple<SortedSet<Label>, SortedSet<Label>> tuple = 
                        new Tuple<SortedSet<Label>, SortedSet<Label>>(
                            new SortedSet<Label>(EnumerateHelper.Sequence(stateStatesMap.Key)), 
                            stateStatesMap.Value);

                    list.Add(tuple);

                    AddMetaTransition(symbolStateStatesMap.Key, tuple, metaTransitions);
                }
            }

            do
            {
                IDictionary<char, IList<Tuple<SortedSet<Label>, SortedSet<Label>>>> newValues = 
                    new Dictionary<char, IList<Tuple<SortedSet<Label>, SortedSet<Label>>>>();

                foreach (KeyValuePair<char, IList<Tuple<SortedSet<Label>,SortedSet<Label>>>> symbolStateStatesMap in values)
                {
                    foreach (KeyValuePair<Label, SortedSet<Label>> stateStatesMap in map[symbolStateStatesMap.Key])
                    {
                        foreach (Tuple<SortedSet<Label>,SortedSet<Label>> metaTransition in symbolStateStatesMap.Value)
                        {
                            if (!stateStatesMap.Value.IsSupersetOf(metaTransition.Item2) &&
                                !metaTransition.Item2.IsSupersetOf(stateStatesMap.Value))
                            {
                                SortedSet<Label> currentState = new SortedSet<Label>(metaTransition.Item1);
                                currentState.Add(stateStatesMap.Key);

                                SortedSet<Label> nextState = new SortedSet<Label>(metaTransition.Item2);
                                nextState.UnionWith(stateStatesMap.Value);

                                Tuple<SortedSet<Label>, SortedSet<Label>> tuple =
                                    new Tuple<SortedSet<Label>, SortedSet<Label>>(currentState, nextState);

                                if (!IsMetaTransitionAbsorbed(symbolStateStatesMap.Key,tuple,metaTransitions))
                                {
                                    IList<Tuple<SortedSet<Label>, SortedSet<Label>>> list;
                                
                                    if (newValues.ContainsKey(symbolStateStatesMap.Key))
                                    {
                                        list = newValues[symbolStateStatesMap.Key];
                                    }
                                    else
                                    {
                                        newValues[symbolStateStatesMap.Key] = list = 
                                            new List<Tuple<SortedSet<Label>, SortedSet<Label>>>();
                                    }

                                    list.Add(tuple);

                                    AddMetaTransition(symbolStateStatesMap.Key, tuple, metaTransitions);
                                }
                            }
                        }
                    }
                }

                values = newValues;
            } while (values.Count > 0);

            SortedSet<MetaTransition> metaTransitionSet = new SortedSet<MetaTransition>();

            foreach (KeyValuePair<char, IDictionary<SortedSet<Label>, IList<SortedSet<Label>>>> transitionMap in metaTransitions)
            {
                foreach (KeyValuePair<SortedSet<Label>, IList<SortedSet<Label>>> currentNextStatesMap in transitionMap.Value)
                {
                    foreach (SortedSet<Label> currentState in currentNextStatesMap.Value)
                    {
                        metaTransitionSet.Add(ToMetaTransition(transitionMap.Key, currentState, currentNextStatesMap.Key));
                    }
                }
            }

            return metaTransitionSet.AsReadOnly();
        }

        private bool IsMetaTransitionAbsorbed(char symbol, Tuple<SortedSet<Label>, SortedSet<Label>> tuple, 
            IDictionary<char, IDictionary<SortedSet<Label>, IList<SortedSet<Label>>>> metaTransitions)
        {
            IDictionary<SortedSet<Label>, IList<SortedSet<Label>>> map = metaTransitions[symbol];

            if (!map.ContainsKey(tuple.Item2))
            {
                return false;
            }

            IList<SortedSet<Label>> list = map[tuple.Item2];

            foreach (SortedSet<Label> set in list)
            {
                if (set.IsSubsetOf(tuple.Item1))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddMetaTransition(char symbol, Tuple<SortedSet<Label>, SortedSet<Label>> tuple,
            IDictionary<char, IDictionary<SortedSet<Label>, IList<SortedSet<Label>>>> metaTransitions)
        {
            IDictionary<SortedSet<Label>, IList<SortedSet<Label>>> map;

            if (metaTransitions.ContainsKey(symbol))
            {
                map = metaTransitions[symbol];
            }
            else
            {
                metaTransitions[symbol] = map = new Dictionary<SortedSet<Label>,IList<SortedSet<Label>>>(LabelSetEqualityComparer.Instance);
            }

            IList<SortedSet<Label>> list;

            if (map.ContainsKey(tuple.Item2))
            {
                list = map[tuple.Item2];
            }
            else
            {
                map[tuple.Item2] = list = new List<SortedSet<Label>>();
            }

            list.Add(tuple.Item1);
        }

        private MetaTransition ToMetaTransition(char symbol, SortedSet<Label> currentState, SortedSet<Label> nextState)
        {
            SortedSet<Label> metaCurrentOptionalStates = new SortedSet<Label>(States);
            metaCurrentOptionalStates.ExceptWith(currentState);

            return new MetaTransition(currentState.AsReadOnly(), metaCurrentOptionalStates.AsReadOnly(), 
                symbol, nextState.AsReadOnly());
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

        public StateMachine(Label initialState, IEnumerable<Label> finalStates, IEnumerable<Transition> transitions)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            if (finalStates == null)
            {
                throw new ArgumentNullException(nameof(finalStates));
            }

            if (transitions == null)
            {
                throw new ArgumentNullException(nameof(transitions));
            }

            if (transitions.Any(t => t == null))
            {
                throw new ArgumentException("One of the transitions is null.");
            }            

            FinalStates = finalStates.ToSortedSet().AsReadOnly();
            Transitions = transitions.ToSortedSet().AsReadOnly();

            if (!FinalStates.Any())
            {
                throw new ArgumentException("Final states set is empty.");
            }

            if (FinalStates.AnyNull())
            {
                throw new ArgumentException("One of the final state is null.");                
            }

            ISet<Label> states = ExtractStates(Transitions);

            states.Add(initialState);

            if (!states.IsSupersetOf(FinalStates))
            {
                throw new ArgumentException("Set of states isn't the superset of final states.");
            }

            Alphabet = Transitions.Select(t => t.Symbol).ToSortedSet().AsReadOnly();

            States = states.AsReadOnly();                      

            InitialState = initialState;
        }
    }
}
