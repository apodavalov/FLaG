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
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}
	}
}
