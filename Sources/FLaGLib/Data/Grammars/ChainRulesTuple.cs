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

        public int Iteration
        {
            get;
            private set;
        }

        public ChainRulesTuple(IEnumerable<NonTerminalSymbol> nonTerminalSymbols, int iteration)
        {
            if (nonTerminalSymbols == null)
            {
                throw new ArgumentNullException(nameof(nonTerminalSymbols));
            }

            NonTerminals = nonTerminalSymbols.ToSortedSet().AsReadOnly();
            Iteration = iteration;
        }
    }
}
