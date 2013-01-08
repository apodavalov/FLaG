using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class DStatus : IComparable<DStatus>, IEquatable<DStatus>
	{
		public void Save(Writer writer, bool isLeft, bool producedFromDFA)
		{
			if (!producedFromDFA)
				writer.WriteLine(@"[");
				
			foreach (NStatus status in Set)
				status.Save(writer,isLeft);
			
			if (!producedFromDFA)
				writer.WriteLine(@"]");
		}
		
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

		public bool Equals (DStatus other)
		{
			return CompareTo(other) == 0;
		}
		
		public override bool Equals (object obj)
		{
			return Equals(obj as DStatus);
		}
		
		public override int GetHashCode ()
		{
			int hash = 0;
			foreach (NStatus s in Set)
				hash ^= s.GetHashCode();

			return hash;
		}
		
		public int CompareTo(DStatus other)
		{
			if (other == null)
				return 1;

			int res = Set.Count.CompareTo(other.Set.Count);
			
			if (res != 0)
				return res;
			
			int count = other.Set.Count;
			
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
