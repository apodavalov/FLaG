using System;
using FLaG.Output;
using Gram=FLaG.Data.Grammars;

namespace FLaG.Data.Equation
{
	class Matrix
	{
		private Expression[][] Mx
		{
			get;
			set;
		}
		
		private Gram.Unterminal[] Unterminals
		{
			get;
			set;
		}
		
		public bool IsLeft
		{
			get;
			set;
		}
		
		public void Save(Writer writer)
		{
			throw new NotImplementedException();
		}
		
		public void Solve(Writer writer)
		{
			throw new NotImplementedException();
		}
		
		public Matrix (Gram.Grammar g)
		{
			Unterminals = g.DeepUnterminals;
			
			Mx = new Expression[Unterminals.Length][];
			
			for (int i = 0; i < Unterminals.Length; i++)
				Mx[i] = new Expression[Unterminals.Length + 1];
			
			IsLeft = g.IsLeft;
			
			foreach (Gram.Rule rule in g.Rules)
			{
				Alter[] alters = new Alter[Unterminals.Length + 1];								
				foreach (Gram.Chain chain in rule.Chains)
				{
					int colNum;
					
					switch (chain.Symbols.Count)
					{
						case 0:
							if (alters[alters.Length - 1] == null)
								alters[alters.Length - 1] = new Alter();
						
							alters[alters.Length - 1].Expressions.Add(new Empty());
							break;
						case 1:
							if (chain.Symbols[0] is Gram.Unterminal)
							{
								Gram.Unterminal u = (Gram.Unterminal)chain.Symbols[0];
							
								colNum = Array.BinarySearch<Gram.Unterminal>(Unterminals,u);
								
								if (alters[colNum] == null)
									alters[colNum] = new Alter();
							
								alters[colNum].Expressions.Add(new Empty());
							}
							else if (chain.Symbols[0] is Gram.Terminal)
							{
								Gram.Terminal t = (Gram.Terminal)chain.Symbols[0];
							
								if (alters[alters.Length - 1] == null)
									alters[alters.Length - 1] = new Alter();
								
								alters[alters.Length - 1].Expressions.Add(new Symbol(t.Value));
							}
							break;
						case 2:
							Gram.Unterminal u = null;								
							Gram.Terminal t = null;
						
							if (chain.Symbols[0] is Gram.Unterminal)
								u = (Gram.Unterminal)chain.Symbols[0];
						
							if (chain.Symbols[0] is Gram.Terminal)
								t = (Gram.Terminal)chain.Symbols[0];
						
							if (chain.Symbols[1] is Gram.Unterminal)
								u = (Gram.Unterminal)chain.Symbols[1];
						
							if (chain.Symbols[1] is Gram.Terminal)
								t = (Gram.Terminal)chain.Symbols[1];
						
							if (u != null && t != null)
							{
								colNum = Array.BinarySearch<Gram.Unterminal>(Unterminals,u);
								
								if (alters[colNum] == null)
									alters[colNum] = new Alter();
							
								alters[colNum].Expressions.Add(new Symbol(t.Value));
							}
						
							break;
					}
				}
				
				int rowNum = Array.BinarySearch<Gram.Unterminal>(Unterminals, rule.Prerequisite);				
				
				for (int i = 0; i < alters.Length; i++)
					Mx[rowNum][i] = alters[i] == null ? null : alters[i].Optimize();
			}
		}
	}
}
