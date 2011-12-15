using System;
using System.Collections.Generic;

namespace FLaG
{
	abstract class Expression
	{
		public Expression()
		{
			
		}
		
		public abstract Expression DeepClone();

		public override abstract bool Equals(object obj);
		
		public override abstract int GetHashCode();
	}
}

