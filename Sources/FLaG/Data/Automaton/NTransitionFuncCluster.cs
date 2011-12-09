using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class NTransitionFuncCluster
	{
		public Symbol Symbol
		{
			get;
			set;
		}

		private bool AddStatus(List<NStatus> statuses, NStatus item)
		{
			int index = statuses.BinarySearch(item);
			
			if (index < 0)
				statuses.Insert(~index,item);
			
			return index < 0;
		}
		
		public NStatus[] OldStatuses
		{
			get
			{
				List<NStatus> statuses = new List<NStatus>();
				
				foreach (NTransitionFunc func in Functions)
					AddStatus(statuses,func.OldStatus);
				
				return statuses.ToArray();
			}
		}
		
		public NTransitionFunc[] Functions
		{
			get;
			set;
		}
	}
}

