using FLaG.Output;
using System.Collections.Generic;
using System.Xml;

namespace FLaG.Data
{
    class Number : Quantity
    {
        public Number()
            : base()
        {

        }
		
		public override Quantity DeepClone()
		{
			Number n = new Number();
			
			n.Value = Value;
			
			return n;
		}

        public Number(XmlReader reader, List<Variable> variableCollection) 
            : this()
        {
            while (!reader.IsStartElement("number")) reader.Read();

            reader.ReadStartElement();

            Value = reader.ReadContentAsInt();

            reader.ReadEndElement();
        }

        public int Value
        {
            get;
            set;
        }

        public override void Save(Writer writer)
        {
			if (Value < 0)
			{
				writer.Write("{");
				writer.Write(Value);
				writer.Write("}");
			}
			else
				writer.Write(Value);
        }
		
		public override void SaveAsRegularExp(Writer writer)
        {
			writer.Write(Value);
        }
    }
}
