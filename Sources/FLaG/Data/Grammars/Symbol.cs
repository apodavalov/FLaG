using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	abstract class Symbol : IComparable<Symbol>
	{
		public abstract void Save(Writer writer, bool isLeft);
		
		public abstract int CompareTo(Symbol other);
	}
}

