using System;

namespace FLaG.Data.Automaton
{
	class DTransitionFunc : IComparable<DTransitionFunc>
	{
		public DStatus OldStatus
		{
			get;
			set;
		}
		
		public Symbol Symbol
		{
			get;
			set;
		}
		
		public DStatus NewStatus
		{
			get;
			set;
		}
		
		public int CompareTo(DTransitionFunc other)
		{
			int res = OldStatus.CompareTo(other.OldStatus);
			
			if (res != 0)
				return res;
			
			return Symbol.CompareTo(other.Symbol);
		}
	}
}

