using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class DAutomaton
	{
		public bool ProducedFromDFA
		{
			get;
			set;
		}
		
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
		
		public bool AddFunc(DTransitionFunc item)		
		{
			int index = Functions.BinarySearch(item);
			if (index < 0)
				Functions.Insert(~index,item);
			
			return index < 0;
		}		
		
		public bool AddEndStatus(DStatus item)		
		{
			return AddStatus(EndStatuses,item);
		}	
		
		private bool AddStatus(List<DStatus> statuses, DStatus item)
		{
			int index = statuses.BinarySearch(item);
			if (index < 0)
				statuses.Insert(~index,item);
			
			return index < 0;
		}
		
		public List<DStatus> EndStatuses
		{
			get;
			private set;
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
			EndStatuses = new List<DStatus>();
		}
	}
}

