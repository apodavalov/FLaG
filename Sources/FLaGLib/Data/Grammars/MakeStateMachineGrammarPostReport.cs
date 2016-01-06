using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class MakeStateMachineGrammarPostReport
    {
        public NonTerminalSymbol Target
        {
            get;
            private set;
        }

        public Chain Chain
        {
            get;
            private set;
        }

        public IReadOnlySet<Rule> PreviousRules
        {
            get;
            private set;
        }

        public IReadOnlySet<Rule> NewRules
        {
            get;
            private set;
        }

        public IReadOnlySet<Rule> NextRules
        {
            get;
            private set;
        }

        public bool Converted
        {
            get;
            private set;
        }

        public MakeStateMachineGrammarPostReport(NonTerminalSymbol target, Chain chain, IEnumerable<Rule> previousRules, IEnumerable<Rule> newRules, IEnumerable<Rule> nextRules, bool converted)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (chain == null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            if (previousRules == null)
            {
                throw new ArgumentNullException(nameof(previousRules));
            }

            if (newRules == null)
            {
                throw new ArgumentNullException(nameof(newRules));
            }

            if (nextRules == null)
            {
                throw new ArgumentNullException(nameof(nextRules));
            }

            Target = target;
            Chain = chain;
            PreviousRules = previousRules.ToSortedSet().AsReadOnly();
            NextRules = nextRules.ToSortedSet().AsReadOnly();
            NewRules = newRules.ToSortedSet().AsReadOnly();
            Converted = converted;
        }
    }
}
