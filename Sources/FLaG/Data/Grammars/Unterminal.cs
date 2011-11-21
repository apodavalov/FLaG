using System;
using FLaG.Output;
using System.Collections.Generic;

namespace FLaG.Data.Grammars
{
	class Unterminal : Symbol, IComparable<Unterminal>
	{
		public int Number
		{
			get;
			private set;
		}
		
		private static List<Unterminal> unterminals = new List<Unterminal>();
		
		public override Symbol DeepClone()
		{
			return this;
		}
		
		public static Unterminal GetInstance(int number)
		{
			Unterminal u = new Unterminal();
			u.Number = number;
			int index = unterminals.BinarySearch(u);
			
			if (index < 0)
			{
				index = ~index;
				unterminals.Insert(index,u);
			}
			
			return unterminals[index];
		}
		
		private Unterminal()
		{
			
		}

		public override void Save(Writer writer, bool isLeft)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write("S", true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
			if (isLeft)
				writer.Write("'");
			else
				writer.Write("''");
			writer.Write(@"}");
		}

		public int CompareTo(Unterminal other)
		{
			return Number.CompareTo(other.Number);
		}
	}
}

