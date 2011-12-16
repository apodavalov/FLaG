using System;
using System.Collections.Generic;

namespace FLaG.Data.Equation
{
	class Alter : Expression
	{
		public HashSet<Expression> Expressions
		{
			get;
			private set;
		}
		
		public Alter()
		{
			Expressions = new HashSet<Expression>();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Alter))
				return false;
			
			Alter o = (Alter)obj;
			
			return Expressions.SetEquals(o.Expressions);
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
			Alter alter = new Alter();
			foreach (Expression e in Expressions)
				alter.Expressions.Add(e.DeepClone());
			
			return alter;
		}

		public override Expression Optimize ()
		{
			// TODO: оптимизировать
			return this;
		}
	}
}

