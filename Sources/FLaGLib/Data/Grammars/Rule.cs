using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.Grammars
{
    public class Rule : IComparable<Rule>, IEquatable<Rule>
    {
        public IReadOnlySet<Chain> Chains
        {
            get;
            private set;
        }

        public NonTerminalSymbol Target
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

        public Rule(IEnumerable<Chain> chains, NonTerminalSymbol target)
        {
            if (chains == null)
            {
                throw new ArgumentNullException("chains");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Chains = chains.ToSortedSet().AsReadOnly();

            if (Chains.Count < 1)
            {
                throw new ArgumentException("Rule must contain at least one chain.");
            }

            Alphabet = Chains.SelectMany(chain => chain.Alphabet).ToSortedSet().AsReadOnly();
            Target = target;
            ISet<NonTerminalSymbol> nonTerminals = Chains.SelectMany(chain => chain.NonTerminals).ToSortedSet();
            nonTerminals.Add(Target);
            NonTerminals = nonTerminals.AsReadOnly();
        }

        public Rule Reorganize(IDictionary<NonTerminalSymbol,NonTerminalSymbol> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            return new Rule(Chains.Select(chain => chain.Reorganize(map)),map.ValueOrDefault(Target,Target));
        }

        public static bool operator ==(Rule objA, Rule objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Rule objA, Rule objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Rule objA, Rule objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Rule objA, Rule objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Rule objA, Rule objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Rule objA, Rule objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Rule objA, Rule objB)
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

        public static int Compare(Rule objA, Rule objB)
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
            Rule rule = obj as Rule;
            return Equals(rule);
        }

        public override int GetHashCode()
        {
            return Target.GetHashCode() ^ Chains.GetSequenceHashCode();
        }

        public bool Equals(Rule other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Target.Equals(other.Target) &&
                Chains.SequenceEqual(other.Chains);
        }

        public int CompareTo(Rule other)
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

            return Chains.SequenceCompare(other.Chains);
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}",
                Target,
                string.Join<Chain>("|", Chains)
            );
        }
    }
}
