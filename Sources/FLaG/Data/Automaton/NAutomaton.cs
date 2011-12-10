using System;
using System.Collections.Generic;
using FLaG.Output;
using FLaG.Data.Helpers;

namespace FLaG.Data.Automaton
{
	class NAutomaton
	{
		private void PassToNewState(bool[] statusesOn)
		{
			for (int i = 0; i < statusesOn.Length; i++)
			{
				if (statusesOn[i])
					statusesOn[i] = false;
				else
				{
					statusesOn[i] = true;
					break;
				}
			}
		}
		
		private DAutomaton _MakeDeterministic()
		{
			DAutomaton automaton = new DAutomaton();
			automaton.Number = Number + 1;
			automaton.IsLeft = IsLeft;
			automaton.ProducedFromDFA = false;
			
			// получаем функции перехода
			NTransitionFunc[] functions = Functions.ToArray();
			
			// сортируем их по символу перехода
			NTransitionFuncBySymbolComparer symbolComparer = new NTransitionFuncBySymbolComparer();
			Array.Sort<NTransitionFunc>(functions,symbolComparer);
			
			// получаем алфавит
			Symbol[] alphabet = Alphabet;
			
			// разбиваем функции на кластеры по полученному алфавиту	
			
			NTransitionFuncCluster[] clusters = new NTransitionFuncCluster[alphabet.Length];
			int oldIndex = -1;
			
			int m = 0;
			Symbol prev = null;
			
			// цикл по всем кроме последнего
			foreach (Symbol s in alphabet)
			{
				NTransitionFunc funcToSearch = new NTransitionFunc(null,s,null);				
				int index = ~Array.BinarySearch<NTransitionFunc>(functions,funcToSearch,symbolComparer);
				
				if (oldIndex >= 0)
				{
					NTransitionFuncCluster cluster = new NTransitionFuncCluster();
					cluster.Symbol = prev;
					cluster.Functions = new NTransitionFunc[index - oldIndex];
					for (int i = oldIndex; i < index; i++)
						cluster.Functions[i - oldIndex] = functions[i];
					clusters[m++] = cluster;
				}
				
				prev = s;
				oldIndex = index;
			}
			
			// последний отдельно рассматриваем
			if (oldIndex >= 0)
			{
				NTransitionFuncCluster cluster = new NTransitionFuncCluster();
				cluster.Symbol = prev;
				cluster.Functions = new NTransitionFunc[functions.Length - oldIndex];
				for (int i = oldIndex; i < functions.Length; i++)
					cluster.Functions[i - oldIndex] = functions[i];	
				clusters[m++] = cluster;
			}
			
			NStatus[] statuses = Statuses;
			bool[] statusesOn = new bool[statuses.Length];
			
			// проходимся по кластеру
			foreach (NTransitionFuncCluster cluster in clusters)
			{
				for (int i = 0; i < statusesOn.Length; i++)
					statusesOn[i] = false;
				
				do
				{
					PassToNewState(statusesOn);
					
					List<NStatus> newStatuses = new List<NStatus>();
					
					foreach (NTransitionFunc func in cluster.Functions)
					{
						int index = Array.BinarySearch<NStatus>(statuses,func.OldStatus);
						if (statusesOn[index])
							AddStatus(newStatuses,func.NewStatus);
					}
					
					DStatus oldStatus = new DStatus();
					
					for (int i = 0; i < statuses.Length; i++)
						if (statusesOn[i])
							oldStatus.AddStatus(statuses[i]);
					
					DStatus newStatus = new DStatus();
					
					foreach (NStatus st in newStatuses)
						newStatus.AddStatus(st);
					
					if (newStatus.Set.Count != 0)					
						automaton.AddFunc(new DTransitionFunc(oldStatus,cluster.Symbol,newStatus));
					
				} while (!Array.TrueForAll<bool>(statusesOn, x => x));
			}
			
			DStatus initialStatus = new DStatus();
			initialStatus.AddStatus(InitialStatus);
			automaton.InitialStatus = initialStatus;
			
			foreach (DStatus status in automaton.Statuses)
			{
				bool atLeastOneFromEnd = false;
				foreach (NStatus endStatus in EndStatuses)				
					if (status.Set.BinarySearch(endStatus) >= 0)
					{
						atLeastOneFromEnd = true;
						break;
					}
				
				if (atLeastOneFromEnd)
					automaton.AddEndStatus(status);
			}
				
			return automaton;
		}
		
		public DAutomaton MakeDeterministic(Writer writer)
		{
			DAutomaton automaton = _MakeDeterministic();
			
			writer.WriteLine(@"Построим для недетерминированного конечного автомата детерминированный конечный автомат", true);
			writer.WriteLine(@"\begin{math}");
			automaton.SaveCortege(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Множество состояний", true);
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"состоит из всех подмножеств множества",true);
			writer.WriteLine(@"\begin{math}");
			SaveQ(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Каждое состояние из",true);
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"будем обозначать",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"[ A_1 A_2 \dots A_n ]");			
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где ",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"A_i \in");			
			SaveQ(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"(учитываем, что состояния",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"[ A_i A_j ]");			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"и",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"[ A_j A_i ]");			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- одно и тоже состояние).",true);
			writer.WriteLine(@"Тогда получаем множество",true);
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"содержащее количество состояний, выражающееся по формуле",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"N_{");
			automaton.SaveQ(writer);
			writer.WriteLine(@"}");
			writer.WriteLine(@"=");
			writer.WriteLine(@"2^n-1");
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"что при",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"n=");
			NStatus[] statuses = Statuses;
			writer.WriteLine(statuses.Length);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"нам дает",true);			
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"N_{");
			automaton.SaveQ(writer);
			writer.WriteLine(@"}");
			writer.WriteLine(@"=");			
			long maxCountStatuses = (long)Math.Pow(2,statuses.Length)-1;
			writer.WriteLine(maxCountStatuses);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"состояний.",true);			
			writer.WriteLine(@"В результате получаем следующее множество всех состояний детерминированного",true);	
			writer.WriteLine(@"конечного автомата (здесь исключены состояния, которые не встречаются в функциях перехода,",true);
			writer.WriteLine(@"не являются начальным и конечным состоянием автомата, всего таких состояний",true);				
			writer.Write(maxCountStatuses - automaton.Statuses.Length);
			writer.WriteLine(@")",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ(writer);
			writer.WriteLine(@"=");
			automaton.SaveStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			writer.WriteLine(@"Входной алфавит детерминированного конечного автомата совпадает с входным алфавитом ",true);	
			writer.WriteLine(@"недетерминированного конечного автомата, т.е.",true);	
			writer.WriteLine(@"\begin{math}");
			automaton.SaveSigma(writer);
			writer.WriteLine(@"=");
			SaveSigma(writer);
			writer.WriteLine(@"=");
			automaton.SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			
			return automaton;
		}
		
		public DStatus MakeSimpliestStatus (NStatus status)
		{
			DStatus dStatus = new DStatus();
			dStatus.AddStatus(status);

			return dStatus;
		}

		public DTransitionFunc MakeSimpliestFunc (NTransitionFunc func)
		{
			return new DTransitionFunc(MakeSimpliestStatus(func.OldStatus),func.Symbol,MakeSimpliestStatus(func.NewStatus));
		}
		
		public DAutomaton MakeSimpliest()
		{
			DAutomaton automaton = new DAutomaton();
			automaton.IsLeft = IsLeft;
			automaton.Number = Number;
			automaton.ProducedFromDFA = true;
			
			automaton.InitialStatus = MakeSimpliestStatus(InitialStatus);
			
			foreach (NTransitionFunc func in Functions)
				automaton.AddFunc(MakeSimpliestFunc(func));
			
			foreach (NStatus status in EndStatuses)
				automaton.AddEndStatus(MakeSimpliestStatus(status));
			
			return automaton;
		}
		
		public bool IsDFA ()
		{
			for (int i = 0; i < Functions.Count - 1; i++)
			{
				if (Functions[i+1].OldStatus.CompareTo(Functions[i].OldStatus) == 0 &&
				    Functions[i+1].Symbol.CompareTo(Functions[i].Symbol) == 0)
					return false;
			}
				
			return true;
		}
		
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
				
				foreach (NStatus status in EndStatuses)
					AddStatus(statuses,status);
				
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

