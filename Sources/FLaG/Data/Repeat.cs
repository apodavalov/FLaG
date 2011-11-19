using System;
using FLaG.Output;
using System.Collections.Generic;

namespace FLaG.Data
{
	class Repeat : Entity
	{
		public bool AtLeastOne
		{
			get;
			set;
		}
		
		public Entity Entity
		{
			get;
			set;
		}
		
		public override Symbol[] CollectAlphabet()
		{
			return Entity.CollectAlphabet();	
		}
		
		public override Entity DeepClone()
		{
			Repeat r = new Repeat();
			
			r.AtLeastOne = AtLeastOne;
			r.Entity = Entity.DeepClone();
			r.NumLabel = NumLabel;
			
			return r;
		}

		public override void Save(Writer writer)
		{
			throw new NotSupportedException();
		}
		
		public override Entity ToRegularSet()
		{
			throw new NotSupportedException();
		}

		public override Entity ToRegularExp()
		{
			throw new NotSupportedException();
		}
		
		public override void SaveAsRegularExp(Writer writer, bool full)
		{
			if (full)
				writer.Write(@"{\underbrace");
			
			writer.Write("{");
			
			if (Entity is Symbol || Entity is Concat)
				Entity.SaveAsRegularExp(writer,full);
			else
			{
				writer.Write(@"\left(");
				Entity.SaveAsRegularExp(writer,full);
				writer.Write(@"\right)");
			}					
			
			writer.Write("^");
			if (AtLeastOne)
            	writer.Write('+');
			else
				writer.Write(@"\ast ");
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
			
			val = Entity.MarkDeepest(val,list);
			
			if (oldval == val)
			{
				list.Add(this);
				NumLabel = val;
				val++;
			}
			
			return val;
		}
		
		public override void GenerateGrammar(int number, Writer writer, bool isLeft)
		{
			
		}
	}
}

