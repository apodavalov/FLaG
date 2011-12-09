using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class DStatus : IComparable<DStatus>
	{
		public List<NStatus> Set
		{
			get;
			private set;
		}
		
		public DStatus()
		{
			Set = new List<NStatus>();
		}
		
		public bool AddStatus(NStatus item)		
		{
			int index = Set.BinarySearch(item);
			if (index < 0)
				Set.Insert(~index,item);
			
			return index < 0;
		}			
		
		public int CompareTo(DStatus other)
		{
			int min = Math.Min(Set.Count,other.Set.Count);
			
			for (int i = 0; i < min; i++)
			{
				int res = Set[i].CompareTo(other.Set[i]);
				if (res != 0)
					return res;
			}
			
			if (Set.Count > min)
				return 1;
			else if (other.Set.Count > min)
				return -1;
			else
				return 0;
		}
	}
}
