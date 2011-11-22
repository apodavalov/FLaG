using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Grammar
	{
		public int Number
		{
			get;
			set;
		}
		
		public Unterminal TargetSymbol
		{
			get;
			set;
		}
		
		public bool IsLeft
		{
			get;
			set;
		}
		
		public string Apostrophs
		{
			get
			{
				if (IsLeft)
					return "'";
				else
					return "''";
			}
		}
		
		public List<Rule> Rules
		{
			get;
			private set;
		}
		
		public Unterminal[] Unterminals
		{
			get
			{
				List<Unterminal> unterminals = new List<Unterminal>();
				
				for (int i = 0; i < Rules.Count; i++)	
				{
					int index = unterminals.BinarySearch(Rules[i].Prerequisite);
					if (index < 0)
						unterminals.Insert(~index,Rules[i].Prerequisite);
				}
				
				return unterminals.ToArray();
			}
		}
		
		public Grammar MakeMirror(ref int LastUseNumber, ref int AdditionalGrammarNumber)
		{
			Grammar g = DeepClone();
			
			g.Number = AdditionalGrammarNumber++;
			
			Dictionary<Unterminal,Unterminal> replacesDictionary = 
				new Dictionary<Unterminal, Unterminal>();
			
			if (!replacesDictionary.ContainsKey(g.TargetSymbol))
				replacesDictionary.Add(g.TargetSymbol,Unterminal.GetInstance(LastUseNumber++));
			
			g.TargetSymbol = replacesDictionary[g.TargetSymbol];
			
			foreach (Rule r in g.Rules)
			{
				if (!replacesDictionary.ContainsKey(r.Prerequisite))
					replacesDictionary.Add(r.Prerequisite,Unterminal.GetInstance(LastUseNumber++));
				
				r.Prerequisite = replacesDictionary[r.Prerequisite];

				foreach (Chain c in r.Chains)
					for (int i = 0; i < c.Symbols.Count; i++)
						if (c.Symbols[i] is Unterminal)
						{
							Unterminal u = (Unterminal)c.Symbols[i];
							if (!replacesDictionary.ContainsKey(u))
								replacesDictionary.Add(u,Unterminal.GetInstance(LastUseNumber++));
							c.Symbols[i] = replacesDictionary[u];
						}
			}
			
			return g;
		}
		
		public Grammar DeepClone()
		{
			Grammar grammar = new Grammar();
			
			grammar.Number = Number;
			grammar.TargetSymbol = TargetSymbol;
			grammar.IsLeft = IsLeft;
			
			foreach (Rule r in Rules)
				grammar.Rules.Add(r.DeepClone());
				
			return grammar;
		}
		
		public static void SaveBothUnterminals(Writer writer, Unterminal[] u1, Unterminal[] u2, bool isLeft)
		{
			List<Unterminal> unterminals = new List<Unterminal>();
			
			foreach (Unterminal u in u1)
			{
				int index = unterminals.BinarySearch(u);
				if (index < 0)
					unterminals.Insert(~index,u);
			}
			
			foreach (Unterminal u in u2)
			{
				int index = unterminals.BinarySearch(u);
				if (index < 0)
					unterminals.Insert(~index,u);
			}
			
			for (int i = 0; i < unterminals.Count; i++)
			{
				if (i != 0)		
					writer.Write(",");
				
				unterminals[i].Save(writer, isLeft);
			}
		}
		
		public void SaveUnterminals(Writer writer)
		{
			Unterminal[] unterminals = Unterminals;
			
			for (int i = 0; i < unterminals.Length; i++)
			{
				if (i != 0)		
					writer.Write(",");
				
				unterminals[i].Save(writer, IsLeft);
			}
		}
		
		public void SaveRules(Writer writer)
		{
			for (int i = 0; i < Rules.Count; i++)
			{
				if (i != 0)		
					writer.Write(",");
				
				Rules[i].Save(writer, IsLeft);
			}
		}
		
		private void SaveLetter(char Letter, Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write(Letter.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveCortege(Writer writer)
		{
			SaveLetter('G',writer);
			writer.Write(@"=");
			writer.Write(@"\left(");			
			SaveN(writer);
			writer.Write(@",");
			writer.Write(@"\Sigma ");
			writer.Write(@",");			
			SaveP(writer);						
			writer.Write(@",");
			TargetSymbol.Save(writer,IsLeft);					
			writer.Write(@"\right)");
		}
		
		public void SplitRules(out List<Rule> onlyTerms, out List<Rule> others)
		{
			onlyTerms = new List<Rule>();
			others = new List<Rule>();
			
			foreach (Rule r in Rules)
			{
				Rule onlyTerm = new Rule();
				Rule other = new Rule();
				
				other.Prerequisite = onlyTerm.Prerequisite = r.Prerequisite;
				
				foreach (Chain c in r.Chains)
				{
					bool allIsTerminals = true;
					foreach (Symbol s in c.Symbols)
						if (s is Unterminal)
						{
							allIsTerminals = false;	
							break;
						}
					
					if (allIsTerminals)
						onlyTerm.Chains.Add(c);
					else
						other.Chains.Add(c);
				}
				
				if (onlyTerm.Chains.Count > 0)
					onlyTerms.Add(onlyTerm);
				
				if (other.Chains.Count > 0)
					others.Add(other);
			}
		}
		
		public void SaveN(Writer writer)
		{
			SaveLetter('N',writer);
		}
		
		public void SaveP(Writer writer)
		{
			SaveLetter('P',writer);
		}
		
		public void SaveG(Writer writer)
		{
			SaveLetter('G',writer);
		}
		
		public Grammar ()
		{
			Rules = new List<Rule>();
		}
	}
}

