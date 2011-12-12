using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class ClassEqualityCollection : IComparable<ClassEqualityCollection>
	{
		public List<ClassEquality> Set
		{
			get;
			private set;
		}
		
		public bool AddStatus(NStatus item)
		{
			int index = Statuses.BinarySearch(item);
			if (index < 0)
				Statuses.Insert(~index,item);
			
			return index < 0;
		}

		
		public List<NStatus> Statuses
		{
			get;
			private set;
		}
		
		public bool AddClassEquality(ClassEquality item)
		{
			int index = Set.BinarySearch(item);
			if (index < 0)
				Set.Insert(~index,item);
			
			return index < 0;
		}
		
		public ClassEqualityCollection ()
		{
			Set = new List<ClassEquality>();
			Statuses = new List<NStatus>();
		}

		public int CompareTo(ClassEqualityCollection other)
		{
			int res = Set.Count.CompareTo(other.Set.Count);
			
			if (res != 0)
				return res;
			
			int count = Set.Count;
			
			for (int i = 0; i < count; i++)
			{
				res = Set[i].CompareTo(other.Set[i]);
				
				if (res != 0)
					return res;
			}
			
			return 0;
		}
	}
}

