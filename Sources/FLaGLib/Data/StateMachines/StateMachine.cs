using System.Collections.Immutable;
using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Data.StateMachines
{
    public sealed class StateMachine
    {
        internal const char _DefaultStateSymbol = 'S';

        public Label InitialState { get; }

        public IImmutableSet<Label> FinalStates { get; }

        public IImmutableSet<Transition> Transitions { get; }

        public IImmutableSet<Label> States { get; }

        public IImmutableSet<char> Alphabet { get; }

        public bool IsDeterministic()
        {
            Transition? prevTransition = null;

            foreach (Transition transition in Transitions)
            {
                if (
                    prevTransition is not null
                    && prevTransition.CurrentState == transition.CurrentState
                    && prevTransition.Symbol == transition.Symbol
                )
                {
                    return false;
                }

                prevTransition = transition;
            }

            return true;
        }

        public RegExps.Expression MakeExpression(
            GrammarType grammarType,
            Action<Matrix>? onBegin = null,
            Action<Matrix>? onIterate = null
        ) => MakeGrammar(grammarType).MakeExpression(grammarType, onBegin, onIterate);

        public Grammar MakeGrammar(GrammarType grammarType) =>
            GrammarDispatcher.Dispatch(grammarType, MakeGrammarLeft, MakeGrammarRight);

        private Grammar MakeGrammarLeft()
        {
            Dictionary<char, TerminalSymbol> charTerminalMap = Alphabet.ToDictionary(
                c => c,
                c => new TerminalSymbol(c)
            );
            Dictionary<Label, NonTerminalSymbol> stateNonTerminalMap = States.ToDictionary(
                s => s,
                s => new NonTerminalSymbol(s)
            );

            NonTerminalSymbol target = Grammar.GetNewNonTerminal(stateNonTerminalMap.Values);

            HashSet<Rule> rules = [];

            foreach (Transition transition in Transitions)
            {
                Chain chain = new([
                    stateNonTerminalMap[transition.CurrentState],
                    charTerminalMap[transition.Symbol],
                ]);

                rules.Add(new Rule([chain], stateNonTerminalMap[transition.NextState]));
            }

            rules.Add(new Rule([Chain.Empty], stateNonTerminalMap[InitialState]));

            IEnumerable<Chain> chains = FinalStates.Select(fs => new Chain([
                stateNonTerminalMap[fs],
            ]));

            rules.Add(new Rule(chains, target));

            return new Grammar(rules, target);
        }

        private Grammar MakeGrammarRight()
        {
            Dictionary<char, TerminalSymbol> charTerminalMap = Alphabet.ToDictionary(
                c => c,
                c => new TerminalSymbol(c)
            );
            Dictionary<Label, NonTerminalSymbol> stateNonTerminalMap = States.ToDictionary(
                s => s,
                s => new NonTerminalSymbol(s)
            );

            NonTerminalSymbol target = stateNonTerminalMap[InitialState];

            HashSet<Rule> rules = [];

            foreach (Transition transition in Transitions)
            {
                Chain chain = new([
                    charTerminalMap[transition.Symbol],
                    stateNonTerminalMap[transition.NextState],
                ]);
                rules.Add(new Rule([chain], stateNonTerminalMap[transition.CurrentState]));
            }

            rules.UnionWith(FinalStates.Select(fs => new Rule([], stateNonTerminalMap[fs])));

            return new Grammar(rules, target);
        }

        public StateMachine RemoveUnreachableStates(
            Action<RemovingUnreachableStatesBeginPostReport>? onBegin = null,
            Action<RemovingUnreachableStatesIterationPostReport>? onIterate = null
        )
        {
            SortedSet<Label> currentReachableStatesSet = [];
            SortedSet<Label> currentApproachedStatesSet = [];
            int i = 0;
            currentReachableStatesSet.Add(InitialState);
            currentApproachedStatesSet.Add(InitialState);
            onBegin?.Invoke(
                new RemovingUnreachableStatesBeginPostReport(
                    currentReachableStatesSet,
                    currentApproachedStatesSet,
                    i
                )
            );
            SortedSet<Label> nextApproachedStatesSet = [];

            do
            {
                ++i;

                foreach (Transition transition in Transitions)
                {
                    if (currentApproachedStatesSet.Contains(transition.CurrentState))
                    {
                        nextApproachedStatesSet.Add(transition.NextState);
                    }
                }

                SortedSet<Label> currentReachableStates = currentReachableStatesSet.ToSortedSet();
                SortedSet<Label> currentApproachedStates = nextApproachedStatesSet.ToSortedSet();
                nextApproachedStatesSet.ExceptWith(currentReachableStatesSet);
                currentReachableStatesSet.UnionWith(nextApproachedStatesSet);

                onIterate?.Invoke(
                    new RemovingUnreachableStatesIterationPostReport(
                        currentReachableStates,
                        currentReachableStatesSet,
                        currentApproachedStates,
                        nextApproachedStatesSet,
                        i,
                        nextApproachedStatesSet.Count <= 0
                    )
                );

                (currentApproachedStatesSet, nextApproachedStatesSet) = (
                    nextApproachedStatesSet,
                    currentApproachedStatesSet
                );
                nextApproachedStatesSet.Clear();
            } while (currentApproachedStatesSet.Count > 0);

            HashSet<Transition> newTransitions = [];

            foreach (Transition transition in Transitions)
            {
                if (currentReachableStatesSet.Contains(transition.CurrentState))
                {
                    newTransitions.Add(transition);
                }
            }

            currentReachableStatesSet.IntersectWith(FinalStates);

            return new StateMachine(InitialState, currentReachableStatesSet, newTransitions);
        }

        public StateMachine Reorganize(IDictionary<Label, Label> map)
        {
            Label initialState = map[InitialState];
            IEnumerable<Label> finalStates = FinalStates.Select(fs => map[fs]);
            IEnumerable<Transition> transitions = Transitions.Select(t => new Transition(
                map[t.CurrentState],
                t.Symbol,
                map[t.NextState]
            ));

            return new StateMachine(initialState, finalStates, transitions);
        }

        public StateMachine Reorganize(int firstIndex)
        {
            int index = firstIndex;

            Dictionary<Label, Label> map = [];

            foreach (Label symbol in States)
            {
                map.Add(symbol, new(new SingleLabel('S', index++)));
            }

            return Reorganize(map);
        }

        public StateMachine Reorganize(Action<IReadOnlyDictionary<Label, Label>>? onStateMap = null)
        {
            return Reorganize('S', onStateMap);
        }

        public StateMachine Reorganize(
            char stateSign,
            Action<IReadOnlyDictionary<Label, Label>>? onStateMap = null
        )
        {
            Dictionary<Label, Label> dictionary = [];

            int i = 1;

            foreach (Label state in States)
            {
                dictionary.Add(state, new(new SingleLabel(stateSign, i++)));
            }

            Label newInitialState = dictionary[InitialState];
            HashSet<Label> newFinalStates = [];

            foreach (Label state in FinalStates)
            {
                newFinalStates.Add(dictionary[state]);
            }

            HashSet<Transition> newTransitions = [];

            foreach (Transition transition in Transitions)
            {
                newTransitions.Add(
                    new Transition(
                        dictionary[transition.CurrentState],
                        transition.Symbol,
                        dictionary[transition.NextState]
                    )
                );
            }

            onStateMap?.Invoke(dictionary);

            return new StateMachine(newInitialState, newFinalStates, newTransitions);
        }

        public StateMachine ConvertToDeterministicIfNot()
        {
            if (IsDeterministic())
            {
                return this;
            }

            SortedSet<Transition> newTransitions = [];
            SortedSet<Label> visitedNewStates = [];
            Label newInitialState = InitialState.ConvertToComplex();
            visitedNewStates.Add(newInitialState);
            Queue<Label> queue = [];
            queue.Enqueue(newInitialState);

            do
            {
                Label currentState = queue.Dequeue();
                Dictionary<char, HashSet<SingleLabel>> symbolSingleLabelsDictionary = [];
                foreach (Transition transition in Transitions)
                {
                    SingleLabel currentStateSingleLabel =
                        transition.CurrentState.ExtractSingleLabel();

                    if (currentState.Sublabels.Contains(currentStateSingleLabel))
                    {
                        HashSet<SingleLabel> singleLabels = symbolSingleLabelsDictionary.GetOrAdd(
                            transition.Symbol,
                            () => []
                        );
                        SingleLabel nextStateSingleLabel =
                            transition.NextState.ExtractSingleLabel();
                        singleLabels.Add(nextStateSingleLabel);
                    }
                }

                foreach (
                    KeyValuePair<char, HashSet<SingleLabel>> entry in symbolSingleLabelsDictionary
                )
                {
                    Label nextState = new Label(entry.Value);
                    newTransitions.Add(new Transition(currentState, entry.Key, nextState));

                    if (visitedNewStates.Add(nextState))
                    {
                        queue.Enqueue(nextState);
                    }
                }
            } while (queue.Count > 0);

            HashSet<Label> newFinalStates = [];

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

            return new StateMachine(newInitialState, newFinalStates, newTransitions);
        }

        public StateMachine Minimize(
            Action<MinimizingBeginPostReport>? onBegin = null,
            Action<MinimizingIterationPostReport>? onIterate = null,
            Action<bool>? onEnd = null
        )
        {
            ILookup<Label, Transition> transitionsByCurrentState = Transitions.ToLookup(
                transition => transition.CurrentState
            );
            SortedSet<Label> nonFinalStates = States.ToSortedSet();
            nonFinalStates.ExceptWith(FinalStates);
            SetsOfEquivalence setsOfEquivalence = new([
                new SetOfEquivalence(nonFinalStates),
                new SetOfEquivalence(FinalStates),
            ]);

            int i = 0;
            onBegin?.Invoke(new MinimizingBeginPostReport(setsOfEquivalence, i));
            bool changed;

            do
            {
                changed = false;
                ++i;
                List<SetOfEquivalence> buildingSetsOfEquivalence = [];
                List<SetOfEquivalenceTransition> setOfEquivalenceTransitions = [];
                IDictionary<Label, int> statesMap = GetStatesMap(setsOfEquivalence);

                foreach (SetOfEquivalence setOfEquivalence in setsOfEquivalence.SortedList)
                {
                    Dictionary<ClassOfEquivalenceSet, HashSet<Label>> classEquivalenceSetMap = [];

                    foreach (Label state in setOfEquivalence.Set)
                    {
                        Dictionary<int, HashSet<char>> groupMap = [];

                        foreach (Transition transition in transitionsByCurrentState[state])
                        {
                            int groupNum = statesMap[transition.NextState];

                            HashSet<char> equality = groupMap.GetOrAdd(groupNum, () => []);
                            equality.Add(transition.Symbol);
                        }

                        ClassOfEquivalenceSet classOfEquivalenceSet = new(
                            groupMap.Select(item => new ClassOfEquivalence(item.Key, item.Value))
                        );
                        HashSet<Label> states = classEquivalenceSetMap.GetOrAdd(
                            classOfEquivalenceSet,
                            () => []
                        );
                        states.Add(state);
                    }

                    foreach (
                        KeyValuePair<
                            ClassOfEquivalenceSet,
                            HashSet<Label>
                        > entry in classEquivalenceSetMap
                    )
                    {
                        SetOfEquivalence set = new(
                            entry.Value,
                            entry.Key.ClassOfEquivalences.Select(
                                c => new SetOfEquivalenceTransition(c.Symbols, c.SetNum)
                            )
                        );
                        buildingSetsOfEquivalence.Add(set);
                    }
                }

                SetsOfEquivalence nextSetsOfEquivalence = new(buildingSetsOfEquivalence);
                changed = setsOfEquivalence != nextSetsOfEquivalence;
                setsOfEquivalence = nextSetsOfEquivalence;
                onIterate?.Invoke(
                    new MinimizingIterationPostReport(setsOfEquivalence, i, !changed)
                );
            } while (changed);

            onEnd?.Invoke(setsOfEquivalence.SortedList.Count != States.Count);

            if (setsOfEquivalence.SortedList.Count == States.Count)
            {
                return this;
            }

            IDictionary<Label, Label> newStatesMap = GetOldNewStatesMap(setsOfEquivalence);
            return new StateMachine(
                newStatesMap[InitialState],
                FinalStates.Select(item => newStatesMap[item]),
                Transitions.Select(item => new Transition(
                    newStatesMap[item.CurrentState],
                    item.Symbol,
                    newStatesMap[item.NextState]
                ))
            );
        }

        public Label GetMetaInitialState()
        {
            CheckStatesSimple("Meta state cannot be obtained for non simple states.");
            return InitialState.ConvertToComplex();
        }

        public IImmutableSet<Label> GetMetaStates()
        {
            CheckStatesSimple("Meta state cannot be obtained for non simple states.");
            return States;
        }

        public MetaFinalState GetMetaFinalStates()
        {
            CheckStatesSimple("Meta final state cannot be obtained for non simple states.");
            SortedSet<Label> optionalStates = States.ToSortedSet();
            optionalStates.ExceptWith(FinalStates);
            return new MetaFinalState(FinalStates, optionalStates.ToImmutableSortedSet());
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

        public IImmutableSet<MetaTransition> GetMetaTransitions()
        {
            CheckStatesSimple("Meta transitions cannot be obtained for non simple states.");
            Dictionary<char, Dictionary<Label, SortedSet<Label>>> map = [];

            foreach (Transition transition in Transitions)
            {
                IDictionary<Label, SortedSet<Label>> value = map.GetOrAdd(
                    transition.Symbol,
                    () => []
                );
                SortedSet<Label> set = value.GetOrAdd(transition.CurrentState, () => []);
                set.Add(transition.NextState);
            }

            Dictionary<
                char,
                Dictionary<SortedSet<Label>, List<SortedSet<Label>>>
            > metaTransitions = [];
            Dictionary<char, List<Tuple<SortedSet<Label>, SortedSet<Label>>>> values = [];

            foreach (
                KeyValuePair<char, Dictionary<Label, SortedSet<Label>>> symbolStateStatesMap in map
            )
            {
                foreach (
                    KeyValuePair<
                        Label,
                        SortedSet<Label>
                    > stateStatesMap in symbolStateStatesMap.Value
                )
                {
                    List<Tuple<SortedSet<Label>, SortedSet<Label>>> list = values.GetOrAdd(
                        symbolStateStatesMap.Key,
                        () => []
                    );
                    Tuple<SortedSet<Label>, SortedSet<Label>> tuple = new(
                        new SortedSet<Label>([stateStatesMap.Key]),
                        stateStatesMap.Value
                    );
                    list.Add(tuple);
                    AddMetaTransition(symbolStateStatesMap.Key, tuple, metaTransitions);
                }
            }

            do
            {
                Dictionary<char, List<Tuple<SortedSet<Label>, SortedSet<Label>>>> newValues = [];

                foreach (
                    KeyValuePair<
                        char,
                        List<Tuple<SortedSet<Label>, SortedSet<Label>>>
                    > symbolStateStatesMap in values
                )
                {
                    foreach (
                        KeyValuePair<Label, SortedSet<Label>> stateStatesMap in map[
                            symbolStateStatesMap.Key
                        ]
                    )
                    {
                        foreach (
                            Tuple<
                                SortedSet<Label>,
                                SortedSet<Label>
                            > metaTransition in symbolStateStatesMap.Value
                        )
                        {
                            if (
                                !stateStatesMap.Value.IsSupersetOf(metaTransition.Item2)
                                && !metaTransition.Item2.IsSupersetOf(stateStatesMap.Value)
                            )
                            {
                                SortedSet<Label> currentState = metaTransition.Item1.ToSortedSet();
                                currentState.Add(stateStatesMap.Key);
                                SortedSet<Label> nextState = metaTransition.Item2.ToSortedSet();
                                nextState.UnionWith(stateStatesMap.Value);
                                Tuple<SortedSet<Label>, SortedSet<Label>> tuple = new(
                                    currentState,
                                    nextState
                                );

                                if (
                                    !IsMetaTransitionAbsorbed(
                                        symbolStateStatesMap.Key,
                                        tuple,
                                        metaTransitions
                                    )
                                )
                                {
                                    List<Tuple<SortedSet<Label>, SortedSet<Label>>> list =
                                        newValues.GetOrAdd(symbolStateStatesMap.Key, () => []);

                                    AddMetaTransition(
                                        symbolStateStatesMap.Key,
                                        tuple,
                                        metaTransitions
                                    );
                                }
                            }
                        }
                    }
                }

                values = newValues;
            } while (values.Count > 0);

            SortedSet<MetaTransition> metaTransitionSet = [];

            foreach (
                KeyValuePair<
                    char,
                    Dictionary<SortedSet<Label>, List<SortedSet<Label>>>
                > transitionMap in metaTransitions
            )
            {
                foreach (
                    KeyValuePair<
                        SortedSet<Label>,
                        List<SortedSet<Label>>
                    > currentNextStatesMap in transitionMap.Value
                )
                {
                    foreach (SortedSet<Label> currentState in currentNextStatesMap.Value)
                    {
                        metaTransitionSet.Add(
                            ToMetaTransition(
                                transitionMap.Key,
                                currentState,
                                currentNextStatesMap.Key
                            )
                        );
                    }
                }
            }

            return metaTransitionSet.ToImmutableSortedSet();
        }

        private static bool IsMetaTransitionAbsorbed(
            char symbol,
            Tuple<SortedSet<Label>, SortedSet<Label>> tuple,
            Dictionary<char, Dictionary<SortedSet<Label>, List<SortedSet<Label>>>> metaTransitions
        )
        {
            Dictionary<SortedSet<Label>, List<SortedSet<Label>>> map = metaTransitions[symbol];

            if (!map.TryGetValue(tuple.Item2, out List<SortedSet<Label>>? list))
            {
                return false;
            }

            foreach (SortedSet<Label> set in list)
            {
                if (set.IsSubsetOf(tuple.Item1))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddMetaTransition(
            char symbol,
            Tuple<SortedSet<Label>, SortedSet<Label>> tuple,
            Dictionary<char, Dictionary<SortedSet<Label>, List<SortedSet<Label>>>> metaTransitions
        )
        {
            metaTransitions
                .GetOrAdd(
                    symbol,
                    () =>
                        new Dictionary<SortedSet<Label>, List<SortedSet<Label>>>(
                            LabelSetEqualityComparer.Instance
                        )
                )
                .GetOrAdd(tuple.Item2, () => [])
                .Add(tuple.Item1);
        }

        private MetaTransition ToMetaTransition(
            char symbol,
            SortedSet<Label> currentState,
            SortedSet<Label> nextState
        )
        {
            SortedSet<Label> metaCurrentOptionalStates = States.ToSortedSet();
            metaCurrentOptionalStates.ExceptWith(currentState);
            return new MetaTransition(currentState, metaCurrentOptionalStates, symbol, nextState);
        }

        private static Dictionary<Label, int> GetStatesMap(SetsOfEquivalence setsOfEquivalence) =>
            setsOfEquivalence
                .SortedList.SelectMany(
                    (set, index) =>
                        set.Set.Select(item => new KeyValuePair<Label, int>(item, index))
                )
                .ToDictionary();

        private static Dictionary<Label, Label> GetOldNewStatesMap(
            SetsOfEquivalence setsOfEquivalence
        ) =>
            setsOfEquivalence
                .SortedList.SelectMany(item =>
                    item.Set.Select(subitem => new Tuple<Label, Label>(subitem, item.Set.First()))
                )
                .ToDictionary(item => item.Item1, item => item.Item2);

        private static SortedSet<Label> ExtractStates(IEnumerable<Transition> transitions)
        {
            SortedSet<Label> states = [];

            foreach (Transition transition in transitions)
            {
                states.Add(transition.CurrentState);
                states.Add(transition.NextState);
            }

            return states;
        }

        public StateMachine(
            Label initialState,
            IEnumerable<Label> finalStates,
            IEnumerable<Transition> transitions,
            IEnumerable<Label>? allStates = null
        )
        {
            FinalStates = finalStates.ToImmutableSortedSet();
            Transitions = transitions.ToImmutableSortedSet();
            SortedSet<Label> states = ExtractStates(Transitions);
            states.Add(initialState);

            if (allStates is not null)
            {
                SortedSet<Label> allStatesSet = allStates.ToSortedSet();

                if (!allStatesSet.IsSupersetOf(states))
                {
                    throw new ArgumentException(
                        "States are not the superset of real states.",
                        nameof(allStates)
                    );
                }

                States = allStatesSet.ToImmutableSortedSet();
            }
            else
            {
                States = states.ToImmutableSortedSet();
            }

            Alphabet = Transitions.Select(t => t.Symbol).ToImmutableSortedSet();
            InitialState = initialState;
        }
    }
}
