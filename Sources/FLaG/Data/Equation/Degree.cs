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
			// TODO: оптимизировать
			return this;
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

