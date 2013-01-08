using System;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class NStatus : IComparable<NStatus>, IEquatable<NStatus>
	{
		public void Save(Writer writer, bool isLeft)
		{
			writer.Write(@"{");
			writer.Write(Value + "", true);
			
			if (Number != null)
			{
				writer.Write("_{");
				writer.Write(Number);
				writer.Write(@"}");
			}
			writer.Write(@"}");
		}
		
		public NStatus(char value)
		{
			Value = value;
		}
		
		public NStatus(char value, int number)
		{
			Value = value;
			Number = number;
		}
		
		public int? Number
		{
			get;
			private set;
		}
		
		public char Value
		{
			get;
			private set;
		}

		public int CompareTo(NStatus other)
		{
			int res = Value.CompareTo(other.Value);
			
			if (res != 0)
				return res;
			
			if (Number == null && other.Number != null)
				return -1;
			
			if (Number != null && other.Number == null)
				return 1;
			
			if (Number == null && other.Number == null)
				return 0;
			
			return Number.Value.CompareTo(other.Number.Value);
		}

		public bool Equals (NStatus other)
		{
			if (other == null)
				return false;

			return CompareTo(other) == 0;
		}

		public override bool Equals (object obj)
		{
			return Equals(obj as NStatus);
		}

		public override int GetHashCode ()
		{
			return (Number == null ? 0 : Number.GetHashCode()) ^ Value.GetHashCode();
		}
	}
}

