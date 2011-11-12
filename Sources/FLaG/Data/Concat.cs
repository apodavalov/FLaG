using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
    class Concat : Entity
    {
        public Concat() 
            : base()
        {
            EntityCollection = new List<Entity>();
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

        public override void Save(XmlWriter writer)
        {
            char times;

            if (EntityCollection.All<Entity>(x => x is Symbol))
                // invisible
                times = (char)8290;
            else
                // visible
                times = (char)8901;

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
                {
                    writer.WriteStartElement("mi", Writer.mathmlNS);
                    // mul
                    writer.WriteCharEntity(times);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("mrow", Writer.mathmlNS);
                EntityCollection[i].Save(writer);
                writer.WriteEndElement(); // mrow
            }                      
        }

        public override Entity ToRegularSet()
        {
            Concat c = new Concat();

            foreach (Entity e in EntityCollection)
                c.EntityCollection.Add(e.ToRegularSet());

            return c;
        }
    }
}
