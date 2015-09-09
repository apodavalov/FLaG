using FLaG.Data.Grammars;
using FLaG.Output;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FLaG.Data
{
    class Symbol : Entity, IComparable<Symbol>
    {
        public Symbol()
            : base()
        {

        }
		
		public override Symbol[] CollectAlphabet()
		{
			return new Symbol[1] {this};
		}
		
		public override Entity DeepClone()
		{
			Symbol s = new Symbol();
			s.Value = Value;
			s.NumLabel = NumLabel;
			
			return s;
		}

        public Symbol(XmlReader reader, List<Variable> variableCollection)
            : this()
        {
            while (!reader.IsStartElement("symbol")) reader.Read();

            reader.ReadStartElement();

            Value = reader.ReadContentAsString()[0];

            reader.ReadEndElement();
        }

        public char Value
        {
            get;
            set;
        }

        public override void Save(Writer writer)
        {
			writer.Write(Value);
        }
		
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace{");
			
			writer.Write(Value);
			
			if (full)
			{
				writer.Write(@"}_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}
		}
		
        public override Entity ToRegularSet()
        {
            return this;
        }
		
		public override Entity ToRegularExp()
		{
			return this;
		}

		public override int MarkDeepest(int val, List<Entity> list)
		{
			if (NumLabel != null)
				return val;
			
			list.Add(this);
			NumLabel = val;
			
			return val+1;
		}

		public int CompareTo(Symbol other)
		{
			return Value.CompareTo(other.Value);
		}

		public override void GenerateAutomaton(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalAutomatonsNum)
		{
			Automaton = new FLaG.Data.Automaton.NAutomaton();
					
			Automaton.Number = NumLabel.Value;			
			Automaton.IsLeft = isLeft;

			FLaG.Data.Automaton.NStatus oldStatus = new FLaG.Data.Automaton.NStatus('S',1);
			FLaG.Data.Automaton.NStatus newStatus = new FLaG.Data.Automaton.NStatus('S',2);
			FLaG.Data.Automaton.Symbol symbol = new FLaG.Data.Automaton.Symbol(Value);

			FLaG.Data.Automaton.NTransitionFunc func = new FLaG.Data.Automaton.NTransitionFunc(oldStatus,symbol,newStatus);

			Automaton.AddFunc(func);

			Automaton.AddEndStatus(newStatus);
			Automaton.InitialStatus = oldStatus;
						
			writer.Write(@"\item ");
			writer.Write("Для выражения вида " ,true);
			writer.Write(@"\emph{");
			writer.Write(Value.ToString(),true);
			writer.Write(@"} ");
			writer.WriteLine(" построим конечный автомат ", true);
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где",true);			
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveQ(writer);
			writer.WriteLine(@"=");
			Automaton.SaveStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- конечное множество состояний автомата,",true);
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveSigma(writer);
			writer.WriteLine(@"=");
			Automaton.SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- входной алфавит автомата (конечное множество допустимых входных символов),",true);
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveDelta(writer);
			writer.WriteLine(@"=");
			Automaton.SaveFunctions(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество функций переходов,",true);
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveQ0(writer);
			writer.WriteLine(@"=");
			Automaton.InitialStatus.Save(writer,isLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- начальное состояние автомата,",true);
			writer.WriteLine(@"\begin{math}");
			Automaton.SaveS(writer);
			writer.WriteLine(@"=");
			Automaton.SaveEndStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- заключительное состояние (конечное множество заключительных состояний).",true);
			writer.WriteLine();
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft, ref int LastUseNumber, ref int AddionalGrammarsNum)
		{
			Grammar = new Grammar();
			
			Grammar.Number = NumLabel.Value;			
			Grammar.IsLeft = isLeft;
			
			Rule rule = new Rule();
			
			Unterminal unterminal = Unterminal.GetInstance(Grammar.Number);			
			
			Grammar.TargetSymbol = unterminal;
			
			rule.Prerequisite = unterminal;
			
			Terminal terminal = new Terminal();
			terminal.Value = Value;
			
			Chain chain = new Chain();
			chain.Symbols.Add(terminal);
			
			Grammar.AddChain(rule,chain);
			
			Grammar.AddRule(Grammar.Rules,rule);
			
			writer.Write(@"\item ");
			writer.Write("Для выражения вида " ,true);
			writer.Write(@"\emph{");
			writer.Write(Value.ToString(),true);
			writer.Write(@"} ");
			writer.WriteLine(" построим грамматику ", true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveCortege(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@", где");
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveN(writer);
			writer.Write("=");
			writer.Write(@"\left\{");
			Grammar.SaveUnterminals(writer);			
			writer.WriteLine(@"\right\}");			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество нетерминальных символов грамматики ",true);
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveG(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(",");
			writer.WriteLine(@"\begin{math}");
			Grammar.SaveP(writer);
			writer.Write("=");
			writer.Write(@"\left\{");
			Grammar.SaveRules(writer);			
			writer.WriteLine(@"\right\}");			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество правил вывода для данной грамматики,",true);
			writer.WriteLine(@"\begin{math}");
			Grammar.TargetSymbol.Save(writer,Grammar.IsLeft);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики.",true);
		}
    }
}
