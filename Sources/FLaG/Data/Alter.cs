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

		private FLaG.Data.Automaton.NAutomaton MergeAutomatons (FLaG.Data.Automaton.NAutomaton automaton1, FLaG.Data.Automaton.NAutomaton automaton2, Alter alter, int Number, Writer writer, bool isLeft)
		{
			FLaG.Data.Automaton.NAutomaton automaton = new FLaG.Data.Automaton.NAutomaton ();
			
			writer.WriteLine (@"\item");
			writer.WriteLine ("Для выражения вида", true);
			writer.WriteLine (@"\begin{math}");
			alter.SaveAsRegularExp (writer, false);
			writer.WriteLine ();
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@", являющегося объединением выражений с построенными конечными автоматами ", true);
			writer.WriteLine (@"\begin{math}");
			automaton1.SaveCortege (writer);			
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@",", true);
			writer.WriteLine (@"\begin{math}");
			automaton2.SaveCortege (writer);			
			writer.WriteLine (@"\end{math}");
			writer.WriteLine ("соответственно, строим конечный автомат ", true);
			
			automaton.Number = Number;
			automaton.IsLeft = isLeft;

			writer.WriteLine (@"\begin{math}");
			automaton.SaveCortege (writer);			
			writer.WriteLine (@"\end{math}.");

			automaton2.MakeNonIntersectStatusesWith (writer, automaton1);

			int num = 1;

			foreach (FLaG.Data.Automaton.NStatus s in automaton1.Statuses)
				if (s.Number > num)
					num = s.Number.Value;

			foreach (FLaG.Data.Automaton.NStatus s in automaton2.Statuses)
				if (s.Number > num)
					num = s.Number.Value;

			num++;

			automaton.InitialStatus = new FLaG.Data.Automaton.NStatus('S',num);

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton1.Functions) 
			{
				automaton.AddFunc (f);
			}

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton2.Functions) 
			{
				automaton.AddFunc (f);
			}

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton1.Functions) 
				if (f.OldStatus.CompareTo(automaton1.InitialStatus) == 0)
					automaton.AddFunc(new FLaG.Data.Automaton.NTransitionFunc(automaton.InitialStatus,f.Symbol,f.NewStatus));

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton2.Functions) 
			{
				if (f.OldStatus.CompareTo(automaton2.InitialStatus) == 0)
					automaton.AddFunc(new FLaG.Data.Automaton.NTransitionFunc(automaton.InitialStatus,f.Symbol,f.NewStatus));
			}

			bool flag = automaton1.EndStatuses.BinarySearch(automaton1.InitialStatus) >= 0 ||
				automaton2.EndStatuses.BinarySearch(automaton2.InitialStatus) >= 0;

			if (flag)
				automaton.AddEndStatus(automaton.InitialStatus);

			foreach (FLaG.Data.Automaton.NStatus s in automaton1.EndStatuses)
				automaton.AddEndStatus (s);

			foreach (FLaG.Data.Automaton.NStatus s in automaton2.EndStatuses)
				automaton.AddEndStatus (s);

			writer.WriteLine ("Строим конечный автомат ", true);
			writer.WriteLine (@"\begin{math}");
			automaton.SaveCortege (writer);
			writer.WriteLine (@"\end{math},");
			writer.WriteLine (@"где", true);			
			writer.WriteLine (@"\begin{math}");
			automaton.SaveQ (writer);
			writer.WriteLine (@"=");
			automaton1.SaveQ (writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveQ (writer);
			writer.WriteLine (@"\cup");
			automaton.InitialStatus.Save (writer,isLeft);
			writer.WriteLine (@"=");
			automaton1.SaveStatuses (writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveStatuses (writer);
			writer.WriteLine (@"\cup");
			automaton.InitialStatus.Save (writer,isLeft);
			writer.WriteLine (@"=");
			automaton.SaveStatuses (writer);
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@"--- конечное множество состояний автомата,", true);
			writer.WriteLine (@"\begin{math}");
			automaton.SaveSigma (writer);
			writer.WriteLine (@"=");
			automaton1.SaveSigma (writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveSigma (writer);
			writer.WriteLine (@"=");
			automaton1.SaveAlphabet (writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveAlphabet (writer);
			writer.WriteLine (@"=");
			automaton.SaveAlphabet (writer);
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@"--- входной алфавит автомата (конечное множество допустимых входных символов),", true);
			writer.WriteLine (@"\begin{math}");
			automaton.SaveDelta (writer);
			writer.WriteLine (@"=");
			automaton.SaveFunctions (writer);
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@"--- множество функций переходов,", true);
			writer.WriteLine (@"\begin{math}");
			automaton.SaveQ0 (writer);
			writer.WriteLine (@"=");
			automaton.InitialStatus.Save (writer, isLeft);
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@"--- начальное состояние автомата.", true);
			writer.WriteLine (@"Так как начальное состояние хотя бы одного из автоматов ");
			writer.WriteLine (@"\begin{math}");
			automaton2.SaveCortege(writer);
			writer.WriteLine (@"\end{math}");
			if (flag) 
				writer.WriteLine(@"входит в множество конечных состояних, то ", true);
			else
				writer.WriteLine(@"не входит в множество конечных состояних, то ", true);
			writer.WriteLine (@"\begin{math}");
			automaton.SaveS (writer);
			writer.WriteLine (@"=");
			automaton1.SaveS(writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveS(writer);
			if (flag) 
			{
				writer.WriteLine (@"\cup");
				automaton.InitialStatus.Save(writer,isLeft);
			}
			writer.WriteLine (@"=");
			automaton1.SaveEndStatuses(writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveEndStatuses(writer);
			if (flag) 
			{
				writer.WriteLine (@"\cup");
				automaton.InitialStatus.Save(writer,isLeft);
			}
			writer.WriteLine (@"=");
			automaton.SaveEndStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- заключительное состояние (конечное множество заключительных состояний).",true);
			writer.WriteLine();

			return automaton;
		}

		public override void GenerateAutomaton(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalAutomatonsNum)
		{
			if (EntityCollection.Count == 0)
			{
				// TODO: сделать обработку на всякий случай
			}
			else if (EntityCollection.Count == 1)
			{
				Automaton = EntityCollection[0].Automaton;
			}
			else
			{
				Automaton = EntityCollection[0].Automaton;				
				
				for (int i = 1; i < EntityCollection.Count; i++)	
				{
					int Number = NumLabel.Value - EntityCollection.Count + 1 + i;	
					
					FLaG.Data.Automaton.NAutomaton alterAutomaton = EntityCollection[i].Automaton;
					
					Alter alter = new Alter();
					
					for (int j = 0; j <= i; j++)
						alter.EntityCollection.Add(EntityCollection[j]);
					
					Automaton = MergeAutomatons(Automaton, alterAutomaton, alter, Number, writer, isLeft);
				}				
			}
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum)
		{
			if (EntityCollection.Count == 0)
			{
				// TODO: сделать обработку на всякий случай
			}
			else if (EntityCollection.Count == 1)
			{
				Grammar = EntityCollection[0].Grammar;
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

