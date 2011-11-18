using System;
using System.Collections.Generic;

namespace FLaG.Data.Grammar
{
	class Grammar
	{
		public List<Rule> Rules
		{
			get;
			private set;
		}
		
		public Unterminal[] Unterminals
		{
			get
			{
				Unterminal[] unterminals = new Unterminal[Rules.Count];
				
				for (int i = 0; i < Rules.Count; i++)					
					unterminals[i] = Rules[i].Prerequisite;
				
				return unterminals;
			}
		}
		
		public Grammar ()
		{
			
		}
	}
}

