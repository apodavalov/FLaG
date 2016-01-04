using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesTuple
    {
        public IReadOnlySet<NonTerminalSymbol> NonTerminals
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> FinalNonTerminals
        {
            get;
            private set;
        }

        public int Iteration
        {
            get;
            private set;
        }

        public ChainRulesTuple(IEnumerable<NonTerminalSymbol> nonTerminalSymbols, IEnumerable<NonTerminalSymbol> finalNonTerminalSymbols, int iteration)
        {
            if (nonTerminalSymbols == null)
            {
                throw new ArgumentNullException(nameof(nonTerminalSymbols));
            }

            if (finalNonTerminalSymbols == null)
            {
                throw new ArgumentNullException(nameof(finalNonTerminalSymbols));
            }

            NonTerminals = nonTerminalSymbols.ToSortedSet().AsReadOnly();
            FinalNonTerminals = finalNonTerminalSymbols.ToSortedSet().AsReadOnly();
            Iteration = iteration;
        }
    }
}
