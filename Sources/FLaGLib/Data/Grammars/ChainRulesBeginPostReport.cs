using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesBeginPostReport
    {
        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> SymbolMap
        {
            get;
            private set;
        }

        public ChainRulesBeginPostReport(int iteration,
            IDictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>> symbolMap)
        {
            if (symbolMap == null)
            {
                throw new ArgumentNullException(nameof(symbolMap));
            }

            SymbolMap = symbolMap.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedSet().AsReadOnly()).ToSortedDictionary().AsReadOnly();

            Iteration = iteration;
        }
    }
}
