using System;
using System.Collections.Generic;

namespace FLaG.Data.Equation
{
	class Concat : Expression
	{
		public List<Expression> Expressions
		{
			get;
			private set;
		}
		
		public Concat()
		{
			Expressions = new List<Expression>();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Concat))
				return false;
			
			Concat o = (Concat)obj;
			
			if (o.Expressions.Count != Expressions.Count)
				return false;
			
			int count = Expressions.Count;
			
			for (int i = 0; i < count; i++)
				if (!Expressions[i].Equals(o.Expressions[i]))
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			
			foreach (Expression e in Expressions)
				hashCode ^= e.GetHashCode();
			
			return hashCode;
		}

		public override Expression DeepClone ()
		{
			Concat concat = new Concat();
			
			foreach (Expression e in Expressions)
				concat.Expressions.Add(e.DeepClone());
			
			return concat;
		}

		public override void Optimize ()
		{
			// TODO: оптимизировать
		}
	}
}
