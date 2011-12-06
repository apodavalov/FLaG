using System;

namespace FLaG.Data.Automaton
{
	interface Status : IComparable<Status>
	{
		bool Equals(object obj);
	}
}

