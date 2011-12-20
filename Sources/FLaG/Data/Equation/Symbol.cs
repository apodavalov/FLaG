using System;
using FLaG.Output;

namespace FLaG.Data.Equation
{
	class Symbol : Expression
	{
		public char Value
		{
			get;
			private set;
		}
		
		public Symbol(char val)
		{
			Value = val;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Symbol))
				return false;
			
			Symbol o = (Symbol)obj;
			
			return o.Value.Equals(Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override Expression DeepClone ()
		{
			return this;
		}

		public override Expression Optimize ()
		{
			return this;
		}
		
		public override void Save(Writer writer)
		{
			writer.Write(Value + "",true);
		}
		
		public override bool IsLetEmpty()
		{
			return false;
		}
		
		public override void LetBeEmpty()
		{
			// ничего не можем поделать
		}
	}
}

