using System;
using FLaG.Output;
using Gram=FLaG.Data.Grammars;

namespace FLaG.Data.Equation
{
	class Matrix
	{
		private Expression[][] Mx
		{
			get;
			set;
		}
		
		private Gram.Unterminal[] Unterminals
		{
			get;
			set;
		}
		
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
			Unterminals = g.DeepUnterminals;
			
			Mx = new Expression[Unterminals.Length][];
			
			for (int i = 0; i < Unterminals.Length; i++)
				Mx[i] = new Expression[Unterminals.Length + 1];
			
			IsLeft = g.IsLeft;
			
			
		}
	}
}
