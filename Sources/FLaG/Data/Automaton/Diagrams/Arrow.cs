using System;
using System.Collections.Generic;
using FLaG.Data.Automaton;

namespace FLaG.Automaton.Diagrams
{
	class Arrow : IComparable<Arrow>
	{
		public int A
		{
			get;
			set;
		}
		
		public int B
		{
			get;
			set;			
		}
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}
		
		public Arrow()
		{
			Symbols = new List<Symbol>();
		}
		
		public bool AddSymbol(Symbol item)
		{
			int index = Symbols.BinarySearch(item);
			
			if (index < 0)
				Symbols.Insert(~index,item);
			
			return index < 0;
		}
		
		public int CompareTo (Arrow other)
		{
			int res = A.CompareTo(other.A);
			if (res != 0)
				return res;
			
			return B.CompareTo(other.B);
		}
	}
}

