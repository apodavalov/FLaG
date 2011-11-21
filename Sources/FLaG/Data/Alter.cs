using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FLaG.Output;
using FLaG.Data.Grammars;

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
		
		private void SaveAsRegularExpWithoutUnderbraces(Writer writer)
		{
			writer.Write("{");
			
			for (int i = 0; i < EntityCollection.Count; i++)
			{
				if (i != 0)
					writer.Write('+');
                EntityCollection[i].SaveAsRegularExp(writer, false);
			}
			
			writer.Write("}");
		}
		
		private void SaveAsRegularExpWithUnderbraces(Writer writer)
		{
			writer.Write("{");
			
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
					writer.Write(@"+");

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

		
		public override int GenerateGrammar (Writer writer, bool isLeft, int LastUseNumber)
		{
			Grammar = new Grammar();
			Grammar.IsLeft = isLeft;
			Grammar.Number = NumLabel.Value;
			Grammar.TargetSymbol = new Unterminal();
			Grammar.TargetSymbol.Number = Grammar.Number;
			
			return LastUseNumber;
		}		
	}
}

