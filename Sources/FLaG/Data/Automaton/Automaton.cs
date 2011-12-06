using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class Automaton
	{
		public HashSet<TransitionFunc> Functions
		{
			get;
			private set;
		}
		
		public Status InitialStatus
		{
			get;
			set;
		}
		
		public Automaton()
		{
			Functions = new HashSet<TransitionFunc>();	
		}
	}
}

