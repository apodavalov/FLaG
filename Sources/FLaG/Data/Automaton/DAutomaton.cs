using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class DAutomaton
	{
		public bool IsLeft
		{
			get;
			set;
		}
		
		public int Number		
		{
			get;
			set;
		}
		
		public bool Add(DTransitionFunc item)		
		{
			int index = Functions.BinarySearch(item);
			if (index < 0)
				Functions.Insert(~index,item);
			
			return index < 0;
		}			
		
		public List<DTransitionFunc> Functions
		{
			get;
			private set;
		}
		
		public DStatus InitialStatus
		{
			get;
			set;
		}
		
		public DAutomaton()
		{
			Functions = new List<DTransitionFunc>();	
		}
	}
}

