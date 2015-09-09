using FLaG.Data.Automaton;
using System.Collections.Generic;

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

