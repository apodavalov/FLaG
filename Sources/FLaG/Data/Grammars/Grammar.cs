using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Grammar
	{
		public void RemoveUnreachedSyms(Writer writer, int newGrammarNumber)
		{
			int oldGrammarNumber = Number;
			writer.WriteLine(@"Производим удаление недостижимых символов грамматики",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			Number = newGrammarNumber;
			writer.WriteLine(@"В результате построим грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", не содержащую указанные символы.",true);			
			writer.WriteLine(@"Для этого нам понадобится дополнительное множество",true);			
			writer.WriteLine(@"\emph{V},");			
			writer.WriteLine(@"в которое будем заносить достижимые символы.",true);			
			HashSet<Symbol> V = new HashSet<Symbol>();
			V.Add(TargetSymbol);
			int i = 0;
			writer.WriteLine();
			writer.WriteLine(@"Итак, на первом шаге алгоритма положим",true);			
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i);
			writer.WriteLine(@"=");
			SaveVSet(writer,V);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"и",true);			
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"i=");
			i++;
			writer.WriteLine(i);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На следующем шаге получаем множество",true);	
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", содержащее символы из множество, построенного на предыдущем шаге",true);	
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i-1);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", а также символы, которые входят в правые части правил для нетерминала,",true);	
			writer.WriteLine(@"принадлежащего множеству",true);
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i-1);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"В результате получаем",true);
			
			List<Unterminal> unterminals = new List<Unterminal>();
			
			foreach (Rule r in Rules)
				unterminals.Add(r.Prerequisite);
			
			bool isAddedSomething;
			
			do
			{
				isAddedSomething = false;
				HashSet<Symbol> oldV = V;
				V = new HashSet<Symbol>();
				
				foreach (Symbol s in oldV)
				{
					if (s is Unterminal)
					{
						Unterminal u = (Unterminal)s;
						
						int index = unterminals.BinarySearch(u);
						
						if (index >= 0)
						{
							foreach (Chain c in Rules[index].Chains)
							{
								foreach (Symbol ss in c.Symbols)
								{
									if (!oldV.Contains(ss) && !V.Contains(ss))
										isAddedSomething = true;
									V.Add(ss);
								}
							}
						}
					}
				}
				
				writer.WriteLine();
				writer.WriteLine(@"\noindent\begin{math}");
				SaveV(writer,i);
				writer.WriteLine(@"=");
				SaveVSet(writer,oldV);
				writer.WriteLine(@"\cup");
				SaveVSet(writer,V);
				writer.WriteLine(@"=");
				oldV.UnionWith(V);
				V = oldV;
				SaveVSet(writer,V);
				writer.WriteLine(@"\end{math}");
				writer.WriteLine();
				
				if (isAddedSomething)
				{
					writer.WriteLine(@"Сравниваем",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i);
					writer.WriteLine(@"\end{math},");
					writer.WriteLine(@"и получаем, что",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i-1);
					writer.WriteLine(@"\neq");
					SaveV(writer,i);
					writer.WriteLine(@"\end{math}.");
					i++;
					writer.WriteLine(@"Делаем приращение",true);
					writer.WriteLine(@"\begin{math}");
					writer.WriteLine(@"i");
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и строим множество",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"вида",true);
				}
				else
				{
					writer.WriteLine(@"Сравниваем",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i);
					writer.WriteLine(@"\end{math},");
					writer.WriteLine(@"и получаем, что",true);
					writer.WriteLine(@"\begin{math}");
					SaveV(writer,i-1);
					writer.WriteLine(@"=");
					SaveV(writer,i);
					writer.WriteLine(@"\end{math}.");
				}
				
			} while (isAddedSomething);
			
			Unterminal oldSymbol = TargetSymbol;
			TargetSymbol = Unterminal.GetInstance(Number);
			
			writer.WriteLine(@"Строим искомую грамматику",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}");
			TargetSymbol = oldSymbol;
			writer.WriteLine(@", где соответствующие элементы грамматики примут следующие значения:",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveN(writer);
			writer.WriteLine(@"=");
			Number = oldGrammarNumber;
			SaveN(writer);
			Number = newGrammarNumber;
			writer.WriteLine(@"\cap");
			SaveV(writer,i);
			writer.WriteLine(@"=\{");
			SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\cap");
			SaveVSet(writer,V);
			writer.WriteLine(@"=");
			HashSet<Symbol> newN = new HashSet<Symbol>(Unterminals);
			newN.IntersectWith(V);
			SaveVSet(writer,newN);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество нетермильнальных символов граммактики");
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\Sigma \cap");
			SaveV(writer,i);
			writer.WriteLine(@"=\{");
			SaveAlphabet(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\cap");
			SaveVSet(writer,V);
			writer.WriteLine(@"=");
			HashSet<Symbol> newAlphabet = new HashSet<Symbol>(Alphabet);
			newAlphabet.IntersectWith(V);
			SaveVSet(writer,newAlphabet);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество термильнальных символов граммактики");
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");		
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");		
			SaveP(writer);
			writer.WriteLine(@"=\{");
			bool atLeastOneRemoved = false;
			for (int j = 0; j < Rules.Count; j++)
				if (!newN.Contains(Rules[j].Prerequisite))
				{
					Rules.RemoveAt(j);
					j--;
					atLeastOneRemoved = true;
				}
			SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");		
			writer.WriteLine(@"--- множество правил вывода для данной грамматики");
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");		
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			Unterminal.GetInstance(Number).Save(writer,IsLeft);			
			writer.WriteLine(@"\equiv");
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@"--- целевой символ грамматики");
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");					
			writer.WriteLine();
			writer.WriteLine(@"В результате выполнения этого шага алгоритма приведения грамматики",true);
			if (atLeastOneRemoved)
				writer.WriteLine(@"произошло удаление недостижимых символов.",true);				
			else
				writer.WriteLine(@"удаление недостижимых символов не произошло.",true);
			writer.WriteLine();
		}
		
		public void Normalize()
		{
			foreach (Rule r in Rules)
				r.Normalize();
			
			Rules.Sort();
			
			// объединяем цепочки правил с одинаковыми целевыми символами			
			int i = 0;
			while (i < Rules.Count)
			{
				int j = i + 1;
				
				while (j < Rules.Count && Rules[i].Prerequisite.CompareTo(Rules[j].Prerequisite) == 0)
				{
					Rules[i].Chains.AddRange(Rules[j].Chains);
					Rules.RemoveAt(j);
				}
				
				i++;
			}
			
			foreach (Rule r in Rules)
				r.Normalize();
		}
		
		public void CheckLangForEmpty (Writer writer)
		{
			writer.WriteLine(@"Проверим на пустоту грамматику",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			
			writer.WriteLine(@"\begin{enumerate}");
			
			int i = 0;
			HashSet<Unterminal> C = new HashSet<Unterminal>();
			writer.WriteLine(@"\item ");
			writer.WriteLine(@"\begin{math}");
			SaveC(writer,i);
			writer.Write(@"=");
			SaveCSet(writer,C);
			writer.Write(@",");
			writer.Write(@"i=");
			writer.Write(i);
			writer.WriteLine(@";\end{math}");
			
			bool isAddedSomething;
			
			do 
			{
				i++;
				isAddedSomething = false;
				HashSet<Unterminal> oldC = C;
				C = new HashSet<Unterminal>();
				
				foreach (Rule r in Rules)
				{	
					foreach (Chain c in r.Chains)
					{
						bool ourChain = true;	
						
						foreach (Symbol s in c.Symbols) 
							if (s is Unterminal)
							{
								Unterminal u = (Unterminal)s;
								
								if (!oldC.Contains(u))
								{
									ourChain = false;
									break;
								}
							}
						
						if (ourChain)
						{
							if (!C.Contains(r.Prerequisite) && !oldC.Contains(r.Prerequisite))
							{
								C.Add(r.Prerequisite);
								isAddedSomething = true;
							}
							break;
						}
					}
				}
				
				writer.WriteLine(@"\item ");
				writer.WriteLine(@"\begin{math}");
				SaveC(writer,i);
				writer.WriteLine(@"=");
				
				if (isAddedSomething)
				{
					SaveC(writer,i-1);
					writer.WriteLine(@"\cup");
					SaveCSet(writer,C);
					oldC.UnionWith(C);
					C = oldC;
					writer.WriteLine(@"=");
					SaveCSet(writer,C);
					writer.Write(@",i=");
					writer.Write(i);
					writer.WriteLine(";");
				}
				else
				{
					oldC.UnionWith(C);
					C = oldC;
					SaveC(writer,i-1);
					writer.Write(@",i=");
					writer.Write(i);
					writer.WriteLine(".");
				}
				
				writer.WriteLine(@"\end{math}");
				

			} while (isAddedSomething);
			
			writer.WriteLine(@"\end{enumerate}");			
			writer.WriteLine();
			writer.WriteLine();
			
			bool containsTarget = C.Contains(TargetSymbol);
			
			writer.WriteLine(@"\begin{math}");
			TargetSymbol.Save(writer,IsLeft);
			if (containsTarget)
				writer.WriteLine(@"\in");
			else
				writer.WriteLine(@"\notin");
			SaveC(writer,i);
			writer.WriteLine(@"\Rightarrow");
			writer.WriteLine(@"\end{math}");
			if (containsTarget)
				writer.WriteLine(@"язык не пуст.",true);
			else
				writer.WriteLine(@"язык пуст.",true);
			writer.WriteLine();
		}
		
		private void SaveV(Writer writer, int Number)
		{
			writer.Write(@"{V");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
		}
		
		private void SaveC(Writer writer, int Number)
		{
			writer.Write(@"{C");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
		}
		
		private void SaveVSet(Writer writer, HashSet<Symbol> V)
		{
			if (V.Count == 0)
			{
				writer.WriteLine(@"\varnothing");
				return;
			}
			else
			{
				writer.WriteLine(@"\{");
				List<Symbol> list = new List<Symbol>(V);
				
				for (int i = 0; i < list.Count; i++)
				{
					if (i != 0)
						writer.Write(@",");
					
					list[i].Save(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		private void SaveCSet(Writer writer, HashSet<Unterminal> C)
		{
			if (C.Count == 0)
			{
				writer.WriteLine(@"\varnothing");
				return;
			}
			else
			{
				writer.WriteLine(@"\{");
				List<Unterminal> list = new List<Unterminal>(C);
				
				for (int i = 0; i < list.Count; i++)
				{
					if (i != 0)
						writer.Write(@",");
					
					list[i].Save(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
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

		public void SaveAlphabet(Writer writer)
		{
			Terminal[] terminals = Alphabet;
			
			for (int i = 0; i < terminals.Length; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				terminals[i].Save(writer, IsLeft);
			}
		}
	
		public Terminal[] Alphabet
		{
			get
			{
				List<Terminal> terminals = new List<Terminal>();
				
				foreach (Rule r in Rules)
					foreach (Chain c in r.Chains)
						foreach (Symbol s in c.Symbols)
							if (s is Terminal)
							{
								Terminal u = (Terminal)s;
								int index = terminals.BinarySearch(u);
								if (index < 0)
									terminals.Insert(~index,u);
							}
				
				return terminals.ToArray();
			}
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
		
		public static void SaveBothUnterminals(Writer writer, bool isLeft, params Unterminal[][] uss)
		{
			List<Unterminal> unterminals = new List<Unterminal>();
			
			foreach (Unterminal[] us in uss)
				foreach (Unterminal u in us)
				{
					int index = unterminals.BinarySearch(u);
					if (index < 0)
						unterminals.Insert(~index,u);
				}	
			
			for (int i = 0; i < unterminals.Count; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				unterminals[i].Save(writer, isLeft);
			}
		}
		
		public void SaveUnterminals(Writer writer)
		{
			Unterminal[] unterminals = Unterminals;
			
			for (int i = 0; i < unterminals.Length; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				unterminals[i].Save(writer, IsLeft);
			}
		}
		
		public void SaveRules(Writer writer)
		{
			for (int i = 0; i < Rules.Count; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				Rules[i].Save(writer, IsLeft);
			}
		}
		
		private void SaveSigmaWithNum(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write(@"\Sigma");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
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
		
		public Grammar()
		{
			Rules = new List<Rule>();
		}
	}
}

