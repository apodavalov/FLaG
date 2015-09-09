using FLaG.Output;

namespace FLaG.Data.Equation
{
    class Empty : Expression
	{
		public Empty()
		{
			
		}

		public override bool Equals (object obj)
		{
			return obj is Empty;
		}

		public override int GetHashCode ()
		{
			return 0;
		}

		public override Expression DeepClone ()
		{
			return this;
		}

		public override Expression Optimize ()
		{
			return this;
		}
		
		public override void Save(Writer writer)
		{
			writer.Write(@"\varepsilon ");
		}
		
		public override bool IsLetEmpty()
		{
			return true;
		}
		
		public override void LetBeEmpty()
		{
			// мы и так сама пустота =)
		}
	}
}

