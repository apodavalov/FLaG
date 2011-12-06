using System;

namespace FLaG.Data.Automaton
{
	class TransitionFunc : IComparable<TransitionFunc>
	{
		public Status OldStatus
		{
			get;
			set;
		}
		
		public Symbol Symbol
		{
			get;
			set;
		}
		
		public Status NewStatus
		{
			get;
			set;
		}
		
		public int CompareTo(TransitionFunc other)
		{
			throw new NotImplementedException();
		}
	}
}

