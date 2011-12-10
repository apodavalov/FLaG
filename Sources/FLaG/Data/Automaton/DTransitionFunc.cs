using System;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class DTransitionFunc : IComparable<DTransitionFunc>
	{
		public void Save(Writer writer, bool IsLeft, bool producedFromDFA)
		{
			writer.WriteLine(@"\delta(");
			OldStatus.Save(writer,IsLeft,producedFromDFA);
			writer.WriteLine(@",");
			Symbol.Save(writer);
			writer.WriteLine(@")");
			writer.WriteLine(@"=");
			NewStatus.Save(writer,IsLeft,producedFromDFA);
		}
		
		public DTransitionFunc(DStatus oldStatus, Symbol symbol, DStatus newStatus)
		{
			OldStatus = oldStatus;
			Symbol = symbol;
			NewStatus = newStatus;
		}
		
		public DStatus OldStatus
		{
			get;
			set;
		}
		
		public Symbol Symbol
		{
			get;
			set;
		}
		
		public DStatus NewStatus
		{
			get;
			set;
		}
		
		public int CompareTo(DTransitionFunc other)
		{
			int res = OldStatus.CompareTo(other.OldStatus);
			
			if (res != 0)
				return res;
			
			return Symbol.CompareTo(other.Symbol);
		}
	}
}

