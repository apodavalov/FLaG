﻿using FLaGLib.Collections;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Grammars
{
    public class Grammar : IComparable<Grammar>, IEquatable<Grammar>
    {
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
            return rules.GroupBy(r => r.Target).Select(g => new Rule(g.SelectMany(r => r.Chains), g.Key)).ToHashSet();
        }

        public bool RemoveChainRules(out Grammar grammar)
        {
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> symbolSetMap =
                new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>();

            IDictionary<NonTerminalSymbol, bool> symbolChangedMap =
                new Dictionary<NonTerminalSymbol, bool>();

            foreach (NonTerminalSymbol nonTerminalSymbol in NonTerminals)
            {
                symbolSetMap[nonTerminalSymbol] = new HashSet<NonTerminalSymbol>(nonTerminalSymbol.AsSequence());
            }

            IDictionary<NonTerminalSymbol, ChainRulesTuple> unchangeableSymbolSetMap = new Dictionary<NonTerminalSymbol, ChainRulesTuple>();
            IDictionary<NonTerminalSymbol, Rule> targetRuleMap = Rules.ToDictionary(r => r.Target);

            // IReadOnlyDictionary<NonTerminalSymbol,IReadOnlySet<NonTerminalSet>> - symbolSetMap

            int i = 0;

            do
            {
                i++;
                
                foreach (KeyValuePair<NonTerminalSymbol, ISet<NonTerminalSymbol>> symbolSet in symbolSetMap)
                {
                    symbolChangedMap[symbolSet.Key] = false;

                    foreach (NonTerminalSymbol nonTerminalSymbol in symbolSet.Value)
                    {
                        if (targetRuleMap.ContainsKey(nonTerminalSymbol))
                        {
                            Rule rule = targetRuleMap[nonTerminalSymbol];

                            foreach (Chain chain in rule.Chains.Where(c => c.Sequence.Count == 1))
                            {
                                NonTerminalSymbol symbol = chain.Sequence.First().As<NonTerminalSymbol>();
                                if (symbol != null && symbolSet.Value.Add(symbol))
                                {
                                    symbolChangedMap[symbolSet.Key] = true;                                        
                                }
                            }
                        }
                    }
                }

                // Save old symbolSetMap

                foreach (KeyValuePair<NonTerminalSymbol, bool> symbolChanged in symbolChangedMap)
                {
                    if (!symbolChanged.Value)
                    {
                        unchangeableSymbolSetMap[symbolChanged.Key] = new ChainRulesTuple(symbolSetMap[symbolChanged.Key], i);
                        symbolSetMap.Remove(symbolChanged.Key);
                    }
                }

                // oldSymbolSetMap, nextSymbolSetMap, leaveSymbolSetMap (unchangeableSymbolSetMap)

                symbolChangedMap.Clear();                
            } while (symbolSetMap.Any());

            IDictionary<NonTerminalSymbol, ChainRulesTuple> unchangeableSymbolSetMapWithoutKey = unchangeableSymbolSetMap
                .Select(kv => 
                    new KeyValuePair<NonTerminalSymbol, ChainRulesTuple>(
                        kv.Key, 
                        new ChainRulesTuple(kv.Value.NonTerminals.Where(s => s != kv.Key), kv.Value.Iteration)
                    )
                ).ToDictionary();

            // unchangeableSymbolSetMap, unchangeableSymbolSetMapWithoutKey

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

            ISet<Rule> additionalRules = new HashSet<Rule>();            

            do
            {
                ISet<Rule> temp = additionalRules;
                additionalRules = newRules;
                newRules = temp;                

                foreach (Rule rule in additionalRules)
                {
                    foreach (KeyValuePair<NonTerminalSymbol, ChainRulesTuple> symbolSet in unchangeableSymbolSetMapWithoutKey)
                    {
                        if (symbolSet.Value.NonTerminals.Contains(rule.Target))
                        {
                            newRules.Add(new Rule(rule.Chains, symbolSet.Key));                            
                        }
                    }
                }

                newRules.UnionWith(additionalRules);
                newRules = Normalize(newRules);

            } while (!newRules.SetEquals(additionalRules));
            
            if (!newRules.SetEquals(Rules))
            {
                grammar = new Grammar(newRules, Target);

                return true;
            }

            grammar = this;

            return false;
        }

        public bool RemoveEmptyRules(out Grammar grammar,
            Action<GrammarBeginPostReport<NonTerminalSymbol>> onBegin = null,
            Action<GrammarIterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newSymbolSet = Rules.Where(r => r.Chains.Any(c => !c.Any())).Select(r => r.Target).ToHashSet();

            if (onBegin != null)
            {
                onBegin(new GrammarBeginPostReport<NonTerminalSymbol>(i, newSymbolSet));
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
                    onIterate(new GrammarIterationPostReport<NonTerminalSymbol>(i,
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

                newTarget = new NonTerminalSymbol(
                        new Label(
                            new SingleLabel('S',
                                NonTerminals
                                    .Where(s => s.Label.LabelType == LabelType.Simple)
                                    .Max(s => s.Label.ExtractSingleLabel().SignIndex ?? 0) + 1
                            )
                        )
                    );

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

        public bool RemoveUnreachedSymbols(out Grammar grammar,
            Action<GrammarBeginPostReport<Symbol>> onBegin = null,
            Action<GrammarIterationPostReport<Symbol>> onIterate = null)
        {
            int i = 0;

            ISet<Symbol> newSymbolSet = new HashSet<Symbol>();
            newSymbolSet.Add(Target);

            if (onBegin != null)
            {
                onBegin(new GrammarBeginPostReport<Symbol>(i, newSymbolSet));
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
                    onIterate(new GrammarIterationPostReport<Symbol>(i,
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
            Action<GrammarBeginPostReport<NonTerminalSymbol>> onBegin = null,
            Action<GrammarIterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newNonTerminalSet = new HashSet<NonTerminalSymbol>();
            
            if (onBegin != null)
            {
                onBegin(new GrammarBeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));
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
                    onIterate(new GrammarIterationPostReport<NonTerminalSymbol>(i,
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

        public bool IsLangEmpty(Action<GrammarBeginPostReport<NonTerminalSymbol>> onBegin = null,
                            Action<GrammarIterationPostReport<NonTerminalSymbol>> onIterate = null)
        {
            int i = 0;

            ISet<NonTerminalSymbol> newNonTerminalSet = new HashSet<NonTerminalSymbol>();

            if (onBegin != null)
            {
                onBegin(new GrammarBeginPostReport<NonTerminalSymbol>(i, newNonTerminalSet));
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
                    onIterate(new GrammarIterationPostReport<NonTerminalSymbol>(i, 
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
                map.Add(symbol,new NonTerminalSymbol(new Label(new SingleLabel('S',index++))));
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
