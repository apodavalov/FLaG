﻿using System;
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
		
		public override Symbol[] CollectAlphabet()
		{
			List<Symbol> symbols = new List<Symbol>();
			
			foreach (Entity e in EntityCollection)
			{
				Symbol[] smbs = e.CollectAlphabet();
				foreach (Symbol s in smbs)
				{
					int index = symbols.BinarySearch(s);
					
					if (index < 0)
						symbols.Insert(~index,s);
				}
			}
			
			return symbols.ToArray();
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
		
		private void SaveAsRegularExpWithUnderbraces(Writer writer)
		{
			if (EntityCollection.Count > 1)			
				writer.Write(@"\left(");					
			
			if (EntityCollection.Count < 2)
			{
				writer.Write(@"{\underbrace");			
				writer.Write("{");
			}
			else
				for (int i = 1; i < EntityCollection.Count; i++)
				{
					writer.Write(@"{\underbrace");			
					writer.Write("{");
				}

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");

                EntityCollection[i].SaveAsRegularExp(writer,true);
				
				if (i > 0)
				{
					writer.Write("}");
					writer.Write(@"_\text{");			
					writer.Write(NumLabel - (EntityCollection.Count - 1) + i);
					writer.Write("}}");
				}
            }                   
			
			if (EntityCollection.Count < 2)
			{
					writer.Write("}");
					writer.Write(@"_\text{");			
					writer.Write(NumLabel);
					writer.Write("}}");
			}
			
			if (EntityCollection.Count > 1)
				writer.Write(@"\right)");
		}
		
		private void SaveAsRegularExpWithoutUnderbraces(Writer writer)
		{
			writer.Write("{");
			
			if (EntityCollection.Count > 1)			
				writer.Write(@"\left(");

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@" \cdot ");

                EntityCollection[i].SaveAsRegularExp(writer,false);
            }                      
			
			if (EntityCollection.Count > 1)
				writer.Write(@"\right)");
			
			writer.Write("}");
		}
	
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				SaveAsRegularExpWithUnderbraces(writer);			
			else
				SaveAsRegularExpWithoutUnderbraces(writer);	
		}

		public override int MarkDeepest(int val, List<Entity> list)
		{
			if (NumLabel != null)
				return val;
			
			int oldval = val;
			
			foreach (Entity e in EntityCollection)
				val = e.MarkDeepest(val, list);
			
			if (oldval == val)
			{
				list.Add(this);
				if (EntityCollection.Count < 3)
					val++;
				else
					val+=EntityCollection.Count - 1;
				NumLabel = val-1;
			}
			
			return val;
		}
		
		public override void GenerateGrammar(Writer writer, bool isLeft)
		{
			
		}
    }
}
