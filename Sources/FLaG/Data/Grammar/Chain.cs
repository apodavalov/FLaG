using System;
using System.Collections.Generic;

namespace FLaG.Data.Grammar
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

