using System.Collections.Frozen;
using System.Collections.Immutable;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Extensions;
using FLaG.Core.Helpers;

namespace FLaG.Core.Data.Grammars
{
    [ComparableEquatable]
    public sealed partial class Grammar
    {
        internal const char _DefaultNonTerminalSymbol = 'S';

        public IImmutableSet<Rule> Rules { get; }

        public IImmutableSet<Symbol> Symbols { get; }

        public IImmutableSet<TerminalSymbol> Alphabet { get; }

        public IImmutableSet<NonTerminalSymbol> NonTerminals { get; }

        public NonTerminalSymbol Target { get; }

        public Grammar(IEnumerable<Rule> rules, NonTerminalSymbol target)
        {
            Rules = Normalize(rules).ToImmutableSortedSet();
            Alphabet = Rules.SelectMany(rule => rule.Alphabet).ToImmutableSortedSet();
            Target = target;
            NonTerminals = Rules
                .SelectMany(rule => rule.NonTerminals)
                .Concat([Target])
                .ToImmutableSortedSet();
            ;
            Symbols = NonTerminals.OfType<Symbol>().Intersect(Alphabet).ToImmutableSortedSet();
        }

        private static IEnumerable<Rule> Normalize(IEnumerable<Rule> rules)
        {
            return rules
                .GroupBy(r => r.Target)
                .Select(g => new Rule(g.SelectMany(r => r.Chains), g.Key));
        }

        private static IEnumerable<T> Enumerate<T>(GrammarType grammarType, IEnumerable<T> list) =>
            GrammarDispatcher.Dispatch(grammarType, () => list, () => list.Reverse());

        private SymbolTuple GetSymbolTuple(
            GrammarType grammarType,
            Chain chain,
            NonTerminalSymbol ruleTarget,
            NonTerminalSymbol? finalTarget = null
        )
        {
            int state = 0;
            Symbol? otherSymbol = null;
            NonTerminalSymbol? nonTerminalSymbol = null;
            TerminalSymbol? terminalSymbol = null;

            foreach (Symbol symbol in Enumerate(grammarType, chain))
            {
                switch (state)
                {
                    case 0:
                        otherSymbol = symbol;
                        state = 1;
                        break;
                    case 1:
                        nonTerminalSymbol =
                            otherSymbol as NonTerminalSymbol
                            ?? throw new InvalidOperationException(
                                "A non terminal symbol is expected."
                            );
                        terminalSymbol =
                            symbol as TerminalSymbol
                            ?? throw new InvalidOperationException(
                                "A terminal symbol is expected."
                            );
                        state = 2;
                        break;
                    default:
                        throw new InvalidOperationException("2 or less symbols are expected.");
                }
            }

            switch (state)
            {
                case 0:
                    if (ruleTarget != Target && finalTarget is not null)
                    {
                        throw new InvalidOperationException(
                            "Rule target must equal to grammar target."
                        );
                    }

                    return new SymbolTuple(ruleTarget, null, null);
                case 1:
                    nonTerminalSymbol = finalTarget is null
                        ? otherSymbol as NonTerminalSymbol
                        : finalTarget;
                    terminalSymbol = otherSymbol as TerminalSymbol;

                    if (terminalSymbol is null && nonTerminalSymbol is null)
                    {
                        throw new InvalidOperationException(
                            string.Format("The symbol type is not supported: {0}.", otherSymbol)
                        );
                    }

                    if (finalTarget is not null && terminalSymbol is null)
                    {
                        throw new InvalidOperationException(
                            "One symbol chain must contain terminal symbol."
                        );
                    }

                    goto case 2;
                case 2:
                default:
                    return new SymbolTuple(ruleTarget, terminalSymbol, nonTerminalSymbol);
            }
        }

        public RegExps.Expression MakeExpression(
            GrammarType grammarType,
            Action<Matrix>? onBegin = null,
            Action<Matrix>? onIterate = null
        )
        {
            Dictionary<NonTerminalSymbol, int> nonTerminalIndexMap = NonTerminals
                .Select((s, i) => new KeyValuePair<NonTerminalSymbol, int>(s, i))
                .ToDictionary();

            RegExps.Expression?[,] expressions = new RegExps.Expression[
                nonTerminalIndexMap.Count,
                nonTerminalIndexMap.Count + 1
            ];

            foreach (Rule rule in Rules)
            {
                int rowNumber = nonTerminalIndexMap[rule.Target];

                foreach (Chain chain in rule.Chains)
                {
                    SymbolTuple symbolTuple = GetSymbolTuple(grammarType, chain, rule.Target);

                    RegExps.Expression expression;
                    int columnNumber;

                    if (symbolTuple.TerminalSymbol is null)
                    {
                        expression = new RegExps.Empty();
                    }
                    else
                    {
                        expression = new RegExps.Symbol(symbolTuple.TerminalSymbol.Symbol);
                    }

                    if (symbolTuple.NonTerminalSymbol is null)
                    {
                        columnNumber = expressions.GetLength(1) - 1;
                    }
                    else
                    {
                        columnNumber = nonTerminalIndexMap[symbolTuple.NonTerminalSymbol];
                    }

                    if (expressions[rowNumber, columnNumber] is not null)
                    {
                        expressions[rowNumber, columnNumber] = new RegExps.BinaryUnion(
                            expression,
                            expressions[rowNumber, columnNumber]!
                        );
                    }
                    else
                    {
                        expressions[rowNumber, columnNumber] = expression;
                    }
                }

                for (int i = 0; i < expressions.GetLength(1); i++)
                {
                    expressions[rowNumber, i] = expressions[rowNumber, i]?.Optimize();
                }
            }

            Matrix matrix = new(
                expressions,
                nonTerminalIndexMap.OrderBy(kv => kv.Value).Select(kv => kv.Key),
                grammarType
            );

            return matrix.Solve(nonTerminalIndexMap[Target], onBegin, onIterate);
        }

        public StateMachine MakeStateMachine(GrammarType grammarType)
        {
            HashSet<Transition> transitions = [];

            NonTerminalSymbol additionalState = GetNewNonTerminal(NonTerminals);

            HashSet<Label> finalStates = GrammarDispatcher.Dispatch<HashSet<Label>>(
                grammarType,
                () => [Target.Label],
                () => [additionalState.Label]
            );

            foreach (Rule rule in Rules)
            {
                foreach (Chain chain in rule.Chains)
                {
                    SymbolTuple symbolTuple = GetSymbolTuple(
                        grammarType,
                        chain,
                        rule.Target,
                        additionalState
                    );

                    if (symbolTuple.NonTerminalSymbol is null)
                    {
                        finalStates.Add(
                            GrammarDispatcher.Dispatch(
                                grammarType,
                                () => additionalState.Label,
                                () => Target.Label
                            )
                        );
                    }
                    else
                    {
                        (Label currentState, Label nextState) = GrammarDispatcher.Dispatch(
                            grammarType,
                            () => (symbolTuple.NonTerminalSymbol.Label, symbolTuple.Target.Label),
                            () => (symbolTuple.Target.Label, symbolTuple.NonTerminalSymbol.Label)
                        );

                        transitions.Add(
                            new Transition(
                                currentState,
                                symbolTuple.TerminalSymbol!.Symbol,
                                nextState
                            )
                        );
                    }
                }
            }

            return GrammarDispatcher.Dispatch(
                grammarType,
                () => new StateMachine(additionalState.Label, finalStates, transitions),
                () => new StateMachine(Target.Label, finalStates, transitions)
            );
        }

        public Grammar MakeStateMachineGrammar(
            GrammarType grammarType,
            Action<IReadOnlySet<Rule>>? onBegin = null,
            Action<MakeStateMachineGrammarPostReport>? onIterate = null
        )
        {
            SortedSet<Rule> newRules = [];

            onBegin?.Invoke(newRules.AsReadOnly());

            HashSet<NonTerminalSymbol> newNonTerminals = NonTerminals.ToHashSet();

            foreach (Rule rule in Rules)
            {
                foreach (Chain chain in rule.Chains)
                {
                    SortedSet<Rule> newChainRules = [];

                    int state = 0;
                    NonTerminalSymbol? nonTerminalSymbol = null;
                    Symbol? otherSymbol = null;
                    Chain newChain;

                    foreach (Symbol symbol in Enumerate(grammarType, chain))
                    {
                        switch (state)
                        {
                            case 0:
                                nonTerminalSymbol = symbol as NonTerminalSymbol;
                                otherSymbol = symbol;

                                if (nonTerminalSymbol is not null)
                                {
                                    state = 1;
                                }
                                else
                                {
                                    state = 2;
                                }

                                break;
                            case 1:
                                otherSymbol = symbol;
                                state = 3;
                                break;
                            case 2:
                                newChain = new([otherSymbol!]);
                                newNonTerminals.Add(
                                    nonTerminalSymbol = GetNewNonTerminal(newNonTerminals)
                                );
                                newChainRules.Add(new([newChain], nonTerminalSymbol));
                                otherSymbol = symbol;
                                state = 3;
                                break;
                            default:
                                newChain = new(
                                    Enumerate(grammarType, [nonTerminalSymbol!, otherSymbol!])
                                );
                                newNonTerminals.Add(
                                    nonTerminalSymbol = GetNewNonTerminal(newNonTerminals)
                                );
                                newChainRules.Add(new([newChain], nonTerminalSymbol));
                                otherSymbol = symbol;
                                break;
                        }
                    }

                    newChain = state switch
                    {
                        0 => new([]),
                        1 or 2 => new([otherSymbol!]),
                        _ => new(Enumerate(grammarType, [nonTerminalSymbol!, otherSymbol!])),
                    };
                    newChainRules.Add(new([newChain], rule.Target));
                    newChainRules = Normalize(newChainRules).ToSortedSet();

                    SortedSet<Rule> previousRules = newRules.ToSortedSet();

                    foreach (Rule newChainRule in newChainRules)
                    {
                        newRules.Add(newChainRule);
                    }

                    onIterate?.Invoke(
                        new(
                            rule.Target,
                            chain,
                            previousRules,
                            newChainRules,
                            newRules,
                            newChainRules.Count > 1
                        )
                    );
                }
            }

            return new(newRules, Target);
        }

        private static FrozenDictionary<
            NonTerminalSymbol,
            IReadOnlySet<NonTerminalSymbol>
        > ToDeeplySorted(Dictionary<NonTerminalSymbol, HashSet<NonTerminalSymbol>> dictionary)
        {
            Dictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> result = [];

            foreach (
                KeyValuePair<
                    NonTerminalSymbol,
                    HashSet<NonTerminalSymbol>
                > keyValuePair in dictionary
            )
            {
                result.Add(keyValuePair.Key, keyValuePair.Value.ToFrozenSet());
            }

            return result.ToFrozenDictionary();
        }

        public bool RemoveChainRules(
            out Grammar grammar,
            Action<ChainRulesBeginPostReport>? onBegin = null,
            Action<ChainRulesIterationPostReport>? onIterate = null,
            Action<ChainRulesEndPostReport>? onEnd = null
        )
        {
            Dictionary<NonTerminalSymbol, HashSet<NonTerminalSymbol>> newSymbolSetMap =
                NonTerminals.ToDictionary(s => s, s => new HashSet<NonTerminalSymbol>([s]));

            Dictionary<NonTerminalSymbol, ChainRulesEndTuple> unchangeableSymbolSetMap = [];

            Dictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            onBegin?.Invoke(new ChainRulesBeginPostReport(0, ToDeeplySorted(newSymbolSetMap)));

            int i = 0;

            do
            {
                ++i;

                Dictionary<NonTerminalSymbol, HashSet<NonTerminalSymbol>> nextSymbolSetMap =
                    newSymbolSetMap;
                newSymbolSetMap = nextSymbolSetMap.Keys.ToDictionary(
                    s => s,
                    s => new HashSet<NonTerminalSymbol>()
                );

                foreach (
                    KeyValuePair<
                        NonTerminalSymbol,
                        HashSet<NonTerminalSymbol>
                    > nextSymbolSet in nextSymbolSetMap
                )
                {
                    foreach (NonTerminalSymbol nonTerminalSymbol in nextSymbolSet.Value)
                    {
                        if (targetRuleMap.TryGetValue(nonTerminalSymbol, out Rule? rule))
                        {
                            foreach (Chain chain in rule.Chains.Where(c => c.Sequence.Count == 1))
                            {
                                NonTerminalSymbol? symbol = chain.Sequence[0] as NonTerminalSymbol;
                                HashSet<NonTerminalSymbol> newSymbolSet = newSymbolSetMap[
                                    nextSymbolSet.Key
                                ];
                                if (
                                    symbol is not null
                                    && !newSymbolSet.Contains(symbol)
                                    && !nextSymbolSet.Value.Contains(symbol)
                                )
                                {
                                    newSymbolSet.Add(symbol);
                                }
                            }
                        }
                    }
                }

                Dictionary<NonTerminalSymbol, HashSet<NonTerminalSymbol>> previousSymbolSetMap =
                    nextSymbolSetMap.ToDictionary(kv => kv.Key, kv => kv.Value);
                Dictionary<NonTerminalSymbol, ChainRulesIterationTuple> symbolMap = [];

                foreach (
                    KeyValuePair<
                        NonTerminalSymbol,
                        HashSet<NonTerminalSymbol>
                    > symbolSet in previousSymbolSetMap
                )
                {
                    HashSet<NonTerminalSymbol> newSymbolSet = newSymbolSetMap[symbolSet.Key];
                    HashSet<NonTerminalSymbol> nonTerminalSet = nextSymbolSetMap[symbolSet.Key];

                    if (newSymbolSet.Count == 0)
                    {
                        unchangeableSymbolSetMap[symbolSet.Key] = new(
                            nonTerminalSet,
                            nonTerminalSet.Where(m => m != symbolSet.Key).ToSortedSet(),
                            i
                        );

                        symbolMap.Add(
                            symbolSet.Key,
                            new(true, symbolSet.Value, newSymbolSet, nonTerminalSet)
                        );
                        nextSymbolSetMap.Remove(symbolSet.Key);
                    }
                    else
                    {
                        HashSet<NonTerminalSymbol> nextSymbolSet = nonTerminalSet;
                        nextSymbolSet.UnionWith(newSymbolSet);
                        symbolMap.Add(
                            symbolSet.Key,
                            new(false, symbolSet.Value, newSymbolSet, nextSymbolSet)
                        );
                    }
                }

                onIterate?.Invoke(new(i, symbolMap, symbolMap.All(kv => kv.Value.IsLastIteration)));

                newSymbolSetMap = symbolMap
                    .Where(kv => !kv.Value.IsLastIteration)
                    .ToDictionary(kv => kv.Key, kv => kv.Value.Next.ToHashSet());
            } while (newSymbolSetMap.Count > 0);

            HashSet<Rule> newRules = [];

            foreach (Rule rule in Rules)
            {
                HashSet<Chain> newChains = [];

                foreach (Chain chain in rule.Chains)
                {
                    if (chain.Sequence.Count != 1 || chain.Sequence[0] is not NonTerminalSymbol)
                    {
                        newChains.Add(chain);
                    }
                }

                if (newChains.Count != 0)
                {
                    newRules.Add(new(newChains, rule.Target));
                }
            }

            onEnd?.Invoke(new(unchangeableSymbolSetMap));

            HashSet<Rule> additionalRules = [];

            foreach (Rule rule in newRules)
            {
                foreach (
                    KeyValuePair<
                        NonTerminalSymbol,
                        ChainRulesEndTuple
                    > symbolSet in unchangeableSymbolSetMap
                )
                {
                    if (symbolSet.Value.FinalNonTerminals.Contains(rule.Target))
                    {
                        additionalRules.Add(new Rule(rule.Chains, symbolSet.Key));
                    }
                }
            }

            newRules = Normalize(newRules.Concat(additionalRules)).ToHashSet();

            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        public bool RemoveEmptyRules(
            out Grammar grammar,
            Action<BeginPostReport<NonTerminalSymbol>>? onBegin = null,
            Action<IterationPostReport<NonTerminalSymbol>>? onIterate = null
        )
        {
            int i = 0;

            HashSet<NonTerminalSymbol> newSymbolSet = Rules
                .Where(r => r.Chains.Any(c => c.Count == 0))
                .Select(r => r.Target)
                .ToHashSet();

            onBegin?.Invoke(new BeginPostReport<NonTerminalSymbol>(i, newSymbolSet));

            bool isAddedSomething;

            do
            {
                ++i;

                isAddedSomething = false;
                HashSet<NonTerminalSymbol> nextSymbolSet = newSymbolSet;
                newSymbolSet = [];

                foreach (Rule rule in Rules)
                {
                    foreach (Chain chain in rule.Chains)
                    {
                        bool isAllSymbolsInNext = true;

                        foreach (Symbol symbol in chain.Sequence)
                        {
                            if (
                                symbol is not NonTerminalSymbol nonTerminal
                                || !nextSymbolSet.Contains(nonTerminal)
                            )
                            {
                                isAllSymbolsInNext = false;
                                break;
                            }
                        }

                        if (isAllSymbolsInNext)
                        {
                            if (
                                !newSymbolSet.Contains(rule.Target)
                                && !nextSymbolSet.Contains(rule.Target)
                            )
                            {
                                newSymbolSet.Add(rule.Target);
                                isAddedSomething = true;
                            }
                        }
                    }
                }

                HashSet<NonTerminalSymbol> previousSymbolSet = nextSymbolSet.ToHashSet();
                nextSymbolSet.UnionWith(newSymbolSet);

                onIterate?.Invoke(
                    new IterationPostReport<NonTerminalSymbol>(
                        i,
                        previousSymbolSet,
                        newSymbolSet,
                        nextSymbolSet,
                        !isAddedSomething
                    )
                );

                newSymbolSet = nextSymbolSet;
            } while (isAddedSomething);

            HashSet<Rule> newRules = [];

            foreach (Rule rule in Rules)
            {
                newRules.Add(
                    new Rule(
                        rule.Chains.SelectMany(c =>
                            ForkByEmpty(c, newSymbolSet).Where(chain => chain != Chain.Empty)
                        ),
                        rule.Target
                    )
                );
            }

            Dictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            NonTerminalSymbol newTarget = Target;

            if (newSymbolSet.Contains(Target))
            {
                newTarget = GetNewNonTerminal(NonTerminals);
                newRules.Add(new([new([Target]), Chain.Empty], newTarget));
            }

            newRules = Normalize(newRules).ToHashSet();

            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, newTarget);

                return true;
            }

            grammar = this;

            return false;
        }

        internal static NonTerminalSymbol GetNewNonTerminal(
            IEnumerable<NonTerminalSymbol> nonTerminals
        )
        {
            return new NonTerminalSymbol(
                new Label(
                    new SingleLabel(
                        _DefaultNonTerminalSymbol,
                        nonTerminals
                            .Where(s => s.Label.LabelType == LabelType.Simple)
                            .Max(s => s.Label.ExtractSingleLabel().SignIndex ?? 0) + 1
                    )
                )
            );
        }

        private static IEnumerable<Chain> ForkByEmpty(
            Chain chain,
            ISet<NonTerminalSymbol> symbolSet
        )
        {
            HashSet<Symbol> symbolSetInternal = [];
            foreach (NonTerminalSymbol nonTerminalSymbol in symbolSet)
            {
                symbolSetInternal.Add(nonTerminalSymbol);
            }

            List<Symbol> symbols = new(chain.Sequence.Count);
            Stack<bool> stack = [];

            int i = 0;

            do
            {
                while (i < chain.Sequence.Count)
                {
                    if (symbolSetInternal.Contains(chain.Sequence[i]))
                    {
                        stack.Push(true);
                    }

                    symbols.Add(chain.Sequence[i]);

                    ++i;
                }

                yield return new(symbols);

                do
                {
                    while (i > 0 && !symbolSetInternal.Contains(chain.Sequence[i - 1]))
                    {
                        symbols.RemoveAt(symbols.Count - 1);
                        --i;
                    }

                    if (i > 0)
                    {
                        bool value = stack.Pop();

                        if (value)
                        {
                            symbols.RemoveAt(symbols.Count - 1);
                            stack.Push(false);
                            break;
                        }
                        else
                        {
                            --i;
                        }
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            } while (stack.Count > 0);
        }

        public bool RemoveUnreachableSymbols(
            out Grammar grammar,
            Action<BeginPostReport<Symbol>>? onBegin = null,
            Action<IterationPostReport<Symbol>>? onIterate = null
        )
        {
            int i = 0;

            HashSet<Symbol> newSymbolSet = [];
            newSymbolSet.Add(Target);

            onBegin?.Invoke(new BeginPostReport<Symbol>(i, newSymbolSet));

            Dictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            bool isAddedSomething;

            do
            {
                ++i;

                isAddedSomething = false;

                HashSet<Symbol> nextSymbolSet = newSymbolSet;
                newSymbolSet = [];

                foreach (Symbol symbol in nextSymbolSet)
                {
                    NonTerminalSymbol? nonTerminalSymbol = symbol as NonTerminalSymbol;

                    if (
                        nonTerminalSymbol is not null
                        && targetRuleMap.TryGetValue(nonTerminalSymbol, out Rule? value)
                    )
                    {
                        foreach (Chain chain in value.Chains)
                        {
                            foreach (Symbol chainSymbol in chain.Sequence)
                            {
                                if (
                                    !nextSymbolSet.Contains(chainSymbol)
                                    && !newSymbolSet.Contains(chainSymbol)
                                )
                                {
                                    isAddedSomething = true;
                                    newSymbolSet.Add(chainSymbol);
                                }
                            }
                        }
                    }
                }

                HashSet<Symbol> previousSymbolSet = nextSymbolSet.ToHashSet();
                nextSymbolSet.UnionWith(newSymbolSet);

                onIterate?.Invoke(
                    new IterationPostReport<Symbol>(
                        i,
                        previousSymbolSet,
                        newSymbolSet,
                        nextSymbolSet,
                        !isAddedSomething
                    )
                );

                newSymbolSet = nextSymbolSet;
            } while (isAddedSomething);

            IEnumerable<Rule> newRules = Rules.Where(r => newSymbolSet.Contains(r.Target));

            if (newRules.Count() < Rules.Count)
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        public bool RemoveUselessSymbols(
            out Grammar grammar,
            Action<BeginPostReport<NonTerminalSymbol>>? onBegin = null,
            Action<IterationPostReport<NonTerminalSymbol>>? onIterate = null
        )
        {
            int i = 0;

            HashSet<NonTerminalSymbol> newNonTerminalSet = [];

            onBegin?.Invoke(new BeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));

            bool isAddedSomething;

            do
            {
                ++i;
                isAddedSomething = false;
                HashSet<NonTerminalSymbol> nextNonTerminalSet = newNonTerminalSet;
                newNonTerminalSet = [];

                foreach (Rule rule in Rules)
                {
                    foreach (Chain chain in rule.Chains)
                    {
                        if (chain.All(s => TerminalOrSetContains(s, nextNonTerminalSet)))
                        {
                            if (
                                !newNonTerminalSet.Contains(rule.Target)
                                && !nextNonTerminalSet.Contains(rule.Target)
                            )
                            {
                                newNonTerminalSet.Add(rule.Target);
                                isAddedSomething = true;
                            }

                            break;
                        }
                    }
                }

                HashSet<NonTerminalSymbol> previousNonTerminalSet = nextNonTerminalSet.ToHashSet();
                nextNonTerminalSet.UnionWith(newNonTerminalSet);

                onIterate?.Invoke(
                    new IterationPostReport<NonTerminalSymbol>(
                        i,
                        previousNonTerminalSet,
                        newNonTerminalSet,
                        nextNonTerminalSet,
                        !isAddedSomething
                    )
                );

                newNonTerminalSet = nextNonTerminalSet;
            } while (isAddedSomething);

            HashSet<Rule> newRules = Normalize(
                    GetRulesWithoutUselessSymbols(Rules, newNonTerminalSet)
                )
                .ToHashSet();

            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        private static IEnumerable<Rule> GetRulesWithoutUselessSymbols(
            IEnumerable<Rule> rules,
            HashSet<NonTerminalSymbol> nonTerminalSet
        )
        {
            foreach (Rule rule in rules)
            {
                if (!nonTerminalSet.Contains(rule.Target))
                {
                    continue;
                }

                HashSet<Chain> newChains = [];

                foreach (Chain chain in rule.Chains)
                {
                    if (chain.All(s => TerminalOrSetContains(s, nonTerminalSet)))
                    {
                        newChains.Add(chain);
                    }
                }

                if (newChains.Count > 0)
                {
                    yield return new Rule(newChains, rule.Target);
                }
            }
        }

        private static bool TerminalOrSetContains(
            Symbol symbol,
            HashSet<NonTerminalSymbol> nonTerminalSet
        )
        {
            NonTerminalSymbol? nonTerminalSymbol = symbol as NonTerminalSymbol;
            return nonTerminalSymbol is null || nonTerminalSet.Contains(nonTerminalSymbol);
        }

        public bool IsLangEmpty(
            Action<BeginPostReport<NonTerminalSymbol>>? onBegin = null,
            Action<IterationPostReport<NonTerminalSymbol>>? onIterate = null
        )
        {
            int i = 0;

            HashSet<NonTerminalSymbol> newNonTerminalSet = [];

            onBegin?.Invoke(new BeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));

            bool isAddedSomething;

            do
            {
                ++i;

                isAddedSomething = false;

                HashSet<NonTerminalSymbol> nextNonTerminalSet = newNonTerminalSet;
                newNonTerminalSet = [];

                foreach (Rule rule in Rules)
                {
                    foreach (Chain chain in rule.Chains)
                    {
                        bool isOurChain = true;

                        foreach (Symbol symbol in chain)
                        {
                            NonTerminalSymbol? nonTerminalSymbol = symbol as NonTerminalSymbol;

                            if (
                                nonTerminalSymbol is not null
                                && !nextNonTerminalSet.Contains(nonTerminalSymbol)
                            )
                            {
                                isOurChain = false;
                                break;
                            }
                        }

                        if (isOurChain)
                        {
                            if (
                                !newNonTerminalSet.Contains(rule.Target)
                                && !nextNonTerminalSet.Contains(rule.Target)
                            )
                            {
                                newNonTerminalSet.Add(rule.Target);
                                isAddedSomething = true;
                            }
                            break;
                        }
                    }
                }

                HashSet<NonTerminalSymbol> previousNonTerminalSet = nextNonTerminalSet.ToHashSet();
                nextNonTerminalSet.UnionWith(newNonTerminalSet);

                onIterate?.Invoke(
                    new IterationPostReport<NonTerminalSymbol>(
                        i,
                        previousNonTerminalSet,
                        newNonTerminalSet,
                        nextNonTerminalSet,
                        !isAddedSomething
                    )
                );

                newNonTerminalSet = nextNonTerminalSet;
            } while (isAddedSomething);

            return !newNonTerminalSet.Contains(Target);
        }

        public Grammar Reorganize(IImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol> map)
        {
            return new Grammar(Rules.Select(rule => rule.Reorganize(map)), map[Target]);
        }

        public Grammar Reorganize(int firstIndex)
        {
            int index = firstIndex;
            ImmutableDictionary<NonTerminalSymbol, NonTerminalSymbol>.Builder map =
                ImmutableDictionary.CreateBuilder<NonTerminalSymbol, NonTerminalSymbol>();
            foreach (NonTerminalSymbol symbol in NonTerminals)
            {
                map.Add(symbol, new(new(new SingleLabel(_DefaultNonTerminalSymbol, index++))));
            }
            return Reorganize(map.ToImmutable());
        }

        internal void SplitRules(
            out ImmutableSortedSet<Rule> terminalSymbolsOnlyRules,
            out ImmutableSortedSet<Rule> otherRules
        )
        {
            ImmutableSortedSet<Rule>.Builder terminalSymbolsOnlyRulesInternal =
                ImmutableSortedSet.CreateBuilder<Rule>();
            ImmutableSortedSet<Rule>.Builder otherRulesInternal =
                ImmutableSortedSet.CreateBuilder<Rule>();

            foreach (Rule rule in Rules)
            {
                ImmutableSortedSet<Chain>.Builder terminalSymbolsOnlyChains =
                    ImmutableSortedSet.CreateBuilder<Chain>();
                ImmutableSortedSet<Chain>.Builder otherChains =
                    ImmutableSortedSet.CreateBuilder<Chain>();

                foreach (Chain chain in rule.Chains)
                {
                    if (chain.All(symbol => symbol.SymbolType == SymbolType.Terminal))
                    {
                        terminalSymbolsOnlyChains.Add(chain);
                    }
                    else
                    {
                        otherChains.Add(chain);
                    }
                }

                if (terminalSymbolsOnlyChains.Count > 0)
                {
                    terminalSymbolsOnlyRulesInternal.Add(
                        new Rule(terminalSymbolsOnlyChains.ToImmutable(), rule.Target)
                    );
                }

                if (otherChains.Count > 0)
                {
                    otherRulesInternal.Add(new Rule(otherChains.ToImmutable(), rule.Target));
                }
            }

            terminalSymbolsOnlyRules = terminalSymbolsOnlyRulesInternal.ToImmutable();
            otherRules = otherRulesInternal.ToImmutable();
        }

        public int FetchHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(Target);
            foreach (Rule rule in Rules)
            {
                hashCode.Add(rule);
            }
            return hashCode.ToHashCode();
        }

        public bool EqualsNonnull(Grammar other)
        {
            return Target.Equals(other.Target) && Rules.SequenceEqual(other.Rules);
        }

        public int CompareToNonnull(Grammar other)
        {
            int result = Target.CompareTo(other.Target);

            if (result != 0)
            {
                return result;
            }

            return Rules.SequenceCompare(other.Rules);
        }

        public override string ToString()
        {
            return string.Format(
                "N = {0}, Σ = {1}, S = {2}, P = {3}",
                string.Concat("{", string.Join(",", NonTerminals), "}"),
                string.Concat("{", string.Join(",", Alphabet), "}"),
                Target,
                string.Concat("{", string.Join(",", Rules), "}")
            );
        }
    }
}
