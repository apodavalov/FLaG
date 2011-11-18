using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;

namespace FLaG.Data
{
	class Alter : Entity
	{
		public Alter() 
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
			Alter a = new Alter();
			
			foreach (Entity e in EntityCollection)
				a.EntityCollection.Add(e.DeepClone());
			
			a.NumLabel = NumLabel;
			
			return a;
		}

        public List<Entity> EntityCollection
        {
            get;
            private set;
        }

        public override void Save(Writer writer)
        {
			writer.Write("{");

            for (int i = 0; i < EntityCollection.Count; i++)
            {
                if (i != 0)
					writer.Write(@"+");

                EntityCollection[i].Save(writer);
            }                      
			
			writer.Write("}");
        }

        public override Entity ToRegularSet()
        {
            Alter a = new Alter();

            foreach (Entity e in EntityCollection)
                a.EntityCollection.Add(e.ToRegularSet());

            return a;
        }

		public override Entity ToRegularExp()
		{
			Alter a = new Alter();

            foreach (Entity e in EntityCollection)
                a.EntityCollection.Add(e.ToRegularExp());

            return a;
		}
	
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			for (int i = 0; i < EntityCollection.Count; i++)
			{
				if (i != 0)
					writer.Write('+');
                EntityCollection[i].SaveAsRegularExp(writer, full);
			}
			
			writer.Write("}");
			
			if (full)
			{
				writer.Write(@"_\text{");
				writer.Write(NumLabel);
				writer.Write(@"}}");
			}			
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
				NumLabel = val;
				val++;
			}
			
			return val;
		}
	}
}

