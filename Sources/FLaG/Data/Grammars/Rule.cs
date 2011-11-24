using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Rule : IComparable<Rule>
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
		
		public Rule DeepClone()
		{
			Rule rule = new Rule();
			rule.Prerequisite = Prerequisite;
			
			foreach (Chain c in Chains)
				rule.Chains.Add(c.DeepClone());
			
			return rule;
		}
		
		public Rule()
		{
			Chains = new List<Chain>();
		}
		
		public void Normalize()
		{
			Chains.Sort();
			
			// удалим из коллекции одинаковые правила
			int i = 0;
			while (i < Chains.Count)
			{
				int j = i + 1;
				
				while (j < Chains.Count && Chains[i].CompareTo(Chains[j]) == 0)
					Chains.RemoveAt(j);
				
				i++;
			}
		}

		public int CompareTo(Rule other)
		{
			int res = Prerequisite.CompareTo(other.Prerequisite);
			
			if (res != 0)
				return res;
			
			// сравниваем до первого несовпадения 
			// условно говоря, сравнение не совсем корректное,
			// так как во внимание берется порядок (хотя он совершенно не важен)
			// для того, чтобы правильно работала эта функция
			// необходимо, чтобы оба правила были нормализованы
			
			int min = Math.Min(Chains.Count,other.Chains.Count);
			
			for (int i = 0; i < min; i++)
			{
				res = Chains[i].CompareTo(other.Chains[i]);
				
				if (res != 0)
					return res;
			}
			
			if (Chains.Count < other.Chains.Count)
				return -1;
			else if (Chains.Count > other.Chains.Count)
				return 1;
			else
				return 0;
		}
	}
}

