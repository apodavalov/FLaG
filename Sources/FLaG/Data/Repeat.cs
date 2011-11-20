using System;
using FLaG.Output;
using System.Collections.Generic;
using FLaG.Data.Grammars;

namespace FLaG.Data
{
	class Repeat : Entity
	{
		public bool AtLeastOne
		{
			get;
			set;
		}
		
		public Entity Entity
		{
			get;
			set;
		}
		
		public override Symbol[] CollectAlphabet()
		{
			return Entity.CollectAlphabet();	
		}
		
		public override Entity DeepClone()
		{
			Repeat r = new Repeat();
			
			r.AtLeastOne = AtLeastOne;
			r.Entity = Entity.DeepClone();
			r.NumLabel = NumLabel;
			
			return r;
		}

		public override void Save(Writer writer)
		{
			throw new NotSupportedException();
		}
		
		public override Entity ToRegularSet()
		{
			throw new NotSupportedException();
		}

		public override Entity ToRegularExp()
		{
			throw new NotSupportedException();
		}
		
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			if (Entity is Symbol || Entity is Concat)
				Entity.SaveAsRegularExp(writer,full);
			else
			{
				writer.Write(@"\left(");
				Entity.SaveAsRegularExp(writer,full);
				writer.Write(@"\right)");
			}					
			
			writer.Write("^");
			if (AtLeastOne)
            	writer.Write('+');
			else
				writer.Write(@"\ast ");
			writer.Write("}");
			
			if (full)
			{
				writer.Write(@"_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}
		}
	
		public override int MarkDeepest(int val, List<Entity> list)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			val = Entity.MarkDeepest(val,list);
			
			if (oldval == val)
			{
				list.Add(this);
				NumLabel = val;
				val++;
			}
			
			return val;
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft)
		{
			Grammar = new Grammar();
			Grammar.IsLeft = isLeft;
			Grammar.Number = NumLabel.Value;
			Grammar.TargetSymbol = new Unterminal();
			Grammar.TargetSymbol.Number = Grammar.Number;
			
			writer.WriteLine(@"\item");
			writer.WriteLine("Для выражения" , true);
			writer.WriteLine(@"\begin{math}");
			SaveAsRegularExp(writer, false);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.Write(@", которое является ", true);
			if (AtLeastOne)
				writer.Write("положительной ", true);
			writer.WriteLine(@"итерацией выражения для которого построена грамматика",true);
			writer.WriteLine(@"\begin{math}");
			Entity.Grammar.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@", строим грамматику", true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveCortege(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@", где", true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveN(writer);
			writer.WriteLine(@"=");
			Entity.Grammar.SaveN(writer);
			writer.WriteLine();
			writer.WriteLine(@"\bigcup");
			writer.WriteLine(@"\{");
			Grammar.TargetSymbol.Save(writer,Grammar.IsLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");
			
			writer.WriteLine(@"\{");
			Entity.Grammar.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			
			writer.WriteLine(@"\bigcup ");
			
			writer.WriteLine(@"\{");
			Grammar.TargetSymbol.Save(writer, isLeft);
			writer.WriteLine(@"\}");
			
			List<Rule> onlyTerms,others;
			
			if (isLeft)
			{
				Entity.Grammar.SplitRules(out onlyTerms, out others);
				
				Grammar.Rules.AddRange(others);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = new Rule();
					
					r.Prerequisite = rule.Prerequisite;
					
					foreach (Chain c in rule.Chains)
					{
						Chain newChain = c.DeepClone();						
						newChain.Symbols.Insert(0,Grammar.TargetSymbol);
						r.Chains.Add(newChain);
						r.Chains.Add(c);
					}
					
					Grammar.Rules.Add(r);
				}
			}
			else
			{
				Entity.Grammar.SplitRules(out onlyTerms, out others);
				
				Grammar.Rules.AddRange(others);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = new Rule();					
					
					r.Prerequisite = rule.Prerequisite;
					
					foreach (Chain c in rule.Chains)
					{
						Chain newChain = c.DeepClone();						
						newChain.Symbols.Add(Grammar.TargetSymbol);
						r.Chains.Add(newChain);
						r.Chains.Add(c);
					}
					
					Grammar.Rules.Add(r);
				}
			}
			
			Rule newRule = new Rule();
			newRule.Prerequisite = Grammar.TargetSymbol;	
			Chain nc = new Chain();
			nc.Symbols.Add(Entity.Grammar.TargetSymbol);
			newRule.Chains.Add(nc);
			
			if (!AtLeastOne)
				newRule.Chains.Add(new Chain());
			
			Grammar.Rules.Add(newRule);
			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			Grammar.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine("--- множество нетермильнальных символов грамматики", true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(";",true);
			
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveP(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			Grammar.Rules.RemoveAt(Grammar.Rules.Count - 1);
			Grammar.SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\bigcup");
			writer.WriteLine(@"\{");		
			newRule.Save(writer,isLeft);
			writer.WriteLine(@"\}");			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			Grammar.Rules.Add(newRule);
			Grammar.SaveRules(writer);
			writer.WriteLine(@"\}");							
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@"--- множество правил вывода для данной грамматики; ",true);
			
			writer.WriteLine(@"\begin{math}");			
			Grammar.TargetSymbol.Save(writer,isLeft);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}.");
		}
	}
}

