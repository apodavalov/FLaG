using System;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class Symbol : IComparable<Symbol>, IEquatable<Symbol>
	{
		public void Save(Writer writer)
		{
			writer.WriteLine(Value);
		}
		
		public char Value
		{
			get;
			set;
		}
		
		public Symbol(char value)
		{
			Value = value;
		}
	
		public int CompareTo(Symbol other)
		{
			return Value.CompareTo(other.Value);
		}

		public bool Equals (Symbol other)
		{
			if (other == null)
				return false;

			return CompareTo(other) == 0;
		}

		public override bool Equals (object obj)
		{
			return Equals(obj as Symbol);
		}

		public override int GetHashCode ()
		{
			return Value.GetHashCode();
		}
	}
}

