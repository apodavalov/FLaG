using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
 
namespace FLaG.Data
{
    class Variable : Quantity, IComparable<Variable>
    {
        public Variable() 
            : base()
        {

        }

        public Variable(XmlReader reader) 
            : this()
        {
            while (!reader.IsStartElement("variable")) reader.Read();

            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                reader.MoveToFirstAttribute();

                do
                {
                    switch (reader.Name)
                    {
                        case "name":
                            Name = reader.Value[0];
                            break;
                        case "sign":
                            if (reader.Value == ">")
                                Sign = SignEnum.MoreThanZero;
                            else
                                Sign = SignEnum.MoreOrEqualZero;
                            break;
                        case "num":
                            Num = int.Parse(reader.Value);
                            break;
                    }
                } while (reader.MoveToNextAttribute());
            }

            reader.ReadStartElement();

            if (!isEmpty)
                reader.ReadEndElement();
        }

        public char Name
        {
            get;
            set;
        }

        public int Num
        {
            get;
            set;
        }

        public SignEnum Sign
        {
            get;
            set;
        }

        public int CompareTo(Variable other)
        {
            if (Name < other.Name)
                return -1;
            else if (Name > other.Name)
                return 1;
            else
                return 0;
        }

        public override void Save(Writer writer)
        {
//            writer.WriteStartElement("mo", Writer.mathmlNS);
//            writer.WriteCharEntity(Name);
//            writer.WriteEndElement();
        }
    }
}
