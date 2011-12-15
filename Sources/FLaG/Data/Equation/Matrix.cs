using System;
using FLaG.Output;
using Gram=FLaG.Data.Grammars;

namespace FLaG.Data.Equation
{
	class Matrix
	{
		public bool IsLeft
		{
			get;
			set;
		}
		
		public void Save(Writer writer)
		{
			throw new NotImplementedException();
		}
		
		public void Solve(Writer writer)
		{
			throw new NotImplementedException();
		}
		
		public Matrix (Gram.Grammar g)
		{
			throw new NotImplementedException();
		}
	}
}
