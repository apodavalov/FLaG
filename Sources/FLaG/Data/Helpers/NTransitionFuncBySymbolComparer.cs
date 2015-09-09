using FLaG.Data.Automaton;
using System.Collections.Generic;

namespace FLaG.Data.Helpers
{
    class NTransitionFuncBySymbolComparer : IComparer<NTransitionFunc>
	{
		public int Compare (NTransitionFunc x, NTransitionFunc y)
		{
			int res = x.Symbol.CompareTo(y.Symbol);
			
			if (res != 0)
				return res;
			
			if (x.NewStatus == null && y.NewStatus != null)
				return -1;
			
			if (x.NewStatus != null && y.NewStatus == null)
				return 1;
			
			return 0;
		}
	}
}

