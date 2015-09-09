using FLaG.Data.Grammars;
using FLaG.Output;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\left\{");	
				
                EntityCollection[i].Save(writer);
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\right\}");
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
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\left(");	

                EntityCollection[i].SaveAsRegularExp(writer,true);
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\right)");
				
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
		}
		
		private void SaveAsRegularExpWithoutUnderbraces(Writer writer)
		{
			writer.Write("{");
			
            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\left(");	

                EntityCollection[i].SaveAsRegularExp(writer,false);
				
				if (EntityCollection[i] is Alter)
					writer.Write(@"\right)");
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
		
		private Grammar MergeGrammars(Grammar grammar1, Grammar grammar2, Concat concat, int Number, Writer writer, bool isLeft)
		{
			Grammar grammar = new Grammar();
			
			writer.WriteLine(@"\item");
			writer.WriteLine("Для выражения вида" , true);
			writer.WriteLine(@"\begin{math}");
			concat.SaveAsRegularExp(writer, false);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", являющегося конкатенацией выражений с построенными грамматиками ", true);
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
			grammar.TargetSymbol = Unterminal.GetInstance(Number);
			
			List<Rule> onlyTerms,others;
			
			if (isLeft)
			{
				grammar2.SplitRules(out onlyTerms, out others);
				
				foreach (Rule rule in others)
					Grammar.AddRule(grammar.Rules,rule);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = rule.DeepClone();
					
					foreach (Chain c in r.Chains)
						c.Symbols.Insert(0,grammar1.TargetSymbol);
					
					Grammar.AddRule(grammar.Rules,r);
				}
			}
			else
			{
				grammar1.SplitRules(out onlyTerms, out others);
				
				foreach (Rule rule in others)
					Grammar.AddRule(grammar.Rules,rule);
				
				foreach (Rule rule in onlyTerms)
				{
					Rule r = rule.DeepClone();
					
					foreach (Chain c in r.Chains)
						c.Symbols.Add(grammar2.TargetSymbol);
					
					Grammar.AddRule(grammar.Rules,r);
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
			writer.WriteLine(@"\cup");
			grammar2.SaveN(writer);
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
			Grammar.SaveBothUnterminals(writer,isLeft,grammar1.Unterminals,grammar2.Unterminals);
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
				writer.WriteLine(@"\cup");
				writer.WriteLine(@"\{");
				grammar.SaveRules(writer);
				writer.WriteLine();
				writer.WriteLine(@"\}");
			}
			else
			{
				grammar2.SaveP(writer);
				writer.WriteLine(@"\cup ");
				writer.WriteLine(@"\{");
				grammar.SaveRules(writer);
				writer.WriteLine();
				writer.WriteLine(@"\}");				
			}
			
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			if (isLeft)
			{
				foreach (Rule rule in grammar1.Rules)
					Grammar.AddRule(grammar.Rules, rule);
			}
			else
			{
				foreach (Rule rule in grammar2.Rules)
					Grammar.AddRule(grammar.Rules, rule);
			}
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

		private FLaG.Data.Automaton.NAutomaton MergeAutomatons (FLaG.Data.Automaton.NAutomaton automaton1, FLaG.Data.Automaton.NAutomaton automaton2, Concat concat, int Number, Writer writer, bool isLeft)
		{
			FLaG.Data.Automaton.NAutomaton automaton = new FLaG.Data.Automaton.NAutomaton ();
			
			writer.WriteLine (@"\item");
			writer.WriteLine ("Для выражения вида", true);
			writer.WriteLine (@"\begin{math}");
			concat.SaveAsRegularExp (writer, false);
			writer.WriteLine ();
			writer.WriteLine (@"\end{math}");
			writer.WriteLine (@", являющегося конкатенацией выражений с построенными конечными автоматами ", true);
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

			foreach (FLaG.Data.Automaton.NStatus s in automaton2.EndStatuses)
				automaton.AddEndStatus (s);

			bool flag = automaton2.EndStatuses.BinarySearch (automaton2.InitialStatus) >= 0;

			if (flag) {
				foreach (FLaG.Data.Automaton.NStatus s in automaton1.EndStatuses)
					automaton.AddEndStatus (s);
			}

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton1.Functions) {
			//	if (automaton1.EndStatuses.BinarySearch (f.OldStatus) < 0)
					automaton.AddFunc (f);
			}

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton2.Functions) 
			{
				if (f.OldStatus.CompareTo(automaton2.InitialStatus) == 0)
				{
					foreach (FLaG.Data.Automaton.NStatus s in automaton1.EndStatuses)
						automaton.AddFunc(new FLaG.Data.Automaton.NTransitionFunc(s,f.Symbol,f.NewStatus));
				}
			}

			foreach (FLaG.Data.Automaton.NTransitionFunc f in automaton2.Functions) 
				automaton.AddFunc (f);

			automaton.InitialStatus = automaton1.InitialStatus;

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
			writer.WriteLine (@"=");
			automaton1.SaveStatuses (writer);
			writer.WriteLine (@"\cup");
			automaton2.SaveStatuses (writer);
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
			writer.WriteLine (@"Так как начальное состояние автомата ");
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
			if (flag) 
			{
				automaton1.SaveS(writer);
				writer.WriteLine (@"\cup");
				automaton2.SaveS(writer);
				writer.WriteLine (@"=");
				automaton1.SaveEndStatuses(writer);
				writer.WriteLine (@"\cup");
				automaton2.SaveEndStatuses(writer);
			}
			else 
			{
				automaton2.SaveS(writer);			
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
					
					FLaG.Data.Automaton.NAutomaton concatAutomaton = EntityCollection[i].Automaton;
					
					Concat concat = new Concat();
					
					for (int j = 0; j <= i; j++)
						concat.EntityCollection.Add(EntityCollection[j]);
					
					Automaton = MergeAutomatons(Automaton, concatAutomaton, concat, Number, writer, isLeft);
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
					
					Grammar concatGrammar = EntityCollection[i].Grammar;
					
					Concat concat = new Concat();
					
					for (int j = 0; j <= i; j++)
						concat.EntityCollection.Add(EntityCollection[j]);
					
					Grammar = MergeGrammars(Grammar, concatGrammar, concat, Number, writer, isLeft);
				}				
			}
		}
    }
}
