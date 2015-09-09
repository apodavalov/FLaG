using FLaGLib.Collections;
using FLaGLib.Extensions;
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
                throw new ArgumentNullException("rules");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Rules = rules.ToHashSet().AsReadOnly();
            Alphabet = Rules.SelectMany(rule => rule.Alphabet).ToSortedSet().AsReadOnly();
            Target = target;
            ISet<NonTerminalSymbol> nonTerminals = Rules.SelectMany(rule => rule.NonTerminals).ToSortedSet();
            nonTerminals.Add(Target);
            NonTerminals = nonTerminals.AsReadOnly();
        }

        public Grammar Reorganize(IDictionary<NonTerminalSymbol,NonTerminalSymbol> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
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
