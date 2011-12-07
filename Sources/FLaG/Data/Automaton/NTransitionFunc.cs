using System;

namespace FLaG.Data.Automaton
{
	class NTransitionFunc : IComparable<NTransitionFunc>
	{
		public NStatus OldStatus
		{
			get;
			set;
		}
		
		public Symbol Symbol
		{
			get;
			set;
		}
		
		public NStatus NewStatus
		{
			get;
			set;
		}
		
		public int CompareTo(NTransitionFunc other)
		{
			int res = OldStatus.CompareTo(other.OldStatus);
			
			if (res != 0)
				return res;
			
			res = Symbol.CompareTo(other.Symbol);
			
			if (res != 0)
				return res;
			
			return NewStatus.CompareTo(other.NewStatus);				
		}
	}
}

