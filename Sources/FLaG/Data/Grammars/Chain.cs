using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Chain : IComparable<Chain>
	{
		public Chain()
		{
			Symbols = new List<Symbol>();
		}
		
		public void Save(Writer writer, bool isLeft)
		{
			if (Symbols.Count > 0)
				for (int i = 0; i < Symbols.Count; i++)
					Symbols[i].Save(writer,isLeft);		
			else
				writer.Write(@"{\varepsilon}");
		}
		
		public Chain DeepClone()
		{
			Chain chain = new Chain();
			
			foreach (Symbol s in Symbols)
				chain.Symbols.Add(s);
			
			return chain;
		}
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}

		public int CompareTo(Chain other)
		{
			// сравниваем элементы попарно до первого несовпадения
			int min = Math.Min(Symbols.Count,other.Symbols.Count);
			
			for (int i = 0; i < min; i++)
			{
				int res = Symbols[i].CompareTo(other.Symbols[i]);
				if (res != 0)
					return res;
			}
			
			if (Symbols.Count < other.Symbols.Count)
				return -1;
			else if (Symbols.Count > other.Symbols.Count)
				return 1;
			else
				return 0;
		}
	}
}

