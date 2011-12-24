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
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;
			
				// поднимаем конкатенации
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
				
				for (int i = 0; i < Expressions.Count - 1; i++)
				{
					Expression expr1 = Expressions[i];
					Expression expr2 = Expressions[i + 1];
					
					// a a = a a, a* a* = a*
					if (expr1.Equals(expr2))
					{
						if (expr1 is Repeat && expr2 is Repeat && !((Repeat)expr1).AtLeastOne)
						{
							Expressions.RemoveAt(i+1);	
							somethingChanged = true;	
						}
					}
					// a a^n = a^(n+1)
					else if (expr2 is Degree && expr1.Equals(((Degree)expr2).Base))
					{
						Degree degree = (Degree)expr2;
						degree.Exp++;
						Expressions[i] = degree.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
					// a a* = a+
					else if (expr2 is Repeat 
						&& !((Repeat)expr2).AtLeastOne 
						&& expr1.Equals(((Repeat)expr2).Expression))
					{
						Repeat repeat = (Repeat)expr2;
						repeat.AtLeastOne = true;
						Expressions[i] = repeat.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
					// a^n a = a^(n+1)
					else if (expr1 is Degree && expr2.Equals(((Degree)expr1).Base))
					{
						Degree degree = (Degree)expr1;
						degree.Exp++;
						Expressions[i] = degree.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
					// a^n a^m = a^(n+m)
					else if (expr1 is Degree && expr2 is Degree 
						&& ((Degree)expr1).Base.Equals(((Degree)expr2).Base))
					{
						Degree degreeExpr1 = (Degree)expr1;
						Degree degreeExpr2 = (Degree)expr2;
						
						degreeExpr1.Exp += degreeExpr2.Exp;
						
						Expressions[i] = degreeExpr1.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
					// a^n a*
					else if (expr1 is Degree && expr2 is Repeat 
						&& !((Repeat)expr2).AtLeastOne 
						&& ((Degree)expr1).Base.Equals(((Repeat)expr2).Expression))
					{
						Degree degreeExpr1 = (Degree)expr1;
						Repeat repeatExpr2 = (Repeat)expr2;						
						
						degreeExpr1.Exp--;
						repeatExpr2.AtLeastOne = true;
						Expressions[i] = degreeExpr1.Optimize();
						Expressions[i+1] = repeatExpr2.Optimize();
						somethingChanged = true;
					}
					// a* a = a+
					else if (expr1 is Repeat 
						&& !((Repeat)expr1).AtLeastOne 
						&& expr2.Equals(((Repeat)expr1).Expression))
					{
						Repeat repeat = (Repeat)expr1;
						repeat.AtLeastOne = true;
						Expressions[i] = repeat.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
					// a* a^n
					else if (expr2 is Degree && expr1 is Repeat 
						&& !((Repeat)expr1).AtLeastOne 
						&& ((Degree)expr2).Base.Equals(((Repeat)expr1).Expression))
					{
						Repeat repeatExpr1 = (Repeat)expr1;						
						Degree degreeExpr2 = (Degree)expr2;
						
						repeatExpr1.AtLeastOne = true;
						degreeExpr2.Exp--;
						Expressions[i] = repeatExpr1.Optimize();
						Expressions[i+1] = degreeExpr2.Optimize();
						somethingChanged = true;
					}
					// a*a+ и a+a*
					else if (expr1 is Repeat && expr2 is Repeat 
						&& ((Repeat)expr1).Expression.Equals(((Repeat)expr2).Expression))
					{
						Repeat repeatExpr1 = (Repeat)expr1;
						
						repeatExpr1.AtLeastOne = true;
						Expressions[i] = repeatExpr1.Optimize();
						Expressions.RemoveAt(i+1);
						i--;
						somethingChanged = true;
					}
				}
				
				for (int i = 0; i < Expressions.Count - 2; i++)
				{
					if (!(Expressions[i + 1] is Repeat))
						continue;
					
					Repeat repeat = (Repeat)Expressions[i+1];
					
					if (!(repeat.Expression is Concat))
						continue;
					
					Concat concat = (Concat)repeat.Expression;
					
					if (concat.Expressions.Count != 2)
						continue;
					
					if (Expressions[i].Equals(concat.Expressions[1]) && 
						Expressions[i+2].Equals(concat.Expressions[0]))
					{
						Concat newConcat = new Concat();
						newConcat.Expressions.Add(concat.Expressions[1]);
						newConcat.Expressions.Add(concat.Expressions[0]);
						
						somethingChanged = true;
						
						Repeat newRepeat;
						
						if (repeat.AtLeastOne)
						{
							Expressions[i+1] = newConcat.DeepClone().Optimize();							
							newRepeat = new Repeat();
							newRepeat.Expression = newConcat;
							newRepeat.AtLeastOne = true;							
							Expressions[i] = newRepeat.Optimize();
							Expressions.RemoveAt(i+2);
						}
						else
						{
							Expressions[i] = newConcat;
							newRepeat = new Repeat();
							newRepeat.Expression = newConcat;
							newRepeat.AtLeastOne = true;							
							Expressions[i] = newRepeat.Optimize();
							Expressions.RemoveAt(i+2);
							Expressions.RemoveAt(i+1);
						}
						
						i--;
					}
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
			{
				if (e is Alter)
					writer.Write(@"(");
				
                e.Save(writer);
				
				if (e is Alter)
					writer.Write(@")");
			}
		}
		
		public override bool IsLetEmpty()
		{
			foreach (Expression e in Expressions)
			{
				if (!e.IsLetEmpty())
					return false;
			}
			
			return true;
		}
		
		public override void LetBeEmpty()
		{
			// ничего не делаем
		}
	}
}
