using FLaGLib.Collections;
using FLaGLib.Data.Helpers;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Grammars
{
    public class Grammar : IComparable<Grammar>, IEquatable<Grammar>
    {
        internal const char _DefaultNonTerminalSymbol = 'S';
        internal const string GrammarIsNotSupportedMessage = "Grammar type {0} is not supported.";

        public IReadOnlySet<Rule> Rules
        {
            get;
            private set;
        }

        public IReadOnlySet<Symbol> Symbols
        {
            get;
            private set;
        }

        public IReadOnlySet<TerminalSymbol> Alphabet
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> NonTerminals
        {
            get;
            private set;
        }

        public NonTerminalSymbol Target
        {
            get;
            private set;
        }

        public Grammar(IEnumerable<Rule> rules, NonTerminalSymbol target)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (rules.AnyNull())
            {
                throw new ArgumentException("At least one rule is null.");
            }

            Rules = Normalize(rules).AsReadOnly();
            Alphabet = Rules.SelectMany(rule => rule.Alphabet).ToSortedSet().AsReadOnly();
            Target = target;
            ISet<NonTerminalSymbol> nonTerminals = Rules.SelectMany(rule => rule.NonTerminals).ToSortedSet();
            nonTerminals.Add(Target);
            NonTerminals = nonTerminals.AsReadOnly();
            Symbols = NonTerminals.OfType<Symbol>().Intersect(Alphabet).ToSortedSet().AsReadOnly();
        }
       

        private static ISet<Rule> Normalize(IEnumerable<Rule> rules)
        {
            return rules.GroupBy(r => r.Target).Select(g => new Rule(g.SelectMany(r => r.Chains), g.Key)).ToSortedSet();
        }

        private static IEnumerable<T> Enumerate<T>(GrammarType grammarType, params T[] items)
        {
            switch (grammarType)
            {
                case GrammarType.Left:
                    return EnumerateHelper.Sequence(items);
                case GrammarType.Right:
                    return EnumerateHelper.ReverseSequence(items);
                default:
                    throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
            }
        }

        private static IEnumerable<T> Enumerate<T>(IReadOnlyList<T> list, GrammarType grammarType)
        {
            switch (grammarType)
            {
                case GrammarType.Left:
                    return list;
                case GrammarType.Right:
                    return list.FastReverse();
                default:
                    throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
            }
        }

        private SymbolTuple GetSymbolTuple(GrammarType grammarType, Chain chain, NonTerminalSymbol ruleTarget, NonTerminalSymbol finalTarget = null)
        {
            int state = 0;
            Symbol otherSymbol = null;
            NonTerminalSymbol nonTerminalSymbol = null;
            TerminalSymbol terminalSymbol = null;

            foreach (Symbol symbol in Enumerate(chain, grammarType))
            {
                switch (state)
                {
                    case 0:
                        otherSymbol = symbol;
                        state = 1;
                        break;
                    case 1:
                        nonTerminalSymbol = otherSymbol.As<NonTerminalSymbol>();

                        if (nonTerminalSymbol == null)
                        {
                            throw new InvalidOperationException("Expected non terminal symbol.");
                        }

                        terminalSymbol = symbol.As<TerminalSymbol>();

                        if (terminalSymbol == null)
                        {
                            throw new InvalidOperationException("Expected terminal symbol.");
                        }

                        state = 2;
                        break;
                    default:
                        throw new InvalidOperationException("Expected 2 or less symbols.");
                }
            }

            switch (state)
            {
                case 0:
                    if (ruleTarget != Target && finalTarget != null)
                    {
                        throw new InvalidOperationException("Expected that rule target equals to grammar target.");
                    }

                    return new SymbolTuple(ruleTarget, null, null);
                case 1:
                    nonTerminalSymbol = finalTarget != null ? finalTarget : otherSymbol.As<NonTerminalSymbol>();
                    terminalSymbol = otherSymbol.As<TerminalSymbol>();

                    if (terminalSymbol == null && nonTerminalSymbol == null)
                    {
                        throw new InvalidOperationException(string.Format("The symbol type is not supported symbol type: {0}.", otherSymbol));
                    }

                    if (finalTarget != null && terminalSymbol == null)
                    {
                        throw new InvalidOperationException("One symbol chain must contain terminal symbol.");
                    }

                    goto case 2;
                case 2:
                default:
                    return new SymbolTuple(ruleTarget, terminalSymbol, nonTerminalSymbol);
            }
        }

        public RegExps.Expression MakeExpression(GrammarType grammarType, Action<Matrix> onBegin = null, Action<Matrix> onIterate = null)
        {
            IDictionary<NonTerminalSymbol, int> nonTerminalIndexMap = NonTerminals.Select((s, i) => new KeyValuePair<NonTerminalSymbol, int>(s, i)).ToDictionary();

            RegExps.Expression[,] expressions = new RegExps.Expression[nonTerminalIndexMap.Count, nonTerminalIndexMap.Count + 1];

            foreach (Rule rule in Rules)
            {
                int rowNumber = nonTerminalIndexMap[rule.Target];

                foreach (Chain chain in rule.Chains)
                {
                    SymbolTuple symbolTuple = GetSymbolTuple(grammarType, chain, rule.Target);

                    RegExps.Expression expression;
                    int columnNumber;

                    if (symbolTuple.TerminalSymbol == null)
                    {
                        expression = RegExps.Empty.Instance;
                    }
                    else
                    {
                        expression = new RegExps.Symbol(symbolTuple.TerminalSymbol.Symbol);
                    }

                    if (symbolTuple.NonTerminalSymbol == null)
                    {
                        columnNumber = expressions.GetLength(1) - 1;
                    }
                    else
                    {
                        columnNumber = nonTerminalIndexMap[symbolTuple.NonTerminalSymbol];
                    }

                    if (expressions[rowNumber, columnNumber] == null)
                    {
                        expressions[rowNumber, columnNumber] = expression;
                    }
                    else
                    {
                        expressions[rowNumber, columnNumber] = new RegExps.BinaryUnion(expression, expressions[rowNumber, columnNumber]);
                    }
                }

                for (int i = 0; i < expressions.GetLength(1); i++)
                {
                    expressions[rowNumber, i] = expressions[rowNumber, i]?.Optimize();
                }
            }

            Matrix matrix = new Matrix(expressions, nonTerminalIndexMap.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList().AsReadOnly(), grammarType);
            
            return matrix.Solve(nonTerminalIndexMap[Target], onBegin, onIterate);
        }

        public StateMachine MakeStateMachine(GrammarType grammarType)
        {
            ISet<Transition> transitions = new HashSet<Transition>();
            ISet<Label> finalStates = new HashSet<Label>();

            NonTerminalSymbol additionalState = GetNewNonTerminal(NonTerminals);
            
            switch (grammarType)
            {
                case GrammarType.Left:
                    finalStates.Add(Target.Label);
                    break;
                case GrammarType.Right:
                    finalStates.Add(additionalState.Label);
                    break;
                default:
                    throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
            }

            foreach (Rule rule in Rules)
            {
                foreach (Chain chain in rule.Chains)
                {
                    SymbolTuple symbolTuple = GetSymbolTuple(grammarType, chain, rule.Target, additionalState);

                    if (symbolTuple.NonTerminalSymbol == null && symbolTuple.NonTerminalSymbol == null)
                    {
                        switch (grammarType)
                        {
                            case GrammarType.Left:
                                finalStates.Add(additionalState.Label);
                                break;
                            case GrammarType.Right:
                                finalStates.Add(Target.Label);
                                break;
                            default:
                                throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
                        }
                    }
                    else
                    {
                        Label currentState;
                        Label nextState;

                        switch (grammarType)
                        {
                            case GrammarType.Left:
                                currentState = symbolTuple.NonTerminalSymbol.Label;
                                nextState = symbolTuple.Target.Label;
                                break;
                            case GrammarType.Right:
                                currentState = symbolTuple.Target.Label;
                                nextState = symbolTuple.NonTerminalSymbol.Label;
                                break;
                            default:
                                throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
                        }

                        transitions.Add(new Transition(currentState, symbolTuple.TerminalSymbol.Symbol, nextState));
                    }                  
                }
            }

            StateMachine stateMachine;

            switch (grammarType)
            {
                case GrammarType.Left:
                    stateMachine = new StateMachine(additionalState.Label, finalStates, transitions);
                    break;
                case GrammarType.Right:
                    stateMachine = new StateMachine(Target.Label, finalStates, transitions);
                    break;
                default:
                    throw new NotSupportedException(string.Format(GrammarIsNotSupportedMessage, grammarType));
            }

            return stateMachine;
        }

        public Grammar MakeStateMachineGrammar(GrammarType grammarType, 
            Action<IReadOnlySet<Rule>> onBegin = null,
            Action<MakeStateMachineGrammarPostReport> onIterate = null)
        {
            ISet<Rule> newRules = new HashSet<Rule>();

            if (onBegin != null)
            {
                onBegin(newRules.AsReadOnly());
            }

            ISet<NonTerminalSymbol> newNonTerminals = new HashSet<NonTerminalSymbol>(NonTerminals);

            foreach (Rule rule in Rules)
            {
                foreach (Chain chain in rule.Chains)
                {
                    ISet<Rule> newChainRules = new HashSet<Rule>();

                    int state = 0;
                    NonTerminalSymbol nonTerminalSymbol = null;
                    Symbol otherSymbol = null;
                    Chain newChain;

                    foreach (Symbol symbol in Enumerate(chain,grammarType))
                    {
                        switch (state)
                        {
                            case 0:
                                nonTerminalSymbol = symbol.As<NonTerminalSymbol>();
                                otherSymbol = symbol;

                                if (nonTerminalSymbol != null)
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
                                newChain = new Chain(otherSymbol.AsSequence());
                                newNonTerminals.Add(nonTerminalSymbol = GetNewNonTerminal(newNonTerminals));
                                newChainRules.Add(new Rule(newChain.AsSequence(), nonTerminalSymbol));
                                otherSymbol = symbol;
                                state = 3;
                                break;
                            default:
                                newChain = new Chain(Enumerate(grammarType, nonTerminalSymbol, otherSymbol));
                                newNonTerminals.Add(nonTerminalSymbol = GetNewNonTerminal(newNonTerminals));
                                newChainRules.Add(new Rule(newChain.AsSequence(), nonTerminalSymbol));
                                otherSymbol = symbol;
                                break;
                        }
                    }

                    switch (state)
                    {
                        case 0:
                            newChain = Chain.Empty;
                            break;
                        case 1:
                        case 2:
                            newChain = new Chain(otherSymbol.AsSequence());
                            break;
                        default:
                            newChain = new Chain(Enumerate(grammarType, nonTerminalSymbol, otherSymbol));
                            break;
                    }

                    newChainRules.Add(new Rule(newChain.AsSequence(), rule.Target));
                    newChainRules = Normalize(newChainRules);

                    ISet<Rule> previousRules = newRules.ToHashSet();

                    newRules.AddRange(newChainRules);

                    if (onIterate != null)
                    {
                        onIterate(new MakeStateMachineGrammarPostReport(rule.Target, chain, previousRules, newChainRules, newRules, newChainRules.Count > 1));
                    }
                }
            }

            return new Grammar(newRules, Target);
        }

        public bool RemoveChainRules(out Grammar grammar,
            Action<ChainRulesBeginPostReport> onBegin = null,
            Action<ChainRulesIterationPostReport> onIterate = null,
            Action<ChainRulesEndPostReport> onEnd = null)
        {
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> newSymbolSetMap =
                NonTerminals.ToDictionary(s => s, 
                    s => new HashSet<NonTerminalSymbol>(s.AsSequence()).Of<ISet<NonTerminalSymbol>>());

            IDictionary<NonTerminalSymbol, ChainRulesEndTuple> unchangeableSymbolSetMap = 
                new Dictionary<NonTerminalSymbol, ChainRulesEndTuple>();

            IDictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            if (onBegin != null)
            {
                onBegin(new ChainRulesBeginPostReport(0, newSymbolSetMap));
            }

            int i = 0;

            do
            {
                i++;

                IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> nextSymbolSetMap = newSymbolSetMap;
                newSymbolSetMap = nextSymbolSetMap.Keys.ToDictionary(s => s, s => new HashSet<NonTerminalSymbol>().Of<ISet<NonTerminalSymbol>>()); 
                
                foreach (KeyValuePair<NonTerminalSymbol, ISet<NonTerminalSymbol>> nextSymbolSet in nextSymbolSetMap)
                {
                    foreach (NonTerminalSymbol nonTerminalSymbol in nextSymbolSet.Value)
                    {
                        if (targetRuleMap.ContainsKey(nonTerminalSymbol))
                        {
                            Rule rule = targetRuleMap[nonTerminalSymbol];

                            foreach (Chain chain in rule.Chains.Where(c => c.Sequence.Count == 1))
                            {
                                NonTerminalSymbol symbol = chain.Sequence.First().As<NonTerminalSymbol>();
                                ISet<NonTerminalSymbol> newSymbolSet = newSymbolSetMap[nextSymbolSet.Key];
                                if (symbol != null && !newSymbolSet.Contains(symbol) && !nextSymbolSet.Value.Contains(symbol))
                                {
                                    newSymbolSet.Add(symbol);
                                }
                            }
                        }
                    }
                }

                IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> previousSymbolSetMap = nextSymbolSetMap.ToDictionary(kv => kv.Key, kv => kv.Value.ToHashSet().Of<ISet<NonTerminalSymbol>>());
                IDictionary<NonTerminalSymbol, ChainRulesIterationTuple> symbolMap = new Dictionary<NonTerminalSymbol, ChainRulesIterationTuple>();
                                
                foreach (KeyValuePair<NonTerminalSymbol, ISet<NonTerminalSymbol>> symbolSet in previousSymbolSetMap)
                {
                    ISet<NonTerminalSymbol> newSymbolSet = newSymbolSetMap[symbolSet.Key];
                    ISet<NonTerminalSymbol> nonTerminalSet = nextSymbolSetMap[symbolSet.Key];

                    if (newSymbolSet.Count == 0)
                    {
                        unchangeableSymbolSetMap[symbolSet.Key] = new ChainRulesEndTuple(nonTerminalSet, nonTerminalSet.Where(m => m != symbolSet.Key), i);

                        symbolMap.Add(symbolSet.Key, new ChainRulesIterationTuple(true, symbolSet.Value, newSymbolSet, nonTerminalSet));
                        nextSymbolSetMap.Remove(symbolSet.Key);
                    }
                    else
                    {
                        ISet<NonTerminalSymbol> nextSymbolSet = nonTerminalSet;
                        nextSymbolSet.UnionWith(newSymbolSet);
                        symbolMap.Add(symbolSet.Key, new ChainRulesIterationTuple(false, symbolSet.Value, newSymbolSet, nextSymbolSet));
                    }
                }

                if (onIterate != null)
                {
                    onIterate(new ChainRulesIterationPostReport(i,
                        symbolMap,
                        symbolMap.All(kv => kv.Value.IsLastIteration)));
                }

                newSymbolSetMap = symbolMap.Where(kv => !kv.Value.IsLastIteration).ToDictionary(kv => kv.Key, kv => kv.Value.Next.ToHashSet().Of<ISet<NonTerminalSymbol>>());
            } while (newSymbolSetMap.Count > 0);

            ISet<Rule> newRules = new HashSet<Rule>();

            foreach (Rule rule in Rules)
            {
                ISet<Chain> newChains = new HashSet<Chain>();

                foreach (Chain chain in rule.Chains)
                {
                    if (chain.Sequence.Count != 1 || chain.Sequence.FirstOrDefault().As<NonTerminalSymbol>() == null)
                    {
                        newChains.Add(chain);
                    }
                }

                if (newChains.Any())
                {
                    newRules.Add(new Rule(newChains, rule.Target));
                }
            }

            if (onEnd != null)
            {
                onEnd(new ChainRulesEndPostReport(unchangeableSymbolSetMap));
            }

            ISet<Rule> additionalRules = new HashSet<Rule>();
            
            foreach (Rule rule in newRules)
            {
                foreach (KeyValuePair<NonTerminalSymbol, ChainRulesEndTuple> symbolSet in unchangeableSymbolSetMap)
                {
                    if (symbolSet.Value.FinalNonTerminals.Contains(rule.Target))
                    {
                        additionalRules.Add(new Rule(rule.Chains, symbolSet.Key));
                    }
                }
            }
            
            newRules = Normalize(newRules.Concat(additionalRules));
            
            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        public bool RemoveEmptyRules(out Grammar grammar,
            Action<BeginPostReport<NonTerminalSymbol>> onBegin = null,
            Action<IterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newSymbolSet = Rules.Where(r => r.Chains.Any(c => !c.Any())).Select(r => r.Target).ToHashSet();

            if (onBegin != null)
            {
                onBegin(new BeginPostReport<NonTerminalSymbol>(i, newSymbolSet));
            }

            bool isAddedSomething;

            do
            {
                i++;

                isAddedSomething = false;
                ISet<NonTerminalSymbol> nextSymbolSet = newSymbolSet;
                newSymbolSet = new HashSet<NonTerminalSymbol>();

                foreach (Rule rule in Rules)
                {
                    foreach (Chain chain in rule.Chains)
                    {
                        bool isAllSymbolsInNext = true;

                        foreach (Symbol symbol in chain.Sequence)
                        {
                            NonTerminalSymbol nonTerminal = symbol.As<NonTerminalSymbol>();

                            if (nonTerminal == null || !nextSymbolSet.Contains(nonTerminal))
                            {
                                isAllSymbolsInNext = false;
                                break;
                            }
                        }

                        if (isAllSymbolsInNext)
                        {
                            if (!newSymbolSet.Contains(rule.Target) && !nextSymbolSet.Contains(rule.Target))
                            {
                                newSymbolSet.Add(rule.Target);
                                isAddedSomething = true;
                            }
                        }
                    }
                }

                ISet<NonTerminalSymbol> previousSymbolSet = nextSymbolSet.ToHashSet();
                nextSymbolSet.UnionWith(newSymbolSet);

                if (onIterate != null)
                {
                    onIterate(new IterationPostReport<NonTerminalSymbol>(i,
                        previousSymbolSet, newSymbolSet, nextSymbolSet, !isAddedSomething));
                }

                newSymbolSet = nextSymbolSet;

            } while (isAddedSomething);

            ISet<Rule> newRules = new HashSet<Rule>();

            foreach (Rule rule in Rules)
            {
                newRules.Add(new Rule(rule.Chains.SelectMany(c => ForkByEmpty(c, newSymbolSet).Where(chain => chain != Chain.Empty)), rule.Target));
            }

            IDictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            NonTerminalSymbol newTarget = Target;

            if (targetRuleMap.ContainsKey(Target) && targetRuleMap[Target].Chains.Contains(Chain.Empty))
            {
                Chain targetChain = new Chain(EnumerateHelper.Sequence(Target));

                newTarget = GetNewNonTerminal(NonTerminals);

                Rule rule = new Rule(EnumerateHelper.Sequence(targetChain, Chain.Empty), newTarget);

                newRules.Add(rule);
            }

            newRules = Normalize(newRules);

            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, newTarget);

                return true;
            }

            grammar = this;

            return false;
        }

        internal static NonTerminalSymbol GetNewNonTerminal(IEnumerable<NonTerminalSymbol> nonTerminals)
        {
            return
                new NonTerminalSymbol(
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

        private static IEnumerable<Chain> ForkByEmpty(Chain chain, ISet<NonTerminalSymbol> symbolSet)
        {
            ISet<Symbol> symbolSetInternal = symbolSet.OfType<Symbol>().ToHashSet();

            IList<Symbol> symbols = new List<Symbol>(chain.Sequence.Count);
            Stack<bool> stack = new Stack<bool>();

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

                    i++;
                }

                yield return new Chain(symbols);

                do
                {
                    while (i > 0 && !symbolSetInternal.Contains(chain.Sequence[i - 1]))
                    {
                        symbols.RemoveAt(symbols.Count - 1);
                        i--;
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
                            i--;
                        }
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            } while (stack.Count > 0);
        }

        public bool RemoveUnreachableSymbols(out Grammar grammar,
            Action<BeginPostReport<Symbol>> onBegin = null,
            Action<IterationPostReport<Symbol>> onIterate = null)
        {
            int i = 0;

            ISet<Symbol> newSymbolSet = new HashSet<Symbol>();
            newSymbolSet.Add(Target);

            if (onBegin != null)
            {
                onBegin(new BeginPostReport<Symbol>(i, newSymbolSet));
            }

            IDictionary<NonTerminalSymbol,Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            bool isAddedSomething;

            do
            {
                i++;

                isAddedSomething = false;

                ISet<Symbol> nextSymbolSet = newSymbolSet;
                newSymbolSet = new HashSet<Symbol>();

                foreach (Symbol symbol in nextSymbolSet)
                {
                    NonTerminalSymbol nonTerminalSymbol = symbol.As<NonTerminalSymbol>();

                    if (nonTerminalSymbol != null && targetRuleMap.ContainsKey(nonTerminalSymbol))
                    {
                        foreach (Chain chain in targetRuleMap[nonTerminalSymbol].Chains)
                        {
                            foreach (Symbol chainSymbol in chain.Sequence)
                            {
                                if (!nextSymbolSet.Contains(chainSymbol) && !newSymbolSet.Contains(chainSymbol))
                                {
                                    isAddedSomething = true;
                                    newSymbolSet.Add(chainSymbol);
                                }
                            }
                        }                        
                    }
                }

                ISet<Symbol> previousSymbolSet = nextSymbolSet.ToHashSet();
                nextSymbolSet.UnionWith(newSymbolSet);

                if (onIterate != null)
                {
                    onIterate(new IterationPostReport<Symbol>(i,
                        previousSymbolSet, newSymbolSet, nextSymbolSet, !isAddedSomething));
                }

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

        public bool RemoveUselessSymbols(out Grammar grammar, 
            Action<BeginPostReport<NonTerminalSymbol>> onBegin = null,
            Action<IterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newNonTerminalSet = new HashSet<NonTerminalSymbol>();
            
            if (onBegin != null)
            {
                onBegin(new BeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));
            }

            bool isAddedSomething;

            do
            {
                i++;
                isAddedSomething = false;
                ISet<NonTerminalSymbol> nextNonTerminalSet = newNonTerminalSet;
                newNonTerminalSet = new HashSet<NonTerminalSymbol>();

                foreach (Rule rule in Rules)
                {
                    foreach (Chain chain in rule.Chains)
                    {
                        if (chain.All(s => TerminalOrSetContains(s, nextNonTerminalSet)))
                        {
                            if (!newNonTerminalSet.Contains(rule.Target) && !nextNonTerminalSet.Contains(rule.Target))
                            {
                                newNonTerminalSet.Add(rule.Target);
                                isAddedSomething = true;
                            }
                            
                            break;
                        }
                    }
                }

                ISet<NonTerminalSymbol> previousNonTerminalSet = nextNonTerminalSet.ToHashSet();
                nextNonTerminalSet.UnionWith(newNonTerminalSet);

                if (onIterate != null)
                {
                    onIterate(new IterationPostReport<NonTerminalSymbol>(i,
                        previousNonTerminalSet, newNonTerminalSet, nextNonTerminalSet, !isAddedSomething));
                }

                newNonTerminalSet = nextNonTerminalSet;

            } while (isAddedSomething);

            ISet<Rule> newRules = Normalize(GetRulesWithoutUselessSymbols(Rules, newNonTerminalSet));

            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        private IEnumerable<Rule> GetRulesWithoutUselessSymbols(IEnumerable<Rule> rules, ISet<NonTerminalSymbol> nonTerminalSet)
        {
            foreach (Rule rule in rules)
            {
                if (!nonTerminalSet.Contains(rule.Target))
                {
                    continue;
                }

                ISet<Chain> newChains = new HashSet<Chain>();

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

        private static bool TerminalOrSetContains(Symbol symbol, ISet<NonTerminalSymbol> nonTerminalSet)
        {
            NonTerminalSymbol nonTerminalSymbol = symbol.As<NonTerminalSymbol>();
            return nonTerminalSymbol == null || nonTerminalSet.Contains(nonTerminalSymbol);
        }

        public bool IsLangEmpty(Action<BeginPostReport<NonTerminalSymbol>> onBegin = null,
                            Action<IterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newNonTerminalSet = new HashSet<NonTerminalSymbol>();

            if (onBegin != null)
            {
                onBegin(new BeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));
            }
            
            bool isAddedSomething;
            
            do 
            {
                i++;

                isAddedSomething = false;

                ISet<NonTerminalSymbol> nextNonTerminalSet = newNonTerminalSet;
                newNonTerminalSet = new HashSet<NonTerminalSymbol>();
                
                foreach (Rule rule in Rules)
                {	
                    foreach (Chain chain in rule.Chains)
                    {
                        bool isOurChain = true;

                        foreach (Symbol symbol in chain)
                        {
                            NonTerminalSymbol nonTerminalSymbol = symbol.As<NonTerminalSymbol>();

                            if (nonTerminalSymbol != null && !nextNonTerminalSet.Contains(nonTerminalSymbol))
                            {
                                isOurChain = false;
                                break;
                            }                            
                        }
                        
                        if (isOurChain)
                        {
                            if (!newNonTerminalSet.Contains(rule.Target) && !nextNonTerminalSet.Contains(rule.Target))
                            {
                                newNonTerminalSet.Add(rule.Target);
                                isAddedSomething = true;
                            }
                            break;
                        }
                    }
                }

                ISet<NonTerminalSymbol> previousNonTerminalSet = nextNonTerminalSet.ToHashSet();
                nextNonTerminalSet.UnionWith(newNonTerminalSet);

                if (onIterate != null)
                {
                    onIterate(new IterationPostReport<NonTerminalSymbol>(i, 
                        previousNonTerminalSet, newNonTerminalSet, nextNonTerminalSet, !isAddedSomething));
                }

                newNonTerminalSet = nextNonTerminalSet;                

            } while (isAddedSomething);			
            
            return !newNonTerminalSet.Contains(Target);
        }

        public Grammar Reorganize(IDictionary<NonTerminalSymbol,NonTerminalSymbol> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            return new Grammar(Rules.Select(rule => rule.Reorganize(map)), map[Target]);
        }

        public Grammar Reorganize(int firstIndex)
        {
            int index = firstIndex;

            IDictionary<NonTerminalSymbol,NonTerminalSymbol> map = new Dictionary<NonTerminalSymbol,NonTerminalSymbol>();

            foreach (NonTerminalSymbol symbol in NonTerminals)
            {
                map.Add(symbol,new NonTerminalSymbol(new Label(new SingleLabel(_DefaultNonTerminalSymbol,index++))));
            }

            return Reorganize(map);
        }

        internal void SplitRules(out IReadOnlySet<Rule> terminalSymbolsOnlyRules, out IReadOnlySet<Rule> otherRules)
        {
            ISet<Rule> terminalSymbolsOnlyRulesInternal = new SortedSet<Rule>();
            ISet<Rule> otherRulesInternal = new SortedSet<Rule>();

            foreach (Rule rule in Rules)
            {
                ISet<Chain> terminalSymbolsOnlyChains = new SortedSet<Chain>();
                ISet<Chain> otherChains = new SortedSet<Chain>();

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
                    terminalSymbolsOnlyRulesInternal.Add(new Rule(terminalSymbolsOnlyChains, rule.Target));
                }

                if (otherChains.Count > 0)
                {
                    otherRulesInternal.Add(new Rule(otherChains, rule.Target));
                }
            }

            terminalSymbolsOnlyRules = terminalSymbolsOnlyRulesInternal.AsReadOnly();
            otherRules = otherRulesInternal.AsReadOnly();
        }

        public static bool operator ==(Grammar objA, Grammar objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Grammar objA, Grammar objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Grammar objA, Grammar objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Grammar objA, Grammar objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Grammar objA, Grammar objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Grammar objA, Grammar objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Grammar objA, Grammar objB)
        {
            if ((object)objA == null)
            {
                if ((object)objB == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return objA.Equals(objB);
        }

        public static int Compare(Grammar objA, Grammar objB)
        {
            if (objA == null)
            {
                if (objB == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return objA.CompareTo(objB);
        }

        public override bool Equals(object obj)
        {
            Grammar grammar = obj as Grammar;
            return Equals(grammar);
        }

        public override int GetHashCode()
        {
            return Target.GetHashCode() ^ Rules.GetSequenceHashCode();
        }

        public bool Equals(Grammar other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Target.Equals(other.Target) &&
                Rules.SequenceEqual(other.Rules);
        }

        public int CompareTo(Grammar other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Target.CompareTo(other.Target);

            if (result != 0)
            {
                return result;
            }

            return Rules.SequenceCompare(other.Rules);
        }

        public override string ToString()
        {
            return string.Format("N = {0}, Σ = {1}, S = {2}, P = {3}", 
                string.Concat("{",string.Join<NonTerminalSymbol>(",",NonTerminals),"}"),
                string.Concat("{",string.Join<TerminalSymbol>(",",Alphabet),"}"),
                Target,
                string.Concat("{",string.Join<Rule>(",",Rules),"}")
            );
        }
    }
}
