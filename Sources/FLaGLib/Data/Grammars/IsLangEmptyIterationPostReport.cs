using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class IsLangEmptyIterationPostReport
    {
        private const string _AtLeastOneNonTerminalSymbolIsNullMessage = "At least one non terminal symbol is null.";

        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> PreviousNonTerminalSet
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> NewNonTerminalSet
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> NextNonTerminalSet
        {
            get;
            private set;
        }

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public IsLangEmptyIterationPostReport(int iteration, 
            IEnumerable<NonTerminalSymbol> previousNonTerminalSet,
            IEnumerable<NonTerminalSymbol> newNonTerminalSet,
            IEnumerable<NonTerminalSymbol> nextNonTerminalSet,
            bool isLastIteration)
        {
            if (previousNonTerminalSet == null)
            {
                throw new ArgumentNullException(nameof(previousNonTerminalSet));
            }

            if (newNonTerminalSet == null)
            {
                throw new ArgumentNullException(nameof(newNonTerminalSet));
            }

            if (nextNonTerminalSet == null)
            {
                throw new ArgumentNullException(nameof(nextNonTerminalSet));
            }

            PreviousNonTerminalSet = previousNonTerminalSet.ToHashSet().AsReadOnly();
            NewNonTerminalSet = newNonTerminalSet.ToHashSet().AsReadOnly();
            NextNonTerminalSet = nextNonTerminalSet.ToHashSet().AsReadOnly();

            if (PreviousNonTerminalSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneNonTerminalSymbolIsNullMessage, nameof(previousNonTerminalSet));
            }

            if (NewNonTerminalSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneNonTerminalSymbolIsNullMessage, nameof(newNonTerminalSet));
            }

            if (NextNonTerminalSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneNonTerminalSymbolIsNullMessage, nameof(nextNonTerminalSet));
            }

            Iteration = iteration;
            IsLastIteration = isLastIteration;
        }
    }
}
