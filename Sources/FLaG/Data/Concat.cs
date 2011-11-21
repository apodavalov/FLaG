using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

namespace FLaG.Data
{
    class Concat : Entity
    {
        public Concat() 
            : base()
        {
            EntityCollection = new List<Entity>();
        }
		
		public override Symbol[] CollectAlphabet()
		{
			List<Symbol> symbols = new List<Symbol>();
			
			foreach (Entity e in EntityCollection)
			{
				Symbol[] smbs = e.CollectAlphabet();
				foreach (Symbol s in smbs)
				{
					int index = symbols.BinarySearch(s);
					
					if (index < 0)
						symbols.Insert(~index,s);
				}
			}
			
			return symbols.ToArray();
		}
		
		public override Entity DeepClone()
		{
			Concat c = new Concat();
			
			foreach (Entity e in EntityCollection)
				c.EntityCollection.Add(e.DeepClone());
			
			c.NumLabel = NumLabel;
			
			return c;
		}

        public Concat(XmlReader reader, List<Variable> variableCollection) 
            : this()
        {
            while (!reader.IsStartElement("concat")) reader.Read();

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();

            while (reader.IsStartElement())
                EntityCollection.Add(Entity.Load(reader, variableCollection));

            if (!isEmpty)
                reader.ReadEndElement();
        }

        public List<Entity> EntityCollection
        {
            get;
            private set;
        }

        public override void Save(Writer writer)
        {
            string times;
			
			writer.Write("{");

            if (EntityCollection.All<Entity>(x => x is Symbol))
                // invisible
                times = " ";
            else
                // visible
                times = @" \cdot ";

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(times);

                EntityCollection[i].Save(writer);
            }                      
			
			writer.Write("}");
        }

        public override Entity ToRegularSet()
        {
            Concat c = new Concat();

            foreach (Entity e in EntityCollection)
                c.EntityCollection.Add(e.ToRegularSet());

            return c;
        }

		public override Entity ToRegularExp ()
		{
			Concat c = new Concat();

            foreach (Entity e in EntityCollection)
                c.EntityCollection.Add(e.ToRegularExp());

            return c;
		}
		
		private void SaveAsRegularExpWithUnderbraces(Writer writer)
		{
			if (EntityCollection.Count > 1)			
				writer.Write(@"\left(");					
			
			if (EntityCollection.Count < 2)
			{
				writer.Write(@"{\underbrace");			
				writer.Write("{");
			}
			else
				for (int i = 1; i < EntityCollection.Count; i++)
				{
					writer.Write(@"{\underbrace");			
					writer.Write("{");
				}

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");

                EntityCollection[i].SaveAsRegularExp(writer,true);
				
				if (i > 0)
				{
					writer.Write("}");
					writer.Write(@"_\text{");			
					writer.Write(NumLabel - (EntityCollection.Count - 1) + i);
					writer.Write("}}");
				}
            }                   
			
			if (EntityCollection.Count < 2)
			{
					writer.Write("}");
					writer.Write(@"_\text{");			
					writer.Write(NumLabel);
					writer.Write("}}");
			}
			
			if (EntityCollection.Count > 1)
				writer.Write(@"\right)");
		}
		
		private void SaveAsRegularExpWithoutUnderbraces(Writer writer)
		{
			writer.Write("{");
			
			if (EntityCollection.Count > 1)			
				writer.Write(@"\left(");

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");

                EntityCollection[i].SaveAsRegularExp(writer,false);
            }                      
			
			if (EntityCollection.Count > 1)
				writer.Write(@"\right)");
			
			writer.Write("}");
		}
	
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				SaveAsRegularExpWithUnderbraces(writer);			
			else
				SaveAsRegularExpWithoutUnderbraces(writer);	
		}

		public override int MarkDeepest(int val, List<Entity> list)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			foreach (Entity e in EntityCollection)
				val = e.MarkDeepest(val, list);
			
			if (oldval == val)
			{
				list.Add(this);
				if (EntityCollection.Count < 3)
					val++;
				else
					val+=EntityCollection.Count - 1;
				NumLabel = val-1;
			}
			
			return val;
		}
		
		private Grammar MergeGrammars(Grammar grammar1, Grammar grammar2, Concat concat, int Number, Writer writer, bool isLeft)
		{
			Grammar grammar = new Grammar();
			
			writer.WriteLine(@"\item");
			writer.WriteLine("Для выражения вида" , true);
			writer.WriteLine(@"\begin{math}");
			concat.SaveAsRegularExp(writer, false);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", которое является конкатенацией выражений и для которых построены грамматики", true);
			writer.WriteLine(@"\begin{math}");
			grammar1.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@",", true);
			writer.WriteLine(@"\begin{math}");
			grammar2.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine("соответственно, строим грамматику ", true);
			
			grammar.Number = Number;
			grammar.IsLeft = isLeft;
			grammar.TargetSymbol = new Unterminal();
			grammar.TargetSymbol.Number = Number;
			
			List<Rule> onlyTerms,others;
			
			if (isLeft)
			{
				grammar2.SplitRules(out onlyTerms, out others);
				
				grammar.Rules.AddRange(others);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = rule.DeepClone(false);
					
					foreach (Chain c in r.Chains)
						c.Symbols.Insert(0,grammar1.TargetSymbol);
					
					grammar.Rules.Add(r);
				}
			}
			else
			{
				grammar1.SplitRules(out onlyTerms, out others);
				
				grammar.Rules.AddRange(others);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = rule.DeepClone(false);
					
					foreach (Chain c in r.Chains)
						c.Symbols.Add(grammar2.TargetSymbol);
					
					grammar.Rules.Add(r);
				}
			}
			
			writer.WriteLine(@"\begin{math}");
			grammar.SaveCortege(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@", где", true);
			writer.WriteLine(@"\begin{math}");
			grammar.SaveN(writer);
			writer.WriteLine(@"=");
			grammar1.SaveN(writer);
			writer.WriteLine();
			writer.WriteLine(@"\bigcup");
			grammar2.SaveN(writer);
			writer.WriteLine(@"=");
			
			writer.WriteLine(@"\{");
			grammar1.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			
			writer.WriteLine(@"\bigcup ");
			
			writer.WriteLine(@"\{");
			grammar2.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			grammar.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}");
			writer.WriteLine("--- множество нетермильнальных символов грамматики", true);
			writer.WriteLine(@"\begin{math}");
			grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(";",true);
			
			writer.WriteLine(@"\begin{math}");
			grammar.SaveP(writer);
			writer.WriteLine(@"=");
			
			if (isLeft)
			{
				grammar1.SaveP(writer);
				writer.WriteLine(@"\bigcup");
				writer.WriteLine(@"\{");
				grammar.SaveRules(writer);
				writer.WriteLine();
				writer.WriteLine(@"\}");
			}
			else
			{
				grammar2.SaveP(writer);
				writer.WriteLine(@"\bigcup ");
				writer.WriteLine(@"\{");
				grammar.SaveRules(writer);
				writer.WriteLine();
				writer.WriteLine(@"\}");				
			}
			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			if (isLeft)
				grammar.Rules.AddRange(grammar1.Rules);
			else
				grammar.Rules.AddRange(grammar2.Rules);
			grammar.SaveRules(writer);				
			writer.WriteLine(@"\}");							
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@"--- множество правил вывода для данной грамматики;",true);
			
			writer.WriteLine(@"\begin{math}");
			grammar.TargetSymbol.Save(writer,isLeft);		
			writer.WriteLine(@"\equiv");
			grammar.TargetSymbol = isLeft ? grammar2.TargetSymbol : grammar1.TargetSymbol;	
			grammar.TargetSymbol.Save(writer,isLeft);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}.");
			return grammar;
		}
		
		public override int GenerateGrammar(Writer writer, bool isLeft, int LastUseNumber)
		{
			if (EntityCollection.Count == 0)
			{
				// TODO: сделать обработку на всякий случай
			}
			else if (EntityCollection.Count == 1)
			{
				// TODO: сделать обработку на всякий случай
			}
			else
			{
				Grammar = EntityCollection[0].Grammar;				
				
				for (int i = 1; i < EntityCollection.Count; i++)	
				{
					int Number = NumLabel.Value - EntityCollection.Count + 1 + i;	
					
					Grammar concatGrammar = EntityCollection[i].Grammar;
					
					Concat concat = new Concat();
					
					for (int j = 0; j <= i; j++)
						concat.EntityCollection.Add(EntityCollection[j]);
					
					Grammar = MergeGrammars(Grammar, concatGrammar, concat, Number, writer, isLeft);
				}				
			}
			
			return LastUseNumber;
		}
    }
}
