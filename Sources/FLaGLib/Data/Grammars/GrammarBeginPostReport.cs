﻿using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class GrammarBeginPostReport<T> where T : Symbol
    {
        private const string _AtLeastOneSymbolIsNullMessage = "At least one symbol is null.";

        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlySet<T> NonTerminalSet
        {
            get;
            private set;
        }

        public GrammarBeginPostReport(int iteration,
            IEnumerable<T> symbolSet)
        {
            if (symbolSet == null)
            {
                throw new ArgumentNullException(nameof(symbolSet));
            }
            
            NonTerminalSet = symbolSet.ToHashSet().AsReadOnly();

            if (NonTerminalSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneSymbolIsNullMessage, nameof(symbolSet));
            }

            Iteration = iteration;
        }
    }
}