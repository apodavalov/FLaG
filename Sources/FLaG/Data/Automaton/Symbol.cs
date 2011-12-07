using System;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class Symbol : IComparable<Symbol>
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
	}
}

