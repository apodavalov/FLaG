using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesIterationPostReport
    {
        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol,IReadOnlySet<NonTerminalSymbol>> PreviousMap
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> NewMap
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> ConsideredNextMap
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> NotConsideredNextMap
        {
            get;
            private set;
        }

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public ChainRulesIterationPostReport(int iteration,
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> previousSymbolSet,
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> newSymbolSet,
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> consideredNextSymbolSet,
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> notConsideredNextSymbolSet,
            bool isLastIteration)
        {
            if (previousSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(previousSymbolSet));
            }

            if (newSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(newSymbolSet));
            }

            if (consideredNextSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(consideredNextSymbolSet));
            }

            if (notConsideredNextSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(notConsideredNextSymbolSet));
            }

            PreviousMap = previousSymbolSet.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedSet().AsReadOnly()).ToSortedDictionary().AsReadOnly();
            NewMap = newSymbolSet.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedSet().AsReadOnly()).ToSortedDictionary().AsReadOnly();
            ConsideredNextMap = consideredNextSymbolSet.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedSet().AsReadOnly()).ToSortedDictionary().AsReadOnly();
            NotConsideredNextMap = notConsideredNextSymbolSet.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedSet().AsReadOnly()).ToSortedDictionary().AsReadOnly();

            Iteration = iteration;
            IsLastIteration = isLastIteration;
        }
    }
}
