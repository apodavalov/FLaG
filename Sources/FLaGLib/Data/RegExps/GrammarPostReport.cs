using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.RegExps
{
    public class GrammarPostReport
    {
        public IReadOnlyList<GrammarExpressionWithOriginal> Dependencies
        {
            get;
            private set;
        }
        
        public GrammarExpressionTuple New
        {
            get;
            private set;
        }

        public GrammarPostReport(GrammarExpressionTuple @new, IEnumerable<GrammarExpressionWithOriginal> dependencies)
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
