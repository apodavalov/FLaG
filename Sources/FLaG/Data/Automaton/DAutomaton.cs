using System;
using System.Collections.Generic;
using FLaG.Output;
using FLaG.Data.Helpers;

namespace FLaG.Data.Automaton
{
	class DAutomaton
	{		
		public void SaveP(Writer writer, int number)
		{
			writer.Write(@"{");
			writer.Write(@"P");
			writer.Write("_{");
			writer.Write(number);
			writer.Write(@"}");
			writer.Write(@"}");
		}
		
		public void SaveR (Writer writer)
		{
			writer.WriteLine("R");
		}
		
		private NAutomaton Reorganize(Writer writer)
		{
			NAutomaton automaton = new NAutomaton();
			automaton.IsLeft = IsLeft;
			automaton.Number = Number + 1;
			
			Dictionary<DStatus,NStatus> dictionary = new Dictionary<DStatus, NStatus>();
			
			int i = 1;
			
			DStatus[] statuses = Statuses;
			
			foreach (DStatus status in statuses)
			{
				dictionary.Add(status,new NStatus('S',i));
				i++;
			}
			
			automaton.InitialStatus = dictionary[InitialStatus];
			
			foreach (DTransitionFunc func in Functions)
				automaton.AddFunc(new NTransitionFunc(dictionary[func.OldStatus],func.Symbol,dictionary[func.NewStatus]));
			
			foreach (DStatus status in EndStatuses)
				automaton.AddEndStatus(dictionary[status]);
			
			writer.WriteLine();
			writer.WriteLine(@"Для упрощения дальнейших преобразований выполним переобозначения состояний ДКА. В результате получим автомат",true);
			writer.WriteLine(@"\begin{math}");
			automaton.SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ(writer);
			writer.WriteLine(@"=");
			automaton.SaveStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- конечное множество состояний автомата;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveSigma(writer);
			writer.WriteLine(@"=");
			automaton.SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- входной алфавит автомата (конечное множество допустимых входных символов);",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveDelta(writer);
			writer.WriteLine(@"=");
			automaton.SaveFunctions(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество функций переходов;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveQ0(writer);
			writer.WriteLine(@"=");
			automaton.InitialStatus.Save(writer,automaton.IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- начальное состояние автомата;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveS(writer);
			writer.WriteLine(@"=");
			automaton.SaveEndStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество заключительных состояний.",true);
			
			return automaton;
		}
		
		public NAutomaton RemoveUnreachedStates (Writer writer)
		{
			writer.WriteLine(@"Выполним удаление недостижимых состояний построенного ДКА. Для ",true);
			writer.WriteLine(@"этого будем использовать два дополнительных множества: ",true);
			writer.WriteLine(@"множество достижимых состояний ",true);
			writer.WriteLine(@"\begin{math}");
			SaveR(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"и множество текущих активных состояний на каждом шаге алгоритма ",true);
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine("P_i");
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine(@"Результатом работы алгоритма является полное множество достижимых состояний.",true);
			writer.WriteLine();
			writer.WriteLine(@"На первом шаге работы алгоритма получаем",true);
			
			int i = 0;
			List<DStatus> R = new List<DStatus>();
			List<DStatus> P = new List<DStatus>();
			
			AddStatus(R,InitialStatus);
			AddStatus(P,InitialStatus);
			
			writer.WriteLine(@"\begin{math}");
			SaveR(writer);
			writer.WriteLine(@"=");
			SaveStatuses(writer, R.ToArray(),false);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"\begin{math}");
			writer.WriteLine(@"i=");
			writer.WriteLine(i);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"\begin{math}");
			SaveP(writer,i);
			writer.WriteLine(@"=");
			SaveStatuses(writer,P.ToArray(),false);
			writer.WriteLine(@"\end{math}.");
			
			i++;
			
			writer.WriteLine();
			writer.WriteLine(@"Далее множество ",true);
			writer.WriteLine(@"\begin{math}");
			SaveP(writer,i);
			writer.WriteLine(@"\end{math}");	
			writer.WriteLine("примет следующий вид",true);
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;
				
				List<DStatus> newP = new List<DStatus>();
				
				foreach (DTransitionFunc func in Functions)
					if (P.BinarySearch(func.OldStatus) >= 0)
						AddStatus(newP,func.NewStatus);

				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				SaveP(writer,i);
				writer.WriteLine(@"=");
				SaveStatuses(writer,newP.ToArray(),false);
				writer.WriteLine(@"\end{math}.");
				writer.WriteLine();
				writer.WriteLine(@"Рассматриваем разность двух множеств",true);
				writer.WriteLine(@"\begin{math}");
				SaveP(writer,i);
				writer.WriteLine(@"\end{math}");
				writer.WriteLine(@"и",true);
				writer.WriteLine(@"\begin{math}");
				SaveR(writer);
				writer.WriteLine(@"\end{math}.");
				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				SaveP(writer,i);
				writer.WriteLine(@"\setminus");
				SaveR(writer);
				writer.WriteLine(@"=");
				SaveStatuses(writer,newP.ToArray(),false);
				writer.WriteLine(@"\setminus");
				SaveStatuses(writer,R.ToArray(),false);
				writer.WriteLine(@"=");
				
				foreach (DStatus status in R)
				{
					int index = newP.BinarySearch(status);
					
					if (index >= 0)
						newP.RemoveAt(index);
				}
				SaveStatuses(writer,newP.ToArray(),false);
				writer.WriteLine(@"\end{math}.");
				
				writer.WriteLine();
				
				if (newP.Count != 0)
				{
					writer.WriteLine(@"Так как разность этих двух множеств не пустое множество,", true);
					writer.WriteLine(@"то к множеству ", true);
					writer.WriteLine(@"\begin{math}");
					SaveR(writer);
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine(@"добавляем элементы из множества", true);
					writer.WriteLine(@"\begin{math}");
					SaveP(writer,i);
					writer.WriteLine(@"\end{math}");
					writer.WriteLine();
					writer.WriteLine(@"\begin{math}");
					SaveR(writer);
					writer.WriteLine(@"\cup");
					SaveP(writer,i);
					writer.WriteLine(@"=");
					SaveStatuses(writer,R.ToArray(),false);
					writer.WriteLine(@"\cup");
					SaveStatuses(writer,newP.ToArray(),false);
					
					foreach (DStatus status in newP)
						AddStatus(R,status);
					
					writer.WriteLine(@"=");
					SaveStatuses(writer,R.ToArray(),false);
					
					writer.WriteLine(@"\end{math}");
					writer.WriteLine();
					writer.WriteLine(@"Алгоритм продолжаем дальше.");
					i++;
					somethingChanged = true;
					P = newP;
					
					writer.WriteLine(@"На следующем шаге алгоритма строим множество",true);
					writer.WriteLine(@"\begin{math}");
					SaveP(writer,i);
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine(@"В это множество включаются все состояния, в которые",true);
					writer.WriteLine(@"можно перейти по функциям перехода из состояний,",true);
					writer.WriteLine(@"входящих в множество",true);
					writer.WriteLine(@"\begin{math}");
					SaveP(writer,i-1);
					writer.WriteLine(@"\end{math}.");
					writer.WriteLine(@"Таким образом, множество текущих активных состояний примет следующий вид",true);					
				}
				else
				{
					writer.WriteLine(@"Так как разность этих двух множеств пустое множество,", true);
					writer.WriteLine(@"то выполнение алгоритма недостижимых состояний заканчивается.", true);
				}
			} while (somethingChanged);
			
			writer.WriteLine();
			writer.WriteLine(@"После выполнения этого алгоритма из множества всех состояний ДКА", true);
			writer.WriteLine(@"удаляем недостижимые состояния. Таким образом, множество состояний ДКА примет вид", true);
			
			for (i = 0; i < Functions.Count; i++)
			{
				if (R.BinarySearch(Functions[i].OldStatus) < 0)
				{
					Functions.RemoveAt(i);
					i--;
				}
			}
			
			DStatus[] oldEndStatuses = EndStatuses.ToArray();
			EndStatuses.Clear();
			DStatus[] currentStatuses = Statuses;
			
			foreach (DStatus status in oldEndStatuses)
				if (Array.BinarySearch<DStatus>(currentStatuses,status) >= 0)
					AddEndStatus(status);
			
			writer.WriteLine();
			
			writer.WriteLine(@"\begin{math}");
			SaveQ(writer);
			writer.WriteLine(@"=");
			SaveStatuses(writer,false);			
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			writer.WriteLine(@"Множество функций переходов эквивалентного ДКА преобразуется к виду",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveDelta(writer);
			writer.WriteLine(@"=");
			SaveFunctions(writer,false);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine();
			writer.WriteLine(@"Начальным состоянием эквивалентного ДКА",true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"будет состояние",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveQ0(writer);
			writer.WriteLine(@"=");
			InitialStatus.Save(writer,IsLeft,ProducedFromDFA);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"Множество заключительных состояний ДКА",true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"примет вид",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveS(writer);
			writer.WriteLine(@"=");
			SaveEndStatuses(writer,false);
			writer.WriteLine(@"\end{math}");
			return Reorganize(writer);
		}
		
		public void SaveFunctionsShort (Writer writer)
		{
//			Dictionary<DStatus,List<DTransitionFunc>> dictionary =
//				new Dictionary<DStatus, List<DTransitionFunc>>();
//			
//			foreach (DTransitionFunc func in Functions)
//			{
//				if (!dictionary.ContainsKey(func.NewStatus))
//					dictionary[func.NewStatus] = new List<DTransitionFunc>();
//				
//				dictionary[func.NewStatus].Add(func);
//			}
			
			DTransitionFunc[] functions;
			
			if (Functions.Count >= 9)
			{
				functions = new DTransitionFunc[9];
				
				for (int i = 0; i < 4; i++)
					functions[i] = Functions[i];
				
				functions[4] = null;
				
				for (int i = 0; i < 4; i++)
					functions[i + 5] = Functions[Functions.Count - 4 + i];
			}
			else
				functions = Functions.ToArray();
			
			if (Functions.Count == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < functions.Length; i++)
				{
					if (i != 0)		
						writer.Write(", \\end{math}\r\n\r\n\\begin{math} ");
					
					if (functions[i] != null)					
						functions[i].Save(writer,IsLeft, ProducedFromDFA);
					else
						writer.WriteLine(@"\dots");
				}
				writer.WriteLine(@"\}");
			}
		}
		
		public void SaveFunctions(Writer writer, bool shortOutput)
		{
			if (shortOutput)
				SaveFunctionsShort(writer);
			else
				SaveFunctionsFull(writer);
		}
		
		private void SaveFunctionsFull(Writer writer)
		{
			if (Functions.Count == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < Functions.Count; i++)
				{
					if (i != 0)		
						writer.Write(", \\end{math}\r\n\r\n\\begin{math} ");
					
					Functions[i].Save(writer,IsLeft, ProducedFromDFA);
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
		
		public void SaveEndStatuses(Writer writer, bool shortOutput)
		{
			SaveStatuses(writer,EndStatuses.ToArray(), shortOutput);
		}
		
		public void SaveStatuses(Writer writer, bool shortOutput)
		{
			SaveStatuses(writer, Statuses, shortOutput);
		}
		
		private void SaveStatusesShort(Writer writer, DStatus[] statuses)
		{
			int prevCount = -1;
			int count = 0;
			int outputCount = 4;
			
			if (statuses.Length == 0)
				writer.WriteLine(@"\varnothing");
			else
			{
				writer.WriteLine(@"\{");
				for (int i = 0; i < statuses.Length; i++)
				{
					if (prevCount != statuses[i].Set.Count)
					{
						count = 0;
						prevCount = statuses[i].Set.Count;
					}
					
					if (count < outputCount)
					{
						if (i != 0)		
							writer.Write(", \\end{math}\r\n\r\n\\begin{math} ");					
						
						statuses[i].Save(writer,IsLeft,ProducedFromDFA);
						count++;
					}
					else if (count == outputCount)
					{
						if (i != 0)		
							writer.Write(", \\end{math}\r\n\r\n\\begin{math} ");
						
						writer.WriteLine(@"\dots");
						count++;
					}
				}
				writer.WriteLine(@"\}");
			}
		}
		
		private void SaveStatusesFull(Writer writer, DStatus[] statuses)
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
		
		private void SaveStatuses(Writer writer, DStatus[] statuses, bool shortOutput)
		{
			if (shortOutput)
				SaveStatusesShort(writer, statuses);
			else
				SaveStatusesFull(writer, statuses);
		}
		
		public void SaveM(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{");
			writer.Write('M'.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
			if (IsLeft)
				writer.Write("'");
			else
				writer.Write("''");
			writer.Write(@"}");
		}
		
		public void SaveS(Writer writer)
		{
			writer.Write(@"{");
			writer.Write('S'.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
		}
		
		public void SaveQ0(Writer writer)
		{
			writer.Write(@"{{Q_0}_{");
			writer.Write(Number);
			writer.Write(@"}}");
		}
		
		public void SaveDelta(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{\delta}");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}");			
			writer.Write(@"}");
		}
		
		public void SaveSigma(Writer writer)
		{
			writer.Write(@"{");
			writer.Write(@"{\Sigma}");
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}");
			writer.Write(@"}");
		}
		
		public void SaveQ(Writer writer)
		{
			writer.Write(@"{");
			writer.Write('Q'.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");			
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

