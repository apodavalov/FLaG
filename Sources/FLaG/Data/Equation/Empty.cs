using System;
using FLaG.Output;

namespace FLaG
{
	class Empty : Expression
	{
		public Empty()
		{
			
		}

		public override bool Equals (object obj)
		{
			return obj is Empty;
		}

		public override int GetHashCode ()
		{
			return 0;
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
			writer.Write(@"\varepsilon ");
		}
	}
}

