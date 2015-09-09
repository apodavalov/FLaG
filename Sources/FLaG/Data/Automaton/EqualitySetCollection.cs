using FLaG.Output;
using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class EqualitySetCollection : IComparable<EqualitySetCollection>
	{
		public int Number
		{
			get;
			set;
		}
		
		public void SaveR(Writer writer)
		{
			writer.Write(@"R(");
			writer.Write(Number);
			writer.WriteLine(@")");
		}
		
		public void SaveRR(Writer writer)
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
					
					Set[i].SaveR(writer,i+1,Number);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public void SaveSets(Writer writer, bool IsLeft)
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
					
					Set[i].SaveSet(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public int GetStatusGroupNum(NStatus status)
		{
			for (int i = 0; i < Set.Count; i++)
			{
				int res = Set[i].Set.BinarySearch(status);
				if (res >= 0)
					return i;
			}
			
			return -1;
		}
		
		public List<EqualitySet> Set
		{
			get;
			private set;
		}
		
		public bool AddEqualitySet(EqualitySet item)
		{
			int index = Set.BinarySearch(item);
			if (index < 0)
				Set.Insert(~index,item);
			
			return index < 0;
		}
		
		public EqualitySetCollection ()
		{
			Set = new List<EqualitySet>();
		}
		
		public int CompareTo(EqualitySetCollection other)
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

