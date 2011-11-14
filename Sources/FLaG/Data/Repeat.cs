using System;
using FLaG.Output;

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
		
		public override void SaveAsRegularExp(Writer writer)
		{
			writer.Write("{");
			
			if (Entity is Symbol || Entity is Concat)
				Entity.SaveAsRegularExp(writer);
			else
			{
				writer.Write(@"\left(");
				Entity.SaveAsRegularExp(writer);
				writer.Write(@"\right)");
			}					
			
			writer.Write("^");
			if (AtLeastOne)
            	writer.Write('+');
			else
				writer.Write(@"\ast ");
			writer.Write("}");
		}
	}
}

