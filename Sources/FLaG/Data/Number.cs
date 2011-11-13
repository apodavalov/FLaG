using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    class Number : Quantity
    {
        public Number()
            : base()
        {

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
    }
}
