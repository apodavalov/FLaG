using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

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
		
		public override void GenerateGrammar(int number, Writer writer, bool isLeft)
		{
			throw new NotImplementedException();
		}
    }
}
