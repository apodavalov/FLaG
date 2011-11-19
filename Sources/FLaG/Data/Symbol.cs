using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

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
		
		public override void GenerateGrammar(Writer writer, bool isLeft)
		{
			Grammar = new Grammar();
			
			Grammar.Number = NumLabel.Value;			
			Grammar.IsLeft = isLeft;
			
			Rule rule = new Rule();
			
			Unterminal unterminal = new Unterminal();
			unterminal.Number = Grammar.Number;			
			
			rule.Prerequisite = unterminal;
			
			Terminal terminal = new Terminal();
			terminal.Value = Value;
			
			Chain chain = new Chain();
			chain.Symbols.Add(terminal);
			
			rule.Chains.Add(chain);
			
			Grammar.Rules.Add(rule);
			
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
			Grammar.SaveS(writer);
			writer.WriteLine();
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- целевой символ грамматики.",true);
		}
    }
}
