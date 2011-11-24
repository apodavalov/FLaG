using System;
using FLaG.Output;

namespace FLaG.Data.Grammars
{
	class Terminal : Symbol, IComparable<Terminal>
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

		public override int CompareTo(Symbol other)
		{
			if (other is Terminal)
				return CompareTo((Terminal)other);
			else if (other is Unterminal)
				return -1;
			
			throw new NotSupportedException();
		}
		
		public int CompareTo(Terminal other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}

