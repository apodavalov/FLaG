using System;
using System.Collections.Generic;
using FLaG.Data.Automaton;

namespace FLaG.Data.Helpers
{
	class ClassEqualityComparerByGroupNum : IComparer<ClassEquality>
	{
		public int Compare (ClassEquality x, ClassEquality y)
		{
			return x.GroupNum.CompareTo(y.GroupNum);
		}
	}
}

