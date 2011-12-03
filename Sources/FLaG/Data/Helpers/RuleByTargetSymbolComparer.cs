using System;
using System.Collections.Generic;
using FLaG.Data.Grammars;

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

