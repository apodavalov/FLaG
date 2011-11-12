using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    class Degree : Entity
    {
        public Degree() 
            : base()
        {

        }

        public Degree(XmlReader reader, List<Variable> variableCollection) 
            : this()
        {
            while (!reader.IsStartElement("degree")) reader.Read();

            reader.ReadStartElement(); // degree

            while (reader.IsStartElement())
            {
                string name = reader.Name;
                
                reader.ReadStartElement(); // base or exp

                if (reader.IsStartElement())
                {
                    if (name == "base")
                        Base = Entity.Load(reader, variableCollection);
                    else if (name == "exp")
                        Exp = Quantity.Load(reader, variableCollection);
                }

                reader.ReadEndElement(); // base or exp
            }

            reader.ReadEndElement(); // degree
        }

        public Entity Base
        {
            get;
            set;
        }

        public Quantity Exp
        {
            get;
            set;
        }

        public override Entity ToRegularSet()
        {
            if (Base is Symbol && Exp is Number)
            {
                Concat c = new Concat();
                Symbol s = (Symbol)Base;
                Number n = (Number)Exp;

                for (int i = 0; i < n.Value; i++)
                    c.EntityCollection.Add(s);

                return c;
            }
            else
            {
                Degree d = new Degree();
                d.Base = Base.ToRegularSet();
                d.Exp = Exp;
                return d;
            }
        }

        public override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("msup", Writer.mathmlNS);

            writer.WriteStartElement("mrow", Writer.mathmlNS);

            writer.WriteStartElement("mfenced", Writer.mathmlNS);
            writer.WriteAttributeString("open", "{");
            writer.WriteAttributeString("close", "}");

            writer.WriteStartElement("mrow",Writer.mathmlNS);

            Base.Save(writer);

            writer.WriteEndElement(); // mrow

            writer.WriteEndElement(); // mfenced

            writer.WriteEndElement(); // mrow

            writer.WriteStartElement("mrow", Writer.mathmlNS);
            Exp.Save(writer);
            writer.WriteEndElement(); // mrow

            writer.WriteEndElement();
        }
    }
}
