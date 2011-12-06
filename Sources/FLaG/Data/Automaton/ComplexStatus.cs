using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class ComplexStatus : List<SimpleStatus>, Status, IComparable<ComplexStatus>
	{
		public new bool Add(SimpleStatus item)		
		{
			int index = BinarySearch(item);
			if (index < 0)
				Insert(~index,item);
			
			return index < 0;
		}			
		
		public ComplexStatus() 
			: base()
		{

		}
		
		public override bool Equals(object obj)
		{
			if (obj is Status)
				return CompareTo((Status)obj) == 0;
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (SimpleStatus s in this)
				hashCode ^= s.GetHashCode();
			
			return hashCode;
		}
		
		public int CompareTo(Status other)
		{
			if (other is ComplexStatus)
				return CompareTo((ComplexStatus)other);
			else
				return 1;
		}

		public int CompareTo(ComplexStatus other)
		{
			int min = Math.Min(Count,other.Count);
			
			for (int i = 0; i < min; i++)
			{
				int res = this[i].CompareTo(other[i]);
				if (res != 0)
					return res;
			}
			
			if (Count > min)
				return 1;
			else if (other.Count > min)
				return -1;
			else
				return 0;
		}
	}
}
