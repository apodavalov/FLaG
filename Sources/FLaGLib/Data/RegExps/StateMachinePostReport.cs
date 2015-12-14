using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.RegExps
{
    public class StateMachinePostReport
    {
        public IReadOnlyList<StateMachineExpressionWithOriginal> Dependencies
        {
            get;
            private set;
        }

        public StateMachineExpressionTuple New
        {
            get;
            private set;
        }

        public StateMachinePostReport(StateMachineExpressionTuple @new, IEnumerable<StateMachineExpressionWithOriginal> dependencies)
        {
            if (@new == null)
            {
                throw new ArgumentNullException(nameof(@new));
            }

            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            New = @new;
            Dependencies = dependencies.ToList().AsReadOnly();

            if (Dependencies.AnyNull())
            {
                throw new ArgumentException("At least one element is null.", nameof(dependencies));
            }
        }
    }
}
