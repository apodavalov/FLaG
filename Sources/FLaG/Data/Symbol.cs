using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    class Symbol : Entity
    {
        public Symbol()
            : base()
        {

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
		
		public override void SaveAsRegularExp(Writer writer)
		{
			writer.Write(Value);
		}

        public override Entity ToRegularSet()
        {
            return this;
        }
		
		public override Entity ToRegularExp()
		{
			return this;
		}

		public override int MarkDeepest(int val)
		{
			if (NumLabel != null)
				return val;
			
			NumLabel = val;
			
			return val+1;
		}
    }
}
