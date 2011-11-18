using System;
using System.Collections.Generic;

namespace FLaG.Data.Grammar
{
	class Rule
	{
		public Unterminal Prerequisite
		{
			get;
			set;
		}
		
		public List<Chain> Chains
		{
			get;
			private set;
		}
		
		public Rule ()
		{
			Chains = new List<Chain>();
		}
	}
}

