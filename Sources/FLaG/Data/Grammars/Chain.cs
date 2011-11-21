using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Chain
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
		
		public Chain DeepClone(bool cloneSymbols)
		{
			Chain chain = new Chain();
			
			if (cloneSymbols)
				foreach (Symbol s in Symbols)
					chain.Symbols.Add(s.DeepClone());
			else
				foreach (Symbol s in Symbols)
					chain.Symbols.Add(s);
			
			return chain;
		}
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}
	}
}

