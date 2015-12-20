using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.RegExps
{
    public class LanguagePostReport
    {
        public IReadOnlyList<LanguageExpressionTuple> Dependencies
        {
            get;
            private set;
        }

        public LanguageExpressionTuple New
        {
            get;
            private set;
        }

        public LanguagePostReport(LanguageExpressionTuple @new, IEnumerable<LanguageExpressionTuple> dependencies)
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
