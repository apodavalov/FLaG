using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class EqualitySet : IComparable<EqualitySet>
	{
		public int Number
		{
			get;
			set;
		}
		
		public void SaveR(Writer writer, int number, int equalitySetCollectionNum)
		{
			writer.Write(@"r_");
			writer.Write(number);
			writer.Write(@"(");
			writer.Write(equalitySetCollectionNum);
			writer.WriteLine(@")");
		}
		
		public void SaveR(Writer writer, int equalitySetCollectionNum)
		{
			SaveR(writer,Number,equalitySetCollectionNum);
		}
		
		public void SaveSet(Writer writer, bool IsLeft)
		{
			if (Set.Count == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < Set.Count; i++)
				{
					if (i != 0)		
						writer.Write(", ");
					
					Set[i].Save(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public List<NStatus> Set
		{
			get;
			private set;
		}
		
		public bool AddStatus(NStatus item)
		{
			int index = Set.BinarySearch(item);
			if (index < 0)
				Set.Insert(~index,item);
			
			return index < 0;
		}
		
		public EqualitySet()
		{
			Set = new List<NStatus>();
		}

		public int CompareTo(EqualitySet other)
		{
			if (Set.Count < other.Set.Count)
				return -1;
			else if (Set.Count > other.Set.Count)
				return 1;
			
			for (int i = 0; i < Set.Count; i++)
			{
				int res = Set[i].CompareTo(other.Set[i]);
					
				if (res != 0)
					return res;
			}
			
			return 0;
		}
	}
}

