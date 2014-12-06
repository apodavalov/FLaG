using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Data.StateMachines
{
    public class SetsOfEquivalenceResult
    {
        public bool IsStatesCombined
        {
            get;
            private set;
        }

        public int LastIteration
        {
            get;
            private set;
        }

        public SetsOfEquivalenceResult(bool isStatesCombined, int lastIteration)
        {
            IsStatesCombined = isStatesCombined;
            LastIteration = lastIteration;
        }
    }
}
