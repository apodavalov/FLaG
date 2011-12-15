using System;

namespace FLaG
{
	class Repeat : Expression
	{
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
			throw new NotImplementedException ();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException ();
		}

		public override Expression DeepClone ()
		{
			throw new NotImplementedException ();
		}
	}
}

