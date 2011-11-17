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

                EntityCollection[i].Save(writer);
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
	
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			if (EntityCollection.Count > 1)			
				writer.Write(@"\left(");

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");

                EntityCollection[i].SaveAsRegularExp(writer,full);
            }                      
			
			if (EntityCollection.Count > 1)
				writer.Write(@"\right)");
			
			writer.Write("}");
			
			if (full)
			{
				writer.Write(@"_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}
		}

		public override int MarkDeepest(int val)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			foreach (Entity e in EntityCollection)
				val = e.MarkDeepest(val);
			
			if (oldval == val)
			{
				NumLabel = val;
				val++;
			}
			
			return val;
		}
    }
}
