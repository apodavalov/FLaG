using System;
using System.Collections.Generic;
using FLaG.Output;

namespace FLaG.Data.Automaton
{
	class DAutomaton
	{
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
		
		public Symbol[] Alphabet
		{
			get
			{
				List<Symbol> symbols = new List<Symbol>();
				
				foreach (DTransitionFunc func in Functions)
					AddSymbol(symbols,func.Symbol);
				
				return symbols.ToArray();
			}
		}
		
		public bool AddSymbol(List<Symbol> symbols, Symbol item)
		{
			int index = symbols.BinarySearch(item);
			
			if (index < 0)
				symbols.Insert(~index,item);
			
			return index < 0;
		}
		
		public void SaveStatuses(Writer writer)
		{
			SaveStatuses(writer, Statuses);
		}
		
		private void SaveStatuses(Writer writer, DStatus[] statuses)
		{
			if (statuses.Length == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < statuses.Length; i++)
				{
					if (i != 0)		
						writer.Write(", \\end{math}\r\n\r\n\\begin{math} ");
					
					statuses[i].Save(writer,IsLeft, ProducedFromDFA);
				}
				writer.WriteLine(@"\}");
			}
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
		
		public bool ProducedFromDFA
		{
			get;
			set;
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
		
		public bool AddFunc(DTransitionFunc item)		
		{
			int index = Functions.BinarySearch(item);
			if (index < 0)
				Functions.Insert(~index,item);
			
			return index < 0;
		}		
		
		public bool AddEndStatus(DStatus item)		
		{
			return AddStatus(EndStatuses,item);
		}	
		
		private bool AddStatus(List<DStatus> statuses, DStatus item)
		{
			int index = statuses.BinarySearch(item);
			if (index < 0)
				statuses.Insert(~index,item);
			
			return index < 0;
		}
		
		public DStatus[] Statuses
		{
			get
			{
				List<DStatus> statuses = new List<DStatus>();
				
				foreach (DTransitionFunc func in Functions)
				{
					AddStatus(statuses,func.OldStatus);
					AddStatus(statuses,func.NewStatus);
				}
				
				AddStatus(statuses,InitialStatus);
				
				foreach (DStatus status in EndStatuses)
					AddStatus(statuses,status);
				
				return statuses.ToArray();
			}
		}
		
		public List<DStatus> EndStatuses
		{
			get;
			private set;
		}
		
		public List<DTransitionFunc> Functions
		{
			get;
			private set;
		}
		
		public DStatus InitialStatus
		{
			get;
			set;
		}
		
		public DAutomaton()
		{
			Functions = new List<DTransitionFunc>();	
			EndStatuses = new List<DStatus>();
		}
	}
}

