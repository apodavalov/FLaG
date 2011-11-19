using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Terminal : Symbol
	{
		public char Value
		{
			get;
			set;
		}

		public override void Save(Writer writer, bool isLeft)
		{
			writer.Write("{");
			writer.Write(Value.ToString(),true);
			writer.Write("}");
		}
	}
}

