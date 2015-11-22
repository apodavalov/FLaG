using System;

namespace FLaGLib.Data.Grammars
{
    internal class SymbolTuple
    {
        public NonTerminalSymbol Target
        {
            get;
            private set;
        }

        public NonTerminalSymbol NonTerminalSymbol
        {
            get;
            private set;
        }

        public TerminalSymbol TerminalSymbol
        {
            get;
            private set;
        }

        public SymbolTuple(NonTerminalSymbol target, TerminalSymbol terminalSymbol, NonTerminalSymbol nonTerminalSymbol)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Target = target;
            NonTerminalSymbol = nonTerminalSymbol;
            TerminalSymbol = terminalSymbol;
        }
    }
}
