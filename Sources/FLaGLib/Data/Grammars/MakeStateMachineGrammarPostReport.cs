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

        public IReadOnlySet<Rule> NewRules
        {
            get;
            private set;
        }

        public bool Converted
        {
            get;
            private set;
        }

        public MakeStateMachineGrammarPostReport(NonTerminalSymbol target, Chain chain, ISet<Rule> newRules, bool converted)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (chain == null)
            {
                throw new ArgumentNullException(nameof(chain));
            }

            if (newRules == null)
            {
                throw new ArgumentNullException(nameof(newRules));
            }

            Target = target;
            Chain = chain;
            NewRules = newRules.AsReadOnly();
            Converted = converted;
        }
    }
}
