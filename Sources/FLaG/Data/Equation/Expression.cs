using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Equation
{
	abstract class Expression
	{
		public Expression()
		{
			
		}
		
		public abstract void Save(Writer writer);
		
		public abstract bool IsLetEmpty();
		
		public abstract void LetBeEmpty();
		
		public abstract Expression Optimize();
		
		public abstract Expression DeepClone();

		public override abstract bool Equals(object obj);
		
		public override abstract int GetHashCode();
	}
}

