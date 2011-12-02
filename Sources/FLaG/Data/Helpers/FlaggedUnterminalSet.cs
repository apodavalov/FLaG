using System;
using System.Collections.Generic;
using FLaG.Data.Grammars;

namespace FLaG.Data.Helpers
{
	class FlaggedUnterminalSet
	{
		public FlaggedUnterminalSet()
		{
			Changed = true;
			RemovedFromFuture = false;
			Set = new HashSet<Unterminal>();
		}
		
		public bool Changed
		{
			get;
			set;
		}
		
		public bool RemovedFromFuture
		{
			get;
			set;
		}
		
		public HashSet<Unterminal> Set
		{
			get;
			set;
		}
	}
}

