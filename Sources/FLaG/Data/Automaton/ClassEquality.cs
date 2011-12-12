using System;
using System.Collections.Generic;

namespace FLaG.Data.Automaton
{
	class ClassEquality : IComparable<ClassEquality>
	{
		public ClassEquality()
		{
			Symbols = new List<Symbol>();
		}
		
		public List<Symbol> Symbols
		{
			get;
			private set;
		}
		
		public int GroupNum
		{
			get;
			set;
		}
		
		public bool AddSymbol(Symbol item)
		{
			int index = Symbols.BinarySearch(item);
			if (index < 0)
				Symbols.Insert(~index,item);
			
			return index < 0;
		}
		
		public int CompareTo(ClassEquality other)
		{
			int res = GroupNum.CompareTo(other.GroupNum);
			
			if (res != 0)
				return res;
			
			res = Symbols.Count.CompareTo(other.Symbols.Count);
			
			if (res != 0)
				return res;
			
			int count = Symbols.Count;
			
			for (int i = 0; i < count; i++)
			{
				res = Symbols[i].CompareTo(other.Symbols[i]);
				
				if (res != 0)
					return res;
			}
			
			return 0;
		}
	}
}
