using System;

namespace FLaG.Data.Automaton
{
	class Symbol : IComparable<Symbol>
	{
		public char Value
		{
			get;
			set;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Symbol)
				return CompareTo((Symbol)obj) == 0;
			else
				return false;
		}
		
		public override int GetHashCode ()
		{
			return Value.GetHashCode();
		}
		
		public int CompareTo(Symbol other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}

