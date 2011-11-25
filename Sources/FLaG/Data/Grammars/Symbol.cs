using System.Collections.Generic;
using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	abstract class Symbol : IComparable<Symbol>
	{
		public abstract void Save(Writer writer, bool isLeft);
		
		public abstract int CompareTo(Symbol other);

		public override bool Equals(object other)
		{
			return CompareTo((Symbol)other) == 0;			
		}

		public override abstract int GetHashCode();
	}
}

