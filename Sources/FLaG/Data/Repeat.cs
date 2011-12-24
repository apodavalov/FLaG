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
		
		public override void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum)
		{
			Grammar = new Grammar();
			Grammar.IsLeft = isLeft;
			Grammar.Number = NumLabel.Value;
			Grammar.TargetSymbol = Unterminal.GetInstance(Grammar.Number);
			
			writer.WriteLine(@"\item");
			writer.WriteLine("Для выражения" , true);
			writer.WriteLine(@"\begin{math}");
			SaveAsRegularExp(writer, false);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.Write(@", которое является ", true);
			if (AtLeastOne)
				writer.Write("положительной ", true);
			writer.WriteLine(@"итерацией выражения с грамматикой",true);
			writer.WriteLine(@"\begin{math}");
			Entity.Grammar.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@", построим грамматику", true);
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
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");
			Grammar.TargetSymbol.Save(writer,Grammar.IsLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");
			
			writer.WriteLine(@"\{");
			Entity.Grammar.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			
			writer.WriteLine(@"\cup ");
			
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
						Grammar.AddChain(r,newChain);
						Grammar.AddChain(r,c);
					}
					
					Grammar.AddRule(Grammar.Rules,r);
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
						Grammar.AddChain(r,newChain);
						Grammar.AddChain(r,c);
					}
					
					Grammar.AddRule(Grammar.Rules,r);
				}
			}
			
			Rule newRule = new Rule();
			newRule.Prerequisite = Grammar.TargetSymbol;	
			Chain nc = new Chain();
			nc.Symbols.Add(Entity.Grammar.TargetSymbol);
			Grammar.AddChain(newRule,nc);
			
			if (!AtLeastOne)
				Grammar.AddChain(newRule,new Chain());

			bool addedNewRule = Grammar.AddRule(Grammar.Rules, newRule);
			
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
			if (addedNewRule)
			{
				int index = Grammar.Rules.BinarySearch(newRule);
				Grammar.Rules.RemoveAt(index);
			}
			Grammar.SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");		
			newRule.Save(writer,isLeft);
			writer.WriteLine(@"\}");			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			if (addedNewRule)
				Grammar.AddRule(Grammar.Rules,newRule);
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

