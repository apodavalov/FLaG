using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Unterminal : Symbol
	{
		public int Number
		{
			get;
			set;
		}
		
		public override Symbol DeepClone()
		{
			Unterminal unterminal = new Unterminal();
			unterminal.Number = Number;
			
			return unterminal;
		}

		public override void Save(Writer writer, bool isLeft)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write("S", true);
			writer.Write("_");
			writer.Write(Number);
			writer.Write(@"}");
			if (isLeft)
				writer.Write("'");
			else
				writer.Write("''");
			writer.Write(@"}");
		}
	}
}

