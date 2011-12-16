using System;
using FLaG.Output;

namespace FLaG
{
	class Repeat : Expression
	{
		public Expression Expression
		{
			get;
			set;
		}
		
		public bool AtLeastOne
		{
			get;
			set;
		}
		
		public Repeat()
		{
			
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Repeat))
				return false;
			
			Repeat o = (Repeat)obj;
			return o.AtLeastOne == AtLeastOne && Expression.Equals(o.Expression);
		}

		public override int GetHashCode()
		{
			return Expression.GetHashCode() ^ AtLeastOne.GetHashCode();
		}

		public override Expression DeepClone ()
		{
			Repeat r = new Repeat();
			
			r.Expression = Expression.DeepClone();
			r.AtLeastOne = AtLeastOne;
			
			return r;
		}

		public override Expression Optimize ()
		{
			// TODO: оптимизировать
			return this;
		}
		
		public override void Save(Writer writer)
		{
			throw new NotImplementedException ();
		}

	}
}

