using System;

namespace FLaG
{
	class Empty : Expression
	{
		public Empty()
		{
			
		}

		public override bool Equals (object obj)
		{
			throw new NotImplementedException ();
		}

		public override int GetHashCode ()
		{
			throw new NotImplementedException ();
		}

		public override Expression DeepClone ()
		{
			throw new NotImplementedException ();
		}
	}
}

