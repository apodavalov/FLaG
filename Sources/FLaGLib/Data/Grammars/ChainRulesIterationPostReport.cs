using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesIterationPostReport
    {
        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol,ChainRulesIterationTuple> SymbolMap
        {
            get;
            private set;
        }

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public ChainRulesIterationPostReport(
            int iteration,
            IDictionary<NonTerminalSymbol, ChainRulesIterationTuple> symbolMap,
            bool isLastIteration)
        {
            if (symbolMap == null)
            {
                throw new ArgumentNullException(nameof(symbolMap));
            }

            SymbolMap = symbolMap.ToSortedDictionary().AsReadOnly();

            Iteration = iteration;
            IsLastIteration = isLastIteration;
        }
    }
}
