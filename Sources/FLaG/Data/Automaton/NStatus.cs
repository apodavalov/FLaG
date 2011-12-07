using System;

namespace FLaG.Data.Automaton
{
	class NStatus : IComparable<NStatus>
	{
		public void Save (FLaG.Output.Writer writer, bool isLeft)
		{
			throw new NotImplementedException ();
		}
		
		public void Save (FLaG.Output.Writer writer)
		{
			throw new NotImplementedException ();
		}
		
		public NStatus(char value)
		{
			Value = value;
		}
		
		public NStatus(char value, int number)
		{
			Value = value;
			Number = number;
		}
		
		public int? Number
		{
			get;
			private set;
		}
		
		public char Value
		{
			get;
			private set;
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

