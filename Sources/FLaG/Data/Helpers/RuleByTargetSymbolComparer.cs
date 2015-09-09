using FLaG.Data.Grammars;
using System.Collections.Generic;

namespace FLaG.Data.Helpers
{
    class RuleByTargetSymbolComparer : IComparer<Rule>
	{
		public int Compare (Rule x, Rule y)
		{
			return x.Prerequisite.CompareTo(y.Prerequisite);
		}
	}
}

