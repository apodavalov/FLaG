using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;

namespace FLaGLib.Data.Grammars
{
    public class IterationPostReport<T> where T : Symbol
    {
        private const string _AtLeastOneSymbolIsNullMessage = "At least one symbol is null.";

        public int Iteration
        {
            get;
            private set;
        }

        public IReadOnlySet<T> PreviousSymbolSet
        {
            get;
            private set;
        }

        public IReadOnlySet<T> NewSymbolSet
        {
            get;
            private set;
        }

        public IReadOnlySet<T> NextSymbolSet
        {
            get;
            private set;
        }

        public bool IsLastIteration
        {
            get;
            private set;
        }

        public IterationPostReport(int iteration, 
            IEnumerable<T> previousSymbolSet,
            IEnumerable<T> newSymbolSet,
            IEnumerable<T> nextSymbolSet,
            bool isLastIteration)
        {
            if (previousSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(previousSymbolSet));
            }

            if (newSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(newSymbolSet));
            }

            if (nextSymbolSet == null)
            {
                throw new ArgumentNullException(nameof(nextSymbolSet));
            }

            PreviousSymbolSet = previousSymbolSet.ToSortedSet().AsReadOnly();
            NewSymbolSet = newSymbolSet.ToSortedSet().AsReadOnly();
            NextSymbolSet = nextSymbolSet.ToSortedSet().AsReadOnly();

            if (PreviousSymbolSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneSymbolIsNullMessage, nameof(previousSymbolSet));
            }

            if (NewSymbolSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneSymbolIsNullMessage, nameof(newSymbolSet));
            }

            if (NextSymbolSet.AnyNull())
            {
                throw new ArgumentException(_AtLeastOneSymbolIsNullMessage, nameof(nextSymbolSet));
            }

            Iteration = iteration;
            IsLastIteration = isLastIteration;
        }
    }
}
