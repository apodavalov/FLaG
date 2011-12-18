using System;
using FLaG.Output;

namespace FLaG.Data.Equation
{
	class Degree : Expression
	{
		public Expression Base
		{
			get;
			set;
		}
		
		public int Exp
		{
			get;
			set;
		}
		
		public override Expression Optimize ()
		{
			if (Exp == 0)
				return new Empty();
			else if (Exp == 1)
				return Base.Optimize();
			else
			{
				Base = Base.Optimize();
				if (Base is Degree)
				{
					Degree degree = (Degree)Base;
					degree.Exp *= Exp;
					return degree;
				}
				else if (Base is Repeat)
				{
					Repeat repeat = (Repeat)Base;
					if (!repeat.AtLeastOne)
						return repeat;
					else
						return this;
				}
				else
					return this;
			}
		}

		public override Expression DeepClone ()
		{
			Degree degree = new Degree();
			degree.Base = Base.DeepClone();
			degree.Exp = Exp;
			
			return degree;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Degree))
				return false;
			
			Degree o = (Degree)obj;
			
			return Exp == o.Exp && Base.Equals(o.Base);
		}

		public override int GetHashCode()
		{
			return Base.GetHashCode() ^ Exp.GetHashCode();
		}
		
		public override void Save(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			Base.Save(writer);			
			writer.Write(@"}^{");
			writer.Write(Exp);
			writer.Write(@"}");
			writer.Write(@"}");
		}
	}
}

