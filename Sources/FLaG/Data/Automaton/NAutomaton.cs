using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class NAutomaton
	{
		public void SaveFunctions(Writer writer)
		{
			if (Functions.Count == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < Functions.Count; i++)
				{
					if (i != 0)		
						writer.Write(", ");
					
					Functions[i].Save(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		private void SaveStatuses(Writer writer, NStatus[] statuses)
		{
			if (statuses.Length == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < statuses.Length; i++)
				{
					if (i != 0)		
						writer.Write(", ");
					
					statuses[i].Save(writer,IsLeft);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public void SaveAlphabet(Writer writer)
		{
			Symbol[] symbols = Alphabet;
			
			if (symbols.Length == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < symbols.Length; i++)
				{
					if (i != 0)		
						writer.Write(", ");
					
					symbols[i].Save(writer);
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public void SaveEndStatuses(Writer writer)
		{
			SaveStatuses(writer,EndStatuses.ToArray());
		}
		
		private bool AddStatus(List<NStatus> statuses, NStatus item)
		{
			int index = statuses.BinarySearch(item);
			if (index < 0)
				statuses.Insert(~index,item);
			
			return index < 0;
		}

		public bool AddSymbol(List<Symbol> symbols, Symbol item)
		{
			int index = symbols.BinarySearch(item);
			
			if (index < 0)
				symbols.Insert(~index,item);
			
			return index < 0;
		}
		
		public Symbol[] Alphabet
		{
			get
			{
				List<Symbol> symbols = new List<Symbol>();
				
				foreach (NTransitionFunc func in Functions)
					AddSymbol(symbols,func.Symbol);
				
				return symbols.ToArray();
			}
		}
		
		public NStatus[] Statuses
		{
			get
			{
				List<NStatus> statuses = new List<NStatus>();
				
				foreach (NTransitionFunc func in Functions)
				{
					AddStatus(statuses,func.OldStatus);
					AddStatus(statuses,func.NewStatus);
				}
				
				AddStatus(statuses,InitialStatus);
				
				return statuses.ToArray();
			}
		}
		
		public void SaveStatuses(Writer writer)
		{
			SaveStatuses(writer, Statuses);
		}
		
		public string Apostrophs
		{
			get
			{
				if (IsLeft)
					return "'";
				else
					return "''";
			}
		}
		
		private void SaveLetter(char Letter, Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write(Letter.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveM(Writer writer)
		{
			SaveLetter('M',writer);
		}
		
		public void SaveS(Writer writer)
		{
			SaveLetter('S',writer);
		}
		
		public void SaveQ0(Writer writer)
		{
			writer.Write(@"{{{Q_0}_{");
			writer.Write(Number);
			writer.Write(@"}}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveDelta(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{\delta}");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveSigma(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{\Sigma}");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}");
			writer.Write(Apostrophs);
			writer.Write(@"}");
		}
		
		public void SaveQ(Writer writer)
		{
			SaveLetter('Q',writer);
		}
		
		public void SaveCortege(Writer writer)
		{
			SaveM(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"(");
			SaveQ(writer);
			writer.WriteLine(@",");
			SaveSigma(writer);
			writer.WriteLine(@",");
			SaveDelta(writer);
			writer.WriteLine(@",");
			SaveQ0(writer);
			writer.WriteLine(@",");
			SaveS(writer);
			writer.WriteLine(@")");
		}
		
		public bool IsLeft
		{
			get;
			set;
		}
		
		public int Number		
		{
			get;
			set;
		}
		
		public bool AddFunc(NTransitionFunc item)		
		{
			int index = Functions.BinarySearch(item);
			if (index < 0)
				Functions.Insert(~index,item);
			
			return index < 0;
		}	
		
		public bool AddEndStatus(NStatus item)		
		{
			return AddStatus(EndStatuses,item);
		}	
		
		public List<NStatus> EndStatuses
		{
			get;
			private set;
		}
		
		public List<NTransitionFunc> Functions
		{
			get;
			private set;
		}
		
		public NStatus InitialStatus
		{
			get;
			set;
		}
		
		public NAutomaton()
		{
			Functions = new List<NTransitionFunc>();	
			EndStatuses = new List<NStatus>();
		}
	}
}

