using System;

namespace FLaG.Data.Automaton
{
	class SimpleStatus : Status, IComparable<SimpleStatus>
	{
		public int? Number
		{
			get;
			set;
		}
		
		public char Value
		{
			get;
			set;
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
			return (Number ?? 0).GetHashCode() ^ Value.GetHashCode();
		}
		
		public int CompareTo(Status other)
		{
			if (other is SimpleStatus)
				return CompareTo((SimpleStatus)other);
			else
				return -1;
		}
		
		public int CompareTo(SimpleStatus other)
		{
			int res = Value.CompareTo(other.Value);
			
			if (res != 0)
				return res;
			
			if (Number == null && other.Number != null)
				return -1;
			
			if (Number != null && other.Number == null)
				return 1;
			
			if (Number == null && other.Number == null)
				return 0;
			
			return Number.Value.CompareTo(other.Number.Value);
		}
	}
}

