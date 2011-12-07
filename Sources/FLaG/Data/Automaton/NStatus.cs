using System;

namespace FLaG.Data.Automaton
{
	class NStatus : IComparable<NStatus>
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

		public int CompareTo(NStatus other)
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

