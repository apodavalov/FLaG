using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
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
		
		public void Save(Writer writer, bool isLeft)
		{
			Prerequisite.Save(writer, isLeft);			
			writer.Write(@"\rightarrow ");
			
			if (Chains.Count > 0)
				for (int i = 0; i < Chains.Count; i++)
				{
					if (i != 0)
						writer.Write(@"\mid ");
					
					Chains[i].Save(writer, isLeft);
				}
			else
				writer.Write(@"{\varepsilon}");
		}
		
		public Rule ()
		{
			Chains = new List<Chain>();
		}
	}
}

