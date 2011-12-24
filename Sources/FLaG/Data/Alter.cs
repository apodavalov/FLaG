using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

namespace FLaG.Data
{
	class Alter : Entity
	{
		public Alter() 
            : base()
        {
            EntityCollection = new List<Entity>();
        }
		
		public Alter(XmlReader reader, List<Variable> variableCollection) 
            : this()
        {
            while (!reader.IsStartElement("alter")) reader.Read();

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();

            while (reader.IsStartElement())
                EntityCollection.Add(Entity.Load(reader, variableCollection));

            if (!isEmpty)
                reader.ReadEndElement();
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
			Alter a = new Alter();
			
			foreach (Entity e in EntityCollection)
				a.EntityCollection.Add(e.DeepClone());
			
			a.NumLabel = NumLabel;
			
			return a;
		}

        public List<Entity> EntityCollection
        {
            get;
            private set;
        }

        public override void Save(Writer writer)
        {
			writer.Write("{");

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@",");

                EntityCollection[i].Save(writer);
            }                      
			
			writer.Write("}");
        }

        public override Entity ToRegularSet()
        {
            Alter a = new Alter();

            foreach (Entity e in EntityCollection)
                a.EntityCollection.Add(e.ToRegularSet());

            return a;
        }

		public override Entity ToRegularExp()
		{
			Alter a = new Alter();

            foreach (Entity e in EntityCollection)
                a.EntityCollection.Add(e.ToRegularExp());

            return a;
		}
		
		private void SaveAsRegularExpWithoutUnderbraces(Writer writer)
		{
			writer.Write("{");
			
			for (int i = 0; i < EntityCollection.Count; i++)
			{
				if (i != 0)
					writer.Write('+');
                EntityCollection[i].SaveAsRegularExp(writer, false);
			}
			
			writer.Write("}");
		}
		
		private void SaveAsRegularExpWithUnderbraces(Writer writer)
		{
			writer.Write("{");
			
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
					writer.Write(@"+");

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
		
		private Grammar MergeGrammars(Grammar grammar1, Grammar grammar2, Alter alter, int Number, Writer writer, bool isLeft)
		{
			Grammar grammar = new Grammar();
			
			writer.WriteLine(@"\item");
			writer.WriteLine("Для выражения вида" , true);
			writer.WriteLine(@"\begin{math}");
			alter.SaveAsRegularExp(writer, false);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", которое является объединением выражений с грамматиками", true);
			writer.WriteLine(@"\begin{math}");
			grammar1.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@",", true);
			writer.WriteLine(@"\begin{math}");
			grammar2.SaveG(writer);			
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine("соответственно, построим грамматику ", true);
			
			grammar.Number = Number;
			grammar.IsLeft = isLeft;
			grammar.TargetSymbol = Unterminal.GetInstance(Number);
			
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
			writer.WriteLine(@"\cup");			
			grammar2.SaveN(writer);
			writer.WriteLine();
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");  
			grammar.TargetSymbol.Save(writer,isLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			grammar1.SaveUnterminals(writer);
			writer.WriteLine(@"\}");			
			writer.WriteLine(@"\cup ");			
			writer.WriteLine(@"\{");
			grammar2.SaveUnterminals(writer);
			writer.WriteLine(@"\}");			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			Grammar.SaveBothUnterminals(writer,isLeft, grammar1.Unterminals,grammar2.Unterminals,new Unterminal[] {grammar.TargetSymbol});
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
			grammar1.SaveP(writer);
			writer.WriteLine(@"\cup");
			grammar2.SaveP(writer);
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");
			
			// Rule
			Rule r = new Rule();
			r.Prerequisite = grammar.TargetSymbol;			
			Chain c = new Chain();
			c.Symbols.Add(grammar1.TargetSymbol);			
			Grammar.AddChain(r,c);
			c = new Chain();
			c.Symbols.Add(grammar2.TargetSymbol);			
			Grammar.AddChain(r,c);
			r.Save(writer,isLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			grammar1.SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");
			grammar2.SaveRules(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");
			r.Save(writer,isLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");			
			writer.WriteLine(@"\{");
			foreach (Rule rule in grammar1.Rules)
				Grammar.AddRule(grammar.Rules,rule);
			foreach (Rule rule in grammar2.Rules)
				Grammar.AddRule(grammar.Rules,rule);
			Grammar.AddRule(grammar.Rules,r);
			grammar.SaveRules(writer);				
			writer.WriteLine(@"\}");							
			writer.WriteLine(@"\end{math}");
			
			writer.WriteLine(@"--- множество правил вывода для данной грамматики;",true);
			
			writer.WriteLine(@"\begin{math}");
			grammar.TargetSymbol.Save(writer,isLeft);		
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики",true);
			writer.WriteLine(@"\begin{math}");
			grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}.");
			return grammar;
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum)
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
					
					Grammar alterGrammar = EntityCollection[i].Grammar;
					
					Alter alter = new Alter();
					
					for (int j = 0; j <= i; j++)
						alter.EntityCollection.Add(EntityCollection[j]);
					
					Grammar = MergeGrammars(Grammar, alterGrammar, alter, Number, writer, isLeft);
				}				
			}		
		}		
	}
}

