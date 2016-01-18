using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class ChainRulesIterationTuple
    {
        public IReadOnlySet<NonTerminalSymbol> Previous
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> New
        {
            get;
            private set;
        }

        public IReadOnlySet<NonTerminalSymbol> Next
        {
            get;
            private set;
        }

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public ChainRulesIterationTuple(bool isLastIteration,
            IEnumerable<NonTerminalSymbol> previous,
            IEnumerable<NonTerminalSymbol> @new,
            IEnumerable<NonTerminalSymbol> next)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            if (@new == null)
            {
                throw new ArgumentNullException(nameof(@new));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            Previous = previous.ToSortedSet().AsReadOnly();
            New = @new.ToSortedSet().AsReadOnly();
            Next = next.ToSortedSet().AsReadOnly();

            IsLastIteration = isLastIteration;
        }
    }
}
