using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesEndPostReport
    {
        public IReadOnlyDictionary<NonTerminalSymbol, ChainRulesTuple> SymbolMap
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<NonTerminalSymbol, ChainRulesTuple> SymbolMapFinal
        {
            get;
            private set;
        }

        public ChainRulesEndPostReport(IDictionary<NonTerminalSymbol, ChainRulesTuple> symbolMap,
            IDictionary<NonTerminalSymbol, ChainRulesTuple> symbolMapFinal)
        {
            if (symbolMap == null)
            {
                throw new ArgumentNullException(nameof(symbolMap));
            }

            if (symbolMapFinal == null)
            {
                throw new ArgumentNullException(nameof(symbolMapFinal));
            }

            SymbolMap = symbolMap.ToSortedDictionary().AsReadOnly();
            SymbolMapFinal = symbolMapFinal.ToSortedDictionary().AsReadOnly();
        }
    }
}