using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class GrammarBeginPostReport
    {
        private const string _AtLeastOneNonTerminalSymbolIsNullMessage = "At least one non terminal symbol is null.";

        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> NonTerminalSet
        {
            get;
            private set;
        }

        public GrammarBeginPostReport(int iteration,
            IEnumerable<NonTerminalSymbol> nonTerminalSet)
        {
            if (nonTerminalSet == null)
            {
                throw new ArgumentNullException(nameof(nonTerminalSet));
            }
            
            NonTerminalSet = nonTerminalSet.ToHashSet().AsReadOnly();

            if (NonTerminalSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneNonTerminalSymbolIsNullMessage, nameof(nonTerminalSet));
            }

            Iteration = iteration;
        }
    }
}
