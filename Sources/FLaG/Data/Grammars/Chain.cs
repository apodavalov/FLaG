using System;
using System.Collections.Generic;

namespace FLaG.Data.Grammars
{
	class Chain
	{
		public Chain()
		{
			Symbols = new List<Symbol>();
		}
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}
	}
}

