using System;
using System.Collections.Generic;
using FLaG.Output;
using FLaG.Data.Helpers;

namespace FLaG.Data.Grammars
{
	class Grammar
	{
		public void MakeAutomatonGrammar(Writer writer, int newGrammarNumber)
		{
			int oldGrammarNumber = Number;
			// newGrammarNumber - с этого же номера начинаем
			// нумеровать дополнительные нетерминальные символы 
			// грамматики
			
			// удалим пустые правила (оно единственное и привязано к целевому символу)
			bool haveEmptyRuleForS = false;
			for (int i = 0; i < Rules.Count; i++)
				for (int j = 0; j < Rules[i].Chains.Count; j++)
				{
					if (Rules[i].Chains[j].Symbols.Count == 0)	
					{
						haveEmptyRuleForS = true;
						Rules[i].Chains.RemoveAt(j);
						j--;
					}
				}
			
			writer.WriteLine(@"Приведем регулярную грамматику к автоматной, для этого грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			Number = newGrammarNumber;
			writer.WriteLine(@"и построим грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			Number = oldGrammarNumber;
			writer.WriteLine(@"\end{math}.");
			
			List<Rule> newRules = new List<Rule>();
			
			writer.WriteLine();
			
			writer.WriteLine(@"\begin{math}");
			SaveN(writer);
			writer.WriteLine(@"=");
			Number = newGrammarNumber;
			SaveN(writer);
			writer.WriteLine(@",",true);
			Number = oldGrammarNumber;			
			Unterminal.GetInstance(newGrammarNumber).Save(writer,IsLeft);
			writer.WriteLine(@"=");			
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			writer.WriteLine(@"В силу того, что",true);
			writer.WriteLine(@"\begin{math}");
			if (haveEmptyRuleForS)
				writer.WriteLine(@"\varepsilon \in L(");
			else
				writer.WriteLine(@"\varepsilon \notin L(");
			SaveG(writer);
			writer.WriteLine(@")");
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"то",true);			
			Number = newGrammarNumber;
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"=");
			
			if (haveEmptyRuleForS)
			{
				Rule rule = new Rule();
				rule.Prerequisite = TargetSymbol;
				rule.Chains.Add(new Chain());
				newRules.Add(rule);
			}
			
			// TODO: SaveRules должна сама это делать
			if (newRules.Count == 0)
				writer.WriteLine(@"\varnothing");
			else
				SaveRules(writer,newRules);
			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			
			writer.WriteLine(@"Построим остальные правила грамматики");
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			
			List<Unterminal> newUnterminals = new List<Unterminal>(Unterminals);
			
			int firstUnterminalNumber = newGrammarNumber + 1;
			
			foreach (Rule r in Rules)
				foreach (Chain c in r.Chains)
				{
					Rule pseudoRule = new Rule();
					pseudoRule.Prerequisite = r.Prerequisite;
					pseudoRule.Chains.Add(c.DeepClone());
				
					List<Rule> chainNewRules = new List<Rule>();
				
					bool needAlgorithm = false;
					Unterminal u = null;
				
					if (c.Symbols.Count > 1)
					{
						if (c.Symbols[IsLeft ? 0 : c.Symbols.Count - 1] is Unterminal)
						{	
							if (c.Symbols.Count > 2) // в этом случае выполняем алгоритм 3.2
							{
								needAlgorithm = true;							
								// извлекаем сразу символ
								u = (Unterminal)c.Symbols[IsLeft ? 0 : c.Symbols.Count - 1];
							}
						}
						else
							needAlgorithm = true; // алгоритм 3.3
					}				
				
					if (!needAlgorithm)
					{
						Rule rule = new Rule();
						rule.Prerequisite = r.Prerequisite;
						rule.Chains.Add(c.DeepClone());
						AddRule(chainNewRules,rule);
					}
					else
					{
						if (IsLeft)
						{
							Rule rule = new Rule();							
							Chain chain = new Chain();
							if (u != null)
								chain.Symbols.Add(u);
							chain.Symbols.Add(c.Symbols[0]);
							rule.Prerequisite = Unterminal.GetInstance(firstUnterminalNumber++);
							rule.Chains.Add(chain);							
							AddRule(chainNewRules,rule);
						
							for (int j = 1; j < c.Symbols.Count - 1; j++)
							{
								rule = new Rule();							
								chain = new Chain();
								chain.Symbols.Add(Unterminal.GetInstance(firstUnterminalNumber-1));
								chain.Symbols.Add(c.Symbols[j]);
								rule.Prerequisite = Unterminal.GetInstance(firstUnterminalNumber++);
								rule.Chains.Add(chain);
								AddRule(chainNewRules,rule);
							}	
						
							rule = new Rule();							
							chain = new Chain();
							chain.Symbols.Add(Unterminal.GetInstance(firstUnterminalNumber-1));
							chain.Symbols.Add(c.Symbols[c.Symbols.Count - 1]);
							rule.Prerequisite = r.Prerequisite;
							rule.Chains.Add(chain);
							AddRule(chainNewRules,rule);
						}
						else
						{
							Rule rule = new Rule();														
							rule.Prerequisite = r.Prerequisite;
							Chain chain = new Chain();
							chain.Symbols.Add(c.Symbols[0]);
							chain.Symbols.Add(Unterminal.GetInstance(firstUnterminalNumber++));													
							rule.Chains.Add(chain);
							AddRule(chainNewRules,rule);						
						
							for (int j = 1; j < c.Symbols.Count - 1; j++)
							{
								rule = new Rule();							
								rule.Prerequisite = Unterminal.GetInstance(firstUnterminalNumber);
								chain = new Chain();																
								chain.Symbols.Add(c.Symbols[j]);
								chain.Symbols.Add(Unterminal.GetInstance(firstUnterminalNumber++));
								rule.Chains.Add(chain);
								AddRule(chainNewRules,rule);
							}	
						
							rule = new Rule();							
							rule.Prerequisite = Unterminal.GetInstance(firstUnterminalNumber);
							chain = new Chain();
							chain.Symbols.Add(c.Symbols[c.Symbols.Count - 1]);
							if (u != null)
								chain.Symbols.Add(u);
							rule.Chains.Add(chain);							
							AddRule(chainNewRules,rule);
						}
					}
				
					writer.WriteLine();
					writer.WriteLine(@"Обработаем правило",true);
					writer.WriteLine(@"\begin{math}");
					pseudoRule.Save(writer,IsLeft);
					writer.WriteLine(@"\end{math}.");
				
					if (!needAlgorithm)
					{
						writer.WriteLine("Данное правило переносится без изменений в",true);
						writer.WriteLine(@"\begin{math}");
						SaveP(writer);
						writer.WriteLine(@"\end{math}.");
					}
					else
					{
						writer.WriteLine("Данное правило преобразовывается в множество правил и данное множество добавляется в",true);
						writer.WriteLine(@"\begin{math}");
						SaveP(writer);
						writer.WriteLine(@"\end{math}.");	
					}
				
					writer.WriteLine();
					writer.WriteLine(@"\begin{math}");
					SaveP(writer);
					writer.WriteLine(@"=");
					SaveP(writer);
					writer.WriteLine(@"\cup");
					writer.WriteLine(@"\{");
					SaveRules(writer,chainNewRules);
					writer.WriteLine(@"\}");
					writer.WriteLine(@"=");
					foreach (Rule rule in chainNewRules)
						AddRule(newRules,rule);
					writer.WriteLine(@"\{");
					SaveRules(writer,newRules);
					writer.WriteLine(@"\}");
					writer.WriteLine(@"\end{math}");
				
					if (needAlgorithm)
					{
						writer.WriteLine();
						SaveN(writer);
						writer.WriteLine(@"=");
						SaveN(writer);
						writer.WriteLine(@"\cup");
						Unterminal[] unterminals = GetUnterminals(chainNewRules);
						writer.WriteLine(@"\{");	
						SaveUnterminals(writer,unterminals);
						writer.WriteLine(@"\}");	
						foreach (Unterminal uu in unterminals)
						{
							int index = newUnterminals.BinarySearch(uu);
							if (index < 0)
								newUnterminals.Insert(~index,uu);
						}
					
						writer.WriteLine(@"=");
						writer.WriteLine(@"\{");	
						SaveUnterminals(writer,newUnterminals.ToArray());
						writer.WriteLine(@"\}");	
					}
				}
			
			Rules.Clear();
			Rules.AddRange(newRules);
			
			writer.WriteLine();
			
			writer.WriteLine(@"Итак, окончательно грамматика",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);	
			writer.WriteLine(@"\end{math}");	
			writer.WriteLine(@"--- это четверка вида",true);
			writer.WriteLine(@"\begin{math}");	
			SaveCortege(writer);
			writer.WriteLine(@"\end{math},");	
			writer.WriteLine(@"где соответствующие элементы грамматики принимают следующие значения",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");	
			SaveN(writer);
			writer.WriteLine(@"=\{");				
			SaveUnterminals(writer);
			writer.WriteLine(@"\}");	
			writer.WriteLine(@"\end{math}");	
			writer.WriteLine(@"--- множество нетерминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);
			writer.WriteLine(@"\end{math};");	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			writer.Write(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество терминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);
			writer.WriteLine(@"\end{math};");	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");	
			SaveP(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			SaveRules(writer);
			writer.WriteLine(@"\}\end{math}");
			writer.WriteLine(@"--- множество правил вывода для данной грамматики;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			Unterminal.GetInstance(newGrammarNumber).Save(writer,IsLeft);
			writer.WriteLine(@"\equiv");
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
		}
		
		private bool AddRule(List<Rule> rules, Rule rule)
		{
			RuleByTargetSymbolComparer comparer = new RuleByTargetSymbolComparer();
			
			int index = rules.BinarySearch(rule,comparer);
			
			if (index < 0)
			{
				rule.Normalize();
				rules.Insert(~index,rule);
				return true;
			}				
			else
			{
				bool result = false;
				
				foreach (Chain c in rule.Chains)	
				{
					int chainIndex = rules[index].Chains.BinarySearch(c);
					if (chainIndex < 0)
					{
						rules[index].Chains.Insert(~chainIndex,c);
						result = true;
					}
				}
				
				return result;
			}
		}
		
		private void SaveSets(Writer writer, Dictionary<Unterminal,FlaggedUnterminalSet> dictionary, int num)
		{
			SaveSets(writer,dictionary,num,false);
		}
		
		private void SaveSets(Writer writer, Dictionary<Unterminal,FlaggedUnterminalSet> dictionary, int num, bool unchangedOnly)
		{
			bool firstTime = true;
			
			foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
			{
				if (s.Value.RemovedFromFuture || unchangedOnly && s.Value.Changed)
					continue;
				
				if (firstTime)
					firstTime = false;
				else
				{
					writer.WriteLine();								
					writer.WriteLine(@";");								
				}
				
				writer.WriteLine(@"\begin{math}");
				SaveNX(writer,s.Key,num);
				writer.WriteLine(@"=");
				SaveCSet(writer,s.Value.Set);
				writer.Write(@"\end{math}");								
			}
			
			writer.WriteLine(@".");	
		}
		
		public bool RemoveChainRules(Writer writer, int newGrammarNumber)
		{
			int oldGrammarNumber = Number;
			writer.WriteLine(@"Удалим цепные правила грамматики",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			Number = newGrammarNumber;
			writer.WriteLine(@"и построим грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@"без правил вида",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"A \rightarrow B"); 
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Для этого строим для каждого нетерминала",true);
			writer.WriteLine(@"грамматики",true);
			Number = oldGrammarNumber;
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"последовательность множеств",true);
			writer.WriteLine(@"\begin{math}");
			SaveNX(writer,0);
			writer.Write(',');
			SaveNX(writer,1);
			writer.Write(',');
			SaveNX(writer,2);
			writer.Write(',');
			SaveNX(writer,3);
			writer.Write(@",\dots");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"для любого нетерминала грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"(\forall X \in ");
			SaveN(writer);
			writer.WriteLine(@")\end{math}.");			
			writer.WriteLine();
			
			writer.WriteLine(@"На первом шаге алгоритма совокупность множеств",true);
			writer.WriteLine(@"\begin{math}");
			SaveNX(writer,0);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@"примет вид",true);
			
			Unterminal[] unterminals = Unterminals;			
			
			Dictionary<Unterminal,FlaggedUnterminalSet> dictionary = 
				new Dictionary<Unterminal, FlaggedUnterminalSet>();
			
			foreach (Unterminal u in unterminals)
			{
				FlaggedUnterminalSet st = new FlaggedUnterminalSet();
				st.Set.Add(u);
				dictionary.Add(u,st);
			}
			
			SaveSets(writer,dictionary,0);
			
			int i = 1;
		
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;
				
				foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
				{
					if (s.Value.RemovedFromFuture)
						continue;
					
					s.Value.Changed = false;
					
					int index = Array.BinarySearch<Unterminal>(unterminals,s.Key);
					
					Rule r = Rules[index];
					
					foreach (Chain c in r.Chains)
					{
						if (c.Symbols.Count == 1 && c.Symbols[0] is Unterminal)												
						{
							if (s.Value.Set.Add((Unterminal)c.Symbols[0]))
							{
								s.Value.Changed = true;	
								somethingChanged = true;
							}
						}
					}
				}	
				
				writer.WriteLine(@"На следующем шаге множества примут вид",true);
				
				SaveSets(writer,dictionary,i);
				
				if (somethingChanged)
				{
					writer.WriteLine(@"Сравниваем множества",true);
					writer.WriteLine(@"\begin{math}");
					SaveNX(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveNX(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"для каждого из нетерминалов грамматики",true);
					writer.WriteLine(@"\begin{math}");
					SaveG(writer);
					writer.WriteLine(@"\end{math}.");					
					writer.WriteLine(@"Видим, что часть множеств построенных на предыдущем и текущем",true);
					writer.WriteLine(@"шаге алгоритма равны",true);
					writer.WriteLine(@"\begin{math}");
					SaveNX(writer,i-1);
					writer.WriteLine(@"=");
					SaveNX(writer,i);
					writer.WriteLine(@"\end{math},");
					writer.WriteLine(@"где",true);
					writer.WriteLine(@"\begin{math}");
					writer.WriteLine(@"X \in \{");
					SaveUnterminals(writer);
					writer.WriteLine(@"\}");
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine(@"Эти множества на следующих этапах рассматривать",true);
					writer.WriteLine(@"не будем, т.е. не рассматриваем множества вида",true);
					SaveSets(writer,dictionary,i,true);
					writer.WriteLine();
					
					foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
						if (!s.Value.Changed && !s.Value.RemovedFromFuture)
						{
							s.Value.RemovedFromFuture = true;
							s.Value.LastIndexWhenChanged = i;
						}
					
					writer.WriteLine(@"Продолжаем алгоритм для нетерминалов",true);
					
					writer.WriteLine(@"\begin{math}");
					
					bool firstTime = true;					
					foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
					{
						if (s.Value.RemovedFromFuture)
							continue;
						
						if (firstTime)
							firstTime = false;
						else
							writer.Write(',');
						
						s.Key.Save(writer,IsLeft);
					}
					writer.WriteLine(@"\end{math}.");
					i++;
				}				
				else					
				{
					foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
						if (!s.Value.RemovedFromFuture)
						{
							s.Value.RemovedFromFuture = true;
							s.Value.LastIndexWhenChanged = i;
						}
					writer.WriteLine(@"Сравниваем множества",true);
					writer.WriteLine(@"\begin{math}");
					SaveNX(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveNX(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"для каждого из нетерминалов грамматики",true);
					writer.WriteLine(@"\begin{math}");
					SaveG(writer);
					writer.WriteLine(@"\end{math}.");					
					writer.WriteLine(@"Видим, что все построенные множества на предыдущем и текущем",true);
					writer.WriteLine(@"шаге алгоритма равны, значит дальнейшее построение последовательности множеств",true);
					writer.WriteLine(@"заканчиваем и переходим к следующему шагу алгоритма, на котором из полученных",true);
					writer.WriteLine(@"множеств исключаем сам нетерминал, для которого построено данное множество, т.е.",true);
					writer.WriteLine(@"\begin{math}");
					writer.WriteLine(@"N^X={N_i}^X \setminus \{ X \}");					
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine();
				}
				
			} while (somethingChanged);
			
			writer.WriteLine(@"В итоге получаем множества вида",true);
			
			bool firstT = true;
			
			foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> s in dictionary)
			{
				if (firstT)
					firstT = false;
				else
					writer.WriteLine(';');
				
				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				SaveNX(writer,s.Key);
				writer.Write("=");
				SaveNX(writer,s.Key,s.Value.LastIndexWhenChanged);
				writer.Write(@"\setminus \{");
				s.Key.Save(writer,IsLeft);
				writer.Write(@"\}");
				writer.Write("=");
				s.Value.Set.Remove(s.Key);
				SaveCSet(writer,s.Value.Set);
				writer.Write(@"\end{math}");
			}
			
			writer.WriteLine(@".");
			writer.WriteLine();
			
			writer.WriteLine(@"На следующем шаге алгоритм строим множества нетерминальных и терминальных",true);
			writer.WriteLine(@"символов грамматики",true);
			writer.WriteLine(@"\begin{math}");
			Number = newGrammarNumber;
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			
			bool removedChainRules = false;
			
			for (int k = 0; k < Rules.Count; k++)
			{
				for (int j = 0; j < Rules[k].Chains.Count; j++)
				{
					if (Rules[k].Chains[j].Symbols.Count == 1 && Rules[k].Chains[j].Symbols[0] is Unterminal)
					{
						Rules[k].Chains.RemoveAt(j);
						j--;
						removedChainRules = true;
					}
				}
				
				if (Rules[k].Chains.Count == 0)
				{
					Rules.RemoveAt(k);
					k--;	
				}
			}
			
			writer.WriteLine(@"В итоге мы получаем следующие множества",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveN(writer);
			writer.Write(@"=\{");
			SaveDeepUnterminals(writer);
			writer.Write(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество нетерминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			writer.Write(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество терминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			Unterminal.GetInstance(newGrammarNumber).Save(writer,IsLeft);
			writer.WriteLine(@"\equiv");
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"Далее строим множество правил вывода",true);
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"В это множество включаем все правила из",true);
			Number = oldGrammarNumber;
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"\end{math},");
			Number = newGrammarNumber;
			writer.WriteLine(@"кроме цепных правил. Таким образом, множество",true);
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"примет вид",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"=\{");
			SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			writer.WriteLine(@"На последнем шаге алгоритма рассматриваем построенное множество",true);
			writer.WriteLine(@"\begin{math}");	
			SaveP(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Для всех правил из этого множества, в левых частях которых находятся",true);
			writer.WriteLine(@"нетерминалы, попавшие в построенные множества",true);
			writer.WriteLine(@"\begin{math}");	
			writer.WriteLine(@"N^X");
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где",true);
			writer.WriteLine(@"\begin{math}");	
			writer.WriteLine(@"X \in");
			SaveN(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"добавляем новое правило, путем замены нетерминала в левой части на нетерминал",true);
			writer.WriteLine(@"\begin{math}");	
			writer.WriteLine(@"X");
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"для которого построено множество",true);
			writer.WriteLine(@"\begin{math}");	
			writer.WriteLine(@"N^X");
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"В результате множество",true);
			writer.WriteLine(@"\begin{math}");	
			SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"примет вид",true);
			
			RuleByTargetSymbolComparer comparer = new RuleByTargetSymbolComparer();			
			
			bool atLeastOneChainAdded;
			
			do
			{
				atLeastOneChainAdded = false;
				
				List<Rule> newRules = new List<Rule>();
				
				foreach (Rule r in Rules)
					foreach (KeyValuePair<Unterminal,FlaggedUnterminalSet> st in dictionary)
						if (st.Value.Set.Contains(r.Prerequisite))
						{
							Rule newR = new Rule();
							newR.Prerequisite = st.Key;
							foreach (Chain c in r.Chains)						
								newR.Chains.Add(c.DeepClone());
							newRules.Add(newR);
						}
				
				foreach (Rule newR in newRules)
				{
					int indexRule = Rules.BinarySearch(newR,comparer);
					Rule r;
					
					if (indexRule < 0)
					{				
						r = new Rule();
						r.Prerequisite = newR.Prerequisite;
					}
					else
						r = Rules[indexRule];
					
					foreach (Chain c in newR.Chains)
					{
						int indexChain = r.Chains.BinarySearch(c);
						if (indexChain < 0)
						{
							r.Chains.Insert(~indexChain,c);
							atLeastOneChainAdded = true;
							removedChainRules = true;
						}
					}
					
					if (r.Chains.Count > 0 && indexRule < 0)
					{
						Rules.Insert(~indexRule,r);	
						removedChainRules = true;
					}
				}
			} while (atLeastOneChainAdded);
			
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");	
			SaveP(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			SaveRules(writer);
			writer.WriteLine(@"\}\end{math}");
			writer.WriteLine();
			
			writer.WriteLine(@"Итак, окончательно грамматика",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);	
			writer.WriteLine(@"\end{math}");	
			writer.WriteLine(@"--- это четверка вида",true);
			writer.WriteLine(@"\begin{math}");	
			SaveCortege(writer);
			writer.WriteLine(@"\end{math},");	
			writer.WriteLine(@"где соответствующие элементы грамматики принимают следующие значения",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");	
			SaveN(writer);
			writer.WriteLine(@"=\{");				
			SaveUnterminals(writer);
			writer.WriteLine(@"\}");	
			writer.WriteLine(@"\end{math}");	
			writer.WriteLine(@"--- множество нетерминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);
			writer.WriteLine(@"\end{math};");	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			writer.Write(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество терминальных символов грамматики",true);
			writer.WriteLine(@"\begin{math}");	
			SaveG(writer);
			writer.WriteLine(@"\end{math};");	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");	
			SaveP(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			SaveRules(writer);
			writer.WriteLine(@"\}\end{math}");
			writer.WriteLine(@"--- множество правил вывода для данной грамматики;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			Unterminal.GetInstance(newGrammarNumber).Save(writer,IsLeft);
			writer.WriteLine(@"\equiv");
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			return removedChainRules;
		}
		
		private bool RemoveFullEmptyRules()
		{
			bool somethingChanged = false;
			// удаляем сначала исключительно пустые правила на всякий случай
			// этот пункт не входит в отчет, поэтому писаниной заниматься тут не будем
			
			bool atLeastOneRuleDeleted;
			
			do
			{
				atLeastOneRuleDeleted = false;
				
				HashSet<Unterminal> fullEmptyUnterminals = new HashSet<Unterminal>();			
				
				for (int i = 0; i < Rules.Count; i++)
				{
					bool isAllEmpty = true;
					foreach (Chain c in Rules[i].Chains)
					{
						if (c.Symbols.Count != 0)
						{
							isAllEmpty = false;
							break;
						}
					}
					
					if (isAllEmpty)
					{
						fullEmptyUnterminals.Add(Rules[i].Prerequisite);
						if (Rules[i].Prerequisite.CompareTo(TargetSymbol) != 0)
						{
							Rules.RemoveAt(i);
							i--;
							somethingChanged = true;							
							atLeastOneRuleDeleted = true;
						}
					}
				}
				
				for (int i = 0; i < Rules.Count; i++)
					for (int j = 0; j < Rules[i].Chains.Count; j++)
						for (int k = 0; k < Rules[i].Chains[j].Symbols.Count; k++)
							if (Rules[i].Chains[j].Symbols[k] is Unterminal)	
							{
								Unterminal u = (Unterminal)Rules[i].Chains[j].Symbols[k];
								if (fullEmptyUnterminals.Contains(u))
								{
									Rules[i].Chains[j].Symbols.RemoveAt(k);
									k--;
									somethingChanged = true;
									atLeastOneRuleDeleted = true;
								}
							}
				
				Normalize();
				
			} while (atLeastOneRuleDeleted);			
						
			return somethingChanged;
		}
		
		public bool RemoveEmptyRules(Writer writer, int newGrammarNumber)
		{		
			RemoveFullEmptyRules();
			int oldGrammarNumber = Number;
			writer.WriteLine(@"Удалим пустые правила грамматики",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			Number = newGrammarNumber;
			writer.WriteLine(@"и построим грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"без пустых правил.",true);
			writer.WriteLine(@"Для этого построим последовать множеств",true);
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,0);
			writer.Write(',');
			SaveV(writer,1);
			writer.Write(',');
			SaveV(writer,2);
			writer.Write(',');
			SaveV(writer,3);
			writer.Write(@",\dots");
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@"На первом шаге алгоритма во множество",true);
			int i = 0;
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i);
			writer.WriteLine(@"\end{math}");
			Number = oldGrammarNumber;
			writer.WriteLine(@"заносим все нетерминалы, для которых во множестве правил",true);
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"имеются пустые правила. В результате получаем множество вида",true);
			HashSet<Unterminal> V = new HashSet<Unterminal>();
			foreach (Rule r in Rules)
			{
				bool atLeastOneEmptyChain = false;
				foreach (Chain c in r.Chains)
					if (c.Symbols.Count == 0)
					{
						atLeastOneEmptyChain = true;
						break;
					}
				if (atLeastOneEmptyChain)
					V.Add(r.Prerequisite);
			}
			
			writer.WriteLine(@"\begin{math}");
			SaveV(writer,i);
			writer.WriteLine(@"=");
			SaveCSet(writer,V);
			writer.WriteLine(@"\end{math}");
			
			i++;
		
			bool isAddedSomething;
			
			do
			{
				isAddedSomething = false;
				HashSet<Unterminal> oldV = V;
				V = new HashSet<Unterminal>();
				
				foreach (Rule r in Rules)
					foreach (Chain c in r.Chains)
					{
						bool isAllSymbolsInV = true;
						
						foreach (Symbol s in c.Symbols)
						{
							if (s is Unterminal)
							{
								Unterminal u = (Unterminal)s;
							
								if (!oldV.Contains(u))
									isAllSymbolsInV = false;
							}
							else
								isAllSymbolsInV = false;
						
							if (!isAllSymbolsInV)
								break;
						}
					
						if (isAllSymbolsInV)
						{
							if (!V.Contains(r.Prerequisite) && !oldV.Contains(r.Prerequisite))
								isAddedSomething = true;
						
							V.Add(r.Prerequisite);																			
							break;
						}
					}
				
				writer.WriteLine();
				writer.WriteLine(@"\noindent\begin{math}");
				SaveV(writer,i);
				writer.WriteLine(@"=");
				SaveCSet(writer,oldV);
				writer.WriteLine(@"\cup");
				SaveCSet(writer,V);
				writer.WriteLine(@"=");
				oldV.UnionWith(V);
				V = oldV;
				SaveCSet(writer,V);
				writer.WriteLine(@"\end{math}");
				writer.WriteLine();
				
				if (isAddedSomething)
				{
					writer.WriteLine(@"Сравниваем множества",true);
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
					writer.WriteLine(@"Делаем приращение",true);
					writer.WriteLine(@"\begin{math}");
					writer.WriteLine(@"i");
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и строим множество",true);
					writer.WriteLine(@"\begin{math}");
					i++;
					SaveV(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"вида",true);
				}
				else
				{
					writer.WriteLine(@"Сравниваем множества",true);
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
			
			writer.WriteLine(@"Алгоритм останавливается.",true);
			writer.WriteLine(@"В результате получаем грамматику",true);
			Unterminal oldTargetSymbol = TargetSymbol;
			TargetSymbol = Unterminal.GetInstance(Number);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", где соответствующие элементы грамматики примут следующие значения",true);
			
			bool somethingChanged = false;
			
			Chain emptyChain = new Chain();
			
			bool targetSymbolRuleHasEmptyChain = false;
			
			foreach (Rule r in Rules)
			{
				if (r.Chains.Count > 1 && r.Prerequisite.CompareTo(oldTargetSymbol) != 0)				
				{
					int index = r.Chains.BinarySearch(emptyChain);
					if (index >= 0)
					{
						r.Chains.RemoveAt(index);
						somethingChanged = true;
					}
				}			
				
				if (r.Prerequisite.CompareTo(oldTargetSymbol) == 0)
					targetSymbolRuleHasEmptyChain = 
						targetSymbolRuleHasEmptyChain | r.Chains.BinarySearch(emptyChain) >= 0;
			}
			
			foreach (Rule r in Rules)
			{
				List<Chain> newChains = new List<Chain>();
				
				foreach (Chain c in r.Chains)
				{
					Chain newChain = c.DeepClone();
					
					for (int j = 0; j < newChain.Symbols.Count; j++)
					{
						if (newChain.Symbols[j] is Unterminal)
						{
							Unterminal u = (Unterminal)newChain.Symbols[j];
							if (V.Contains(u))
							{
								newChain.Symbols.RemoveAt(j);
								j--;
							}
						}
					}
					
					if (emptyChain.CompareTo(newChain) != 0)
					{
						int index = newChains.BinarySearch(newChain);
						if (index < 0)
							newChains.Insert(~index,newChain);
					}
				}
				
				foreach (Chain c in newChains)
				{
					int index = r.Chains.BinarySearch(c);
					if (index < 0)
					{
						r.Chains.Insert(~index,c);
						somethingChanged = true;
					}
				}
			}
			
			bool isNewTargetSymbol;
			
			if (V.Contains(oldTargetSymbol) && !targetSymbolRuleHasEmptyChain)
			{
				Rule rule;
				rule = new Rule();
				rule.Prerequisite = TargetSymbol;
				Chain c = new Chain();				
				rule.Chains.Add(c);
				
				c = new Chain();
				c.Symbols.Add(oldTargetSymbol);
				rule.Chains.Insert(~rule.Chains.BinarySearch(c),c);
				
				int index = Rules.BinarySearch(rule);
				if (index < 0)
				{
					Rules.Insert(~index,rule);
					somethingChanged = true;
				}
				
				isNewTargetSymbol = true;
			}
			else
				isNewTargetSymbol = false;

			Number = newGrammarNumber;
			writer.WriteLine(@"\begin{math}");
			SaveN(writer);
			writer.WriteLine(@"=\{");
			SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество нетерминальных символов граммактики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			Number = oldGrammarNumber;
			writer.WriteLine(@"=");
			SaveSigmaWithNum(writer);
			Number = newGrammarNumber;
			writer.WriteLine(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество терминальных символов граммактики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");			
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"=\{");
			SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество правил вывода для данной граммактики;",true);
			writer.WriteLine(@"\begin{math}");
			TargetSymbol.Save(writer,IsLeft);
			if (!isNewTargetSymbol)
			{
				writer.WriteLine(@"\equiv");
				TargetSymbol = oldTargetSymbol;
				TargetSymbol.Save(writer,IsLeft);
			}
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");	
			writer.WriteLine();	
			writer.WriteLine(@"В результате выполнения этого шага алгоритма приведения грамматики",true);
			if (somethingChanged)
				writer.WriteLine(@"произошло удаление пустых правил.",true);				
			else
				writer.WriteLine(@"удаление пустых правил не произошло.",true);
			writer.WriteLine();	
			
			return somethingChanged;
		}
		
		public bool RemoveUselessSyms (Writer writer, int newGrammarNumber)
		{
			int oldGrammarNumber = Number;
			writer.WriteLine(@"Удалим бесплодные символы грамматики",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			Number = newGrammarNumber;
			writer.WriteLine(@"и построим грамматику",true);			
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", не содержащую бесплодные символы.",true);
			writer.WriteLine(@"Бесплодным является символ, который не порождает",true);
			writer.WriteLine(@"ни одной сентенциальной формы данной грамматики. Другими",true);
			writer.WriteLine(@"словами мы будем работать с множеством нетерминальных символов.",true);
			writer.WriteLine(@"Для этого построим последовать множеств",true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer,0);
			writer.Write(',');
			SaveM(writer,1);
			writer.Write(',');
			SaveM(writer,2);
			writer.Write(',');
			SaveM(writer,3);
			writer.Write(@",\dots");
			writer.WriteLine(@"\end{math}");
			
			int i = 0;
			HashSet<Unterminal> M = new HashSet<Unterminal>();			
			writer.WriteLine();
			writer.WriteLine(@"Полагаем, что ", true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer,i);
			writer.WriteLine("=");
			SaveCSet(writer,M);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Тогда следующий элемент этой последовательности примет вид", true);
			
			bool isAddedSomething;
			
			Terminal[] alphabet = Alphabet;
			HashSet<Symbol> C = new HashSet<Symbol>(alphabet);
			i++;
			
			do
			{				
				isAddedSomething = false;
				C.UnionWith(M);
				HashSet<Unterminal> oldM = M;
				M = new HashSet<Unterminal>();
				
				foreach (Rule r in Rules)
					foreach (Chain c in r.Chains)
					{
						bool isAllSymbolsInC = true;
						
						foreach (Symbol s in c.Symbols)
							if (!C.Contains(s))
							{	
								isAllSymbolsInC = false;
								break;
							}
					
						if (isAllSymbolsInC)
						{
							if (!M.Contains(r.Prerequisite) && !oldM.Contains(r.Prerequisite))
								isAddedSomething = true;
						
							M.Add(r.Prerequisite);																			
							break;
						}
					}
				
				writer.WriteLine();
				writer.WriteLine(@"\noindent\begin{math}");
				SaveM(writer,i);
				writer.WriteLine(@"=");
				SaveCSet(writer,M);
				writer.WriteLine(@"\cup");
				SaveM(writer,i-1);
				writer.WriteLine(@"=");
				SaveCSet(writer,M);
				writer.WriteLine(@"\cup");
				SaveCSet(writer,oldM);
				writer.WriteLine(@"=");
				oldM.UnionWith(M);
				M = oldM;
				SaveCSet(writer,M);
				writer.WriteLine(@"\end{math}.");
				writer.WriteLine();
					
				if (isAddedSomething)
				{
					writer.WriteLine(@"Сравниваем множества",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i);
					writer.WriteLine(@"\end{math},");
					writer.WriteLine(@"и получаем, что",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i-1);
					writer.WriteLine(@"\neq");
					SaveM(writer,i);
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine(@"Делаем приращение",true);
					writer.WriteLine(@"\begin{math}");
					writer.WriteLine(@"i");
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и строим множество",true);
					writer.WriteLine(@"\begin{math}");
					i++;
					SaveV(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"вида",true);
				}
				else
				{
					writer.WriteLine(@"Сравниваем множества",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i-1);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine(@"и",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i);
					writer.WriteLine(@"\end{math},");
					writer.WriteLine(@"и получаем, что",true);
					writer.WriteLine(@"\begin{math}");
					SaveM(writer,i-1);
					writer.WriteLine(@"=");
					SaveM(writer,i);
					writer.WriteLine(@"\end{math}.");
				}
			} while (isAddedSomething);
			
			writer.WriteLine(@"Алгоритм останавливается.",true);
			writer.WriteLine(@"В результате получаем грамматику",true);
			Unterminal oldTargetSymbol = TargetSymbol;
			TargetSymbol = Unterminal.GetInstance(Number);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", где соответствующие элементы грамматики примут следующие значения",true);
			writer.WriteLine(@"\begin{math}");
			SaveN(writer);
			writer.WriteLine(@"=");
			SaveM(writer,i);
			writer.WriteLine(@"=");
			SaveCSet(writer,M);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество нетерминальных символов граммактики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");
			writer.WriteLine(@"\begin{math}");
			SaveSigmaWithNum(writer);
			Number = oldGrammarNumber;
			writer.WriteLine(@"=");
			SaveSigmaWithNum(writer);
			Number = newGrammarNumber;
			writer.WriteLine(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество терминальных символов граммактики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math};");			
			bool atLeastOneRemoved = false;
			for (int j = 0; j < Rules.Count; j++)
				if (!M.Contains(Rules[j].Prerequisite))
				{
					Rules.RemoveAt(j);
					j--;
					atLeastOneRemoved = true;
				}
			writer.WriteLine(@"\begin{math}");
			SaveP(writer);
			writer.WriteLine(@"=\{");
			SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество правил вывода для данной граммактики;",true);
			writer.WriteLine(@"\begin{math}");
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\equiv");
			TargetSymbol = oldTargetSymbol;
			TargetSymbol.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			SaveG(writer);
			writer.WriteLine(@"\end{math}.");	
			writer.WriteLine();	
			writer.WriteLine(@"В результате выполнения этого шага алгоритма приведения грамматики",true);
			if (atLeastOneRemoved)
				writer.WriteLine(@"произошло удаление бесплодных (бесполезных) символов.",true);				
			else
				writer.WriteLine(@"удаление бесплодных (бесполезных) символов не произошло.",true);
			writer.WriteLine();	
			
			return atLeastOneRemoved;
		}
		
		public bool RemoveUnreachedSyms(Writer writer, int newGrammarNumber)
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
			writer.WriteLine(@"=");
			SaveAlphabet(writer);
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
			
			return atLeastOneRemoved;
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
		
		private void SaveNX(Writer writer, int Number)
		{
			writer.Write("{{N_{");
			writer.Write(Number);
			writer.Write("}}^X}");
		}
		
		private void SaveNX(Writer writer, Unterminal unterminal)
		{
			writer.Write("{N^{");
			unterminal.Save(writer,IsLeft);
			writer.Write("}}");
		}
		
		private void SaveNX(Writer writer, Unterminal unterminal, int Number)
		{
			writer.Write("{{N_{");
			writer.Write(Number);
			writer.Write("}}^{");			
			unterminal.Save(writer,IsLeft);
			writer.Write("}}");
		}
		
		private void SaveM(Writer writer, int Number)
		{
			writer.Write(@"{M");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
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
			
			if (terminals.Length == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < terminals.Length; i++)
				{
					if (i != 0)		
						writer.Write(", ");
					
					terminals[i].Save(writer, IsLeft);
				}
				writer.WriteLine(@"\}");
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
		
		private Unterminal[] GetDeepUnterminals(List<Rule> rules)
		{
			List<Unterminal> unterminals = new List<Unterminal>();
			
			for (int i = 0; i < rules.Count; i++)	
			{
				int index = unterminals.BinarySearch(rules[i].Prerequisite);
				if (index < 0)
					unterminals.Insert(~index,rules[i].Prerequisite);
				
				foreach (Chain c in rules[i].Chains)
					foreach (Symbol s in c.Symbols)						
						if (s is Unterminal)
						{
							Unterminal u = (Unterminal)s;
							index = unterminals.BinarySearch(u);
							if (index < 0)
								unterminals.Insert(~index,u);
						}
			}
			
			return unterminals.ToArray();
		}
		
		private Unterminal[] GetUnterminals(List<Rule> rules)
		{
			List<Unterminal> unterminals = new List<Unterminal>();
			
			for (int i = 0; i < rules.Count; i++)	
			{
				int index = unterminals.BinarySearch(rules[i].Prerequisite);
				if (index < 0)
					unterminals.Insert(~index,rules[i].Prerequisite);
			}
			
			return unterminals.ToArray();
		}
		
		public Unterminal[] DeepUnterminals
		{
			get
			{
				return GetDeepUnterminals(Rules);
			}
		}
		
		public Unterminal[] Unterminals
		{
			get
			{
				return GetUnterminals(Rules);
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
		
		public void SaveDeepUnterminals(Writer writer)
		{
			SaveUnterminals(writer, DeepUnterminals);			
		}
		
		public void SaveUnterminals(Writer writer)
		{
			SaveUnterminals(writer, Unterminals);			
		}
		
		public void SaveUnterminals(Writer writer, Unterminal[] unterminals)
		{
			for (int i = 0; i < unterminals.Length; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				unterminals[i].Save(writer, IsLeft);
			}
		}
		
		public void SaveRules(Writer writer)
		{
			SaveRules(writer,Rules);
		}
		
		public void SaveRules(Writer writer, List<Rule> rules)
		{
			for (int i = 0; i < rules.Count; i++)
			{
				if (i != 0)		
					writer.Write(", ");
				
				rules[i].Save(writer, IsLeft);
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

