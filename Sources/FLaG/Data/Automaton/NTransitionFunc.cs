using FLaG.Output;
using System;

namespace FLaG.Data.Automaton
{
	class NTransitionFunc : IComparable<NTransitionFunc>
	{
		public void Save(Writer writer, bool IsLeft)
		{
			writer.WriteLine(@"\delta(");
			OldStatus.Save(writer,IsLeft);
			writer.WriteLine(@",");
			Symbol.Save(writer);
			writer.WriteLine(@")");
			writer.WriteLine(@"=");
			NewStatus.Save(writer,IsLeft);
		}
		
		public NTransitionFunc(NStatus oldStatus, Symbol symbol, NStatus newStatus)
		{
			OldStatus = oldStatus;
			Symbol = symbol;
			NewStatus = newStatus;
		}
		
		public NStatus OldStatus
		{
			get;
			private set;
		}
		
		public Symbol Symbol
		{
			get;
			private set;
		}
		
		public NStatus NewStatus
		{
			get;
			private set;
		}
		
		public int CompareTo(NTransitionFunc other)
		{
			int res = OldStatus.CompareTo(other.OldStatus);
			
			if (res != 0)
				return res;
			
			res = Symbol.CompareTo(other.Symbol);
			
			if (res != 0)
				return res;
			
			return NewStatus.CompareTo(other.NewStatus);				
		}
	}
}

