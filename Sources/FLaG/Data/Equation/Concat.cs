using System;
using System.Collections.Generic;
using FLaG.Output;

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

		public override Expression Optimize ()
		{
			for (int i = 0; i < Expressions.Count; i++)
				Expressions[i] = Expressions[i].Optimize();
			
			bool isAllEmpty = true;
			
			for (int i = 0; i < Expressions.Count; i++)
				if (!(Expressions[i] is Empty))
				{
					isAllEmpty = false;
					break;
				}
			
			if (isAllEmpty)
				return new Empty();
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;
			
				// поднимает конкатенации
				for (int i = 0; i < Expressions.Count; i++)
					if (Expressions[i] is Concat)
					{
						Concat c = (Concat)Expressions[i];
						Expressions.RemoveAt(i);
						for (int j = c.Expressions.Count - 1; j >= 0; j--)
							Expressions.Insert(i,c.Expressions[j]);
						i--;
						i += c.Expressions.Count;
						somethingChanged = true;
					}
			
				// удаляем пустые 
				for (int i = 0; i < Expressions.Count; i++)
					if (Expressions[i] is Empty)
					{
						Expressions.RemoveAt(i--);
						somethingChanged = true;
					}
				
				
				
				
			} while (somethingChanged);
			
			if (Expressions.Count == 0)
				return new Empty();
			else if (Expressions.Count == 1)
				return Expressions[0];
			else return this;
		}
		
		public override void Save(Writer writer)
		{
            foreach (Expression e in Expressions)
                e.Save(writer);
		}
	}
}
