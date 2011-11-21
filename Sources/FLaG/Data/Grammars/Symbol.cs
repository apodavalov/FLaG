using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	abstract class Symbol
	{
		public abstract void Save(Writer writer, bool isLeft);
		
		public abstract Symbol DeepClone();
	}
}

