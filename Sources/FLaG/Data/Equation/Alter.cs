using System;
using System.Collections.Generic;
using FLaG.Output;

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
			List<Expression> list = new List<Expression>();
			
			foreach (Expression e in Expressions)
				list.Add(e);
			
			Expressions.Clear();
			
			for (int i = 0; i < list.Count; i++)
				list[i] = list[i].Optimize();
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;			
			
				// поднимаем альтернативы
				for (int i = 0; i < list.Count; i++)
					if (list[i] is Alter)
					{
						Alter a = (Alter)list[i];
						list.RemoveAt(i);
						foreach (Expression e in a.Expressions)
							list.Insert(i++,e);
						i--;						
						somethingChanged = true;
					}
				
				bool haveEmpty = false;
				
				for (int i = 0; i < list.Count; i++)
					if (list[i] is Empty || list[i] is Repeat && !((Repeat)list[i]).AtLeastOne)
					{
						haveEmpty = true;
						break;
					}
				
				if (haveEmpty)
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is Repeat)		
						{
							Repeat repeat = (Repeat)list[i];
							if (repeat.AtLeastOne)
							{	
								repeat.AtLeastOne = false;
								list[i] = repeat.Optimize();
								somethingChanged = true;
							}
						}
					}
				
				bool haveRepeatWithAtLeastOneFalse = false;
				
				for (int i = 0; i < list.Count; i++)
					if (list[i] is Repeat && !((Repeat)list[i]).AtLeastOne)
					{
						haveRepeatWithAtLeastOneFalse = true;
						break;
					}
				
				if (haveRepeatWithAtLeastOneFalse)
					for (int i = 0; i < list.Count; i++)
						if (list[i] is Empty)
						{
							list.RemoveAt(i);
							i--;
							somethingChanged = true;
						}
				
				List<Repeat> repeats = new List<Repeat>();
				
				for (int i = 0; i < list.Count; i++)
					if (list[i] is Repeat)
						repeats.Add((Repeat)list[i]);
				
				for (int i = 0; i < repeats.Count; i++)
					for (int j = 0; j < list.Count; j++)
					{
						bool rem = false;
					
						if (list[j].Equals(repeats[i].Expression))
							rem = true;
						else if (list[j] is Degree)
						{
							Degree d = (Degree)list[j];
							if (d.Base.Equals(repeats[i].Expression))
								rem = true;
						}
					
						if (rem)
						{
							list.RemoveAt(j);
							somethingChanged = true;
							j--;
						}
					}
				
			} while (somethingChanged);
			
			foreach (Expression e in list)
				Expressions.Add(e);

			if (list.Count == 0)
				return new Empty();
			else if (list.Count == 1)
				return list[0];
			else
				return this;				
		}

		public override void Save(Writer writer)
		{
			writer.Write("(");
			
			bool first = true;

            foreach (Expression e in Expressions)
            {
                if (!first)
					writer.Write(@" + ");
				else
					first = false;

                e.Save(writer);
            }                      
			
			writer.Write(")");
		}
	}
}

