using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesEndPostReport
    {
        public IReadOnlyDictionary<NonTerminalSymbol, ChainRulesEndTuple> SymbolMap
        {
            get;
            private set;
        }

        public ChainRulesEndPostReport(IDictionary<NonTerminalSymbol, ChainRulesEndTuple> symbolMap)
        {
            if (symbolMap == null)
            {
                throw new ArgumentNullException(nameof(symbolMap));
            }

            SymbolMap = symbolMap.ToSortedDictionary().AsReadOnly();
        }
    }
}