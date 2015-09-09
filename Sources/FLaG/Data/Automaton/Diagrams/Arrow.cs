using FLaG.Data.Automaton;
using System;
using System.Collections.Generic;
using System.Text;

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
		
		public string Text
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				
				for (int i = 0; i < Symbols.Count; i++)
				{
					if (i != 0)
						sb.Append(',');
					
					sb.Append(Symbols[i].Value);
				}
				
				return sb.ToString();
			}
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

