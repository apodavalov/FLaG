using System;
using FLaG.Output;
using Gram=FLaG.Data.Grammars;
using System.Collections.Generic;
using Machine=FLaG.Data.Automaton;

namespace FLaG.Data.Equation
{
	class Matrix
	{
		private Expression[][] Mx
		{
			get;
			set;
		}
		
		public Gram.Unterminal[] Unterminals
		{
			get;
			private set;
		}
		
		public int TargetSymbolIndex
		{
			get;
			private set;
		}
		
		public bool IsLeft
		{
			get;
			private set;
		}
		
		private void Save(Writer writer)
		{
			writer.WriteLine(@"\left\{");
			
  			writer.WriteLine(@"\begin{array}{l l}");
			
			for (int i = 0; i < Mx.Length; i++)
			{
				writer.Write(@"{");
				
				Unterminals[i].Save(writer,IsLeft);
				
				writer.Write(@" = ");
				
				bool first = true;
				
				for (int j = 0; j < Mx[i].Length; j++)
				{
					if (Mx[i][j] == null)
						continue;
					
					if (!first)	
						writer.Write(@" + ");
					else
						first = false;
					
					if (IsLeft && j < Mx[i].Length - 1)
						Unterminals[j].Save(writer,IsLeft);
					
					if (!(Mx[i][j] is Empty) || j == Mx[i].Length - 1)
					{
						writer.Write(@"{");
						if (Mx[i][j] is Alter)
							writer.Write(@"(");
						Mx[i][j].Save(writer);
						if (Mx[i][j] is Alter)
							writer.Write(@")");
						writer.Write(@"}");
					}
					
					if (!IsLeft && j < Mx[i].Length - 1)
						Unterminals[j].Save(writer,IsLeft);
				}
				
				writer.WriteLine(@"}\\");
			}
  			writer.WriteLine(@"\end{array} \right.");
		}
		
		private bool IterateSimply(int row, bool isDirect)
		{
			bool res = false;
			
			int first = isDirect ? 0 : row + 1;
			int last = isDirect ? row - 1 : Mx.Length - 1;

			for (int i = first; i <= last; i++)
			{
				if (Mx[row][i] != null)
				{
					Expression expr = Mx[row][i];					
					Mx[row][i] = null;
					
					res = true;
					
					for (int j = 0; j < Mx[i].Length; j++)
					{
						if (Mx[i][j] != null)
						{
							Concat concat = new Concat();
							
							if (!IsLeft)
								concat.Expressions.Add(expr.DeepClone());
							
							concat.Expressions.Add(Mx[i][j].DeepClone());
							
							if (IsLeft)
								concat.Expressions.Add(expr.DeepClone());								
							
							if (Mx[row][j] != null)
							{
								Alter alter = new Alter();
								alter.Expressions.Add(Mx[row][j]);
								alter.Expressions.Add(concat);
								Mx[row][j] = alter.Optimize();
							}
							else
								Mx[row][j] = concat.Optimize();
						}
					}
				}
			}
						
			
			return res;
		}

		private bool IterateAlphaBeta(int row)
		{
			if (Mx[row][row] == null)
				return false;
			
			Repeat repeat = new Repeat();
			
			repeat.AtLeastOne = false;
			repeat.Expression = Mx[row][row];
			Mx[row][row] = null;
			
			for (int i = 0; i < Mx[row].Length; i++)
				if (Mx[row][i] != null)
				{
					Concat concat = new Concat();
					if (!IsLeft)
						concat.Expressions.Add(repeat.DeepClone());
				
					concat.Expressions.Add(Mx[row][i]);
				
					if (IsLeft)
						concat.Expressions.Add(repeat.DeepClone());
				
					Mx[row][i] = concat.Optimize();
				}
			
			return true;
		}

		private void _Save(Writer writer, ref bool first)
		{
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
				
			if (!first)
				writer.Write(@"\Rightarrow ");
			else
				first = false;
				
			Save(writer);
			writer.WriteLine(@"\end{math}");
		}
		
		private void Reverse()
		{
			Array.Reverse(Unterminals);
			TargetSymbolIndex = Unterminals.Length - 1 - TargetSymbolIndex;
			Array.Reverse(Mx);
			
			for (int i = 0; i < Mx.Length; i++)
			{
				int len = Mx[i].Length - 1;
				for (int j = 0; j < len / 2; j++)
				{
					Expression temp = Mx[i][j];
					Mx[i][j] = Mx[i][len - 1 - j];
					Mx[i][len - 1 - j] = temp;
				}
			}
		}
		
		private void Sort()
		{
			int[] labels = new int[Unterminals.Length];
			
			for (int i = 0; i < labels.Length; i++)
				labels[i] = -1;
			
			int t = 0;
			
			labels[TargetSymbolIndex] = t;
			HashSet<int> V,newV;
			
			V = new HashSet<int>();
			V.Add(TargetSymbolIndex);
			
			do
			{
				t++;
				newV = new HashSet<int>();
				
				foreach (int v in V)					
					for (int i = 0; i < Mx[v].Length - 1; i++)
						if (Mx[v][i] != null && labels[i] < 0)
						{
							labels[i] = t;
							newV.Add(i);
						}
				
				V = newV;
			} while (V.Count > 0);

			for (int j = 0; j < labels.Length; j++)
				for (int i = 0; i < labels.Length - 1; i++)
				{
					if (labels[i] < labels[i + 1])
					{
						SwapEquation(i,i+1);
						int temp = labels[i];
						labels[i] = labels[i + 1];
						labels[i + 1] = temp;
					}
				}
		}
		
		private void SwapEquation(int index1, int index2)
		{
			Gram.Unterminal tempUnterminal = Unterminals[index1];
			Unterminals[index1] = Unterminals[index2];
			Unterminals[index2] = tempUnterminal;
			
			if (TargetSymbolIndex == index1)
				TargetSymbolIndex = index2;
			else if (TargetSymbolIndex == index2)
				TargetSymbolIndex = index1;
			
			Expression[] tempExpressions = Mx[index1];
			Mx[index1] = Mx[index2];
			Mx[index2] = tempExpressions;
			
			for (int i = 0; i < Mx.Length; i++)
			{
				Expression tempExpr = Mx[i][index1];
				Mx[i][index1] = Mx[i][index2];
				Mx[i][index2] = tempExpr;
			}
		}		
		
		public Expression Solve(Writer writer, bool reverse)
		{
			if (reverse)
				Reverse();
			writer.WriteLine(@"Система уравнений с регулярными коэффициентами примет следующий вид", true);
			writer.WriteLine(@"\begin{math}");
			Save(writer);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine();		
			writer.WriteLine(@"Найдем решение данной системы.", true);
			
			bool first = true;
			
			for (int i = 0; i < Mx.Length; i++)
			{
				if (IterateSimply(i,true))
					_Save(writer,ref first);
				
				if (IterateAlphaBeta(i))
					_Save(writer,ref first);
			}
			
			for (int i = Mx.Length - 1; i >= 0; i--)
			{
				if (IterateSimply(i,false))
					_Save(writer,ref first);
				
				if (IterateAlphaBeta(i))
					_Save(writer,ref first);
			}
			
			return Mx[TargetSymbolIndex][Mx[TargetSymbolIndex].Length - 1];
		}
		
		public Matrix (Gram.Unterminal[] unterminals, Expression[][] Mx, bool isLeft)
		{
			Unterminals = unterminals;
			this.Mx = Mx;
			this.IsLeft = isLeft;
		}

		public Matrix (Machine.NAutomaton automaton, bool isLeft)
		{
			this.IsLeft = isLeft;

			if (isLeft) 
			{
				Machine.NStatus[] statuses = automaton.Statuses;

				Unterminals = new Gram.Unterminal[statuses.Length + 1];

				int max = 1;

				for (int i = 0; i < statuses.Length; i++)
				{
					Unterminals [i] = Gram.Unterminal.GetInstance (statuses [i].Number.Value);
					if (max < Unterminals[i].Number)
						max = Unterminals[i].Number;
				}

				max++;

				Unterminals[Unterminals.Length - 1] = Gram.Unterminal.GetInstance(max);

				Mx = new Expression[Unterminals.Length][];
				
				for (int i = 0; i < Unterminals.Length; i++)
					Mx [i] = new Expression[Unterminals.Length + 1];

				for (int i = 0; i < Mx.Length - 1; i++)
				{
					for (int j = 0; j < Mx[i].Length - 2; j++)
					{
						Alter alter = new Alter();

						foreach (Machine.NTransitionFunc func in automaton.Functions)
						{
							if (statuses[j].CompareTo(func.OldStatus) == 0 && statuses[i].CompareTo(func.NewStatus) == 0)
								alter.Expressions.Add(new Symbol(func.Symbol.Value));
						}

						if (alter.Expressions.Count == 0)
							Mx[i][j] = null;
						else
							Mx[i][j] = alter.Optimize();
					}
				}

				TargetSymbolIndex = Unterminals.Length - 1;

				for (int i = 0; i < statuses.Length; i++)
				{
					if (automaton.EndStatuses.BinarySearch(statuses[i]) >= 0)
					{
						Mx[Mx.Length - 1][i] = new Empty();
						Mx[i][Mx[i].Length - 1] = new Empty();
					}
				}
			}
			else 
			{
				Machine.NStatus[] statuses = automaton.Statuses;
				Unterminals = new Gram.Unterminal[statuses.Length];

				for (int i = 0; i < statuses.Length; i++) {
					Unterminals [i] = Gram.Unterminal.GetInstance (statuses [i].Number.Value);
					if (automaton.InitialStatus.CompareTo (statuses [i]) == 0)
						TargetSymbolIndex = i;
				}

				Mx = new Expression[Unterminals.Length][];
				
				for (int i = 0; i < Unterminals.Length; i++)
					Mx [i] = new Expression[Unterminals.Length + 1];

				for (int i = 0; i < Mx.Length; i++) {
					if (automaton.EndStatuses.BinarySearch (statuses [i]) >= 0)
						Mx [i] [Mx [i].Length - 1] = new Empty ();
				}

				for (int i = 0; i < Mx.Length; i++)
				{
					for (int j = 0; j < Mx[i].Length - 1; j++)
					{
						Alter alter = new Alter();

						foreach (Machine.NTransitionFunc func in automaton.Functions)
						{
							if (statuses[i].CompareTo(func.OldStatus) == 0 && statuses[j].CompareTo(func.NewStatus) == 0)
								alter.Expressions.Add(new Symbol(func.Symbol.Value));
						}

						if (alter.Expressions.Count == 0)
							Mx[i][j] = null;
						else
							Mx[i][j] = alter.Optimize();
					}
				}
			}
		}
		
		public Matrix (Gram.Grammar g)
		{
			Unterminals = g.DeepUnterminals;
			
			TargetSymbolIndex = Array.BinarySearch<Gram.Unterminal>(Unterminals,g.TargetSymbol);
			
			Mx = new Expression[Unterminals.Length][];
			
			for (int i = 0; i < Unterminals.Length; i++)
				Mx[i] = new Expression[Unterminals.Length + 1];
			
			IsLeft = g.IsLeft;
			
			foreach (Gram.Rule rule in g.Rules)
			{
				Alter[] alters = new Alter[Unterminals.Length + 1];								
				foreach (Gram.Chain chain in rule.Chains)
				{
					int colNum;
					
					switch (chain.Symbols.Count)
					{
						case 0:
							if (alters[alters.Length - 1] == null)
								alters[alters.Length - 1] = new Alter();
						
							alters[alters.Length - 1].Expressions.Add(new Empty());
							break;
						case 1:
							if (chain.Symbols[0] is Gram.Unterminal)
							{
								Gram.Unterminal u = (Gram.Unterminal)chain.Symbols[0];
							
								colNum = Array.BinarySearch<Gram.Unterminal>(Unterminals,u);
								
								if (alters[colNum] == null)
									alters[colNum] = new Alter();
							
								alters[colNum].Expressions.Add(new Empty());
							}
							else if (chain.Symbols[0] is Gram.Terminal)
							{
								Gram.Terminal t = (Gram.Terminal)chain.Symbols[0];
							
								if (alters[alters.Length - 1] == null)
									alters[alters.Length - 1] = new Alter();
								
								alters[alters.Length - 1].Expressions.Add(new Symbol(t.Value));
							}
							break;
						case 2:
							Gram.Unterminal u = null;								
							Gram.Terminal t = null;
						
							if (chain.Symbols[0] is Gram.Unterminal)
								u = (Gram.Unterminal)chain.Symbols[0];
						
							if (chain.Symbols[0] is Gram.Terminal)
								t = (Gram.Terminal)chain.Symbols[0];
						
							if (chain.Symbols[1] is Gram.Unterminal)
								u = (Gram.Unterminal)chain.Symbols[1];
						
							if (chain.Symbols[1] is Gram.Terminal)
								t = (Gram.Terminal)chain.Symbols[1];
						
							if (u != null && t != null)
							{
								colNum = Array.BinarySearch<Gram.Unterminal>(Unterminals,u);
								
								if (alters[colNum] == null)
									alters[colNum] = new Alter();
							
								alters[colNum].Expressions.Add(new Symbol(t.Value));
							}
						
							break;
					}
				}
				
				int rowNum = Array.BinarySearch<Gram.Unterminal>(Unterminals, rule.Prerequisite);				
				
				for (int i = 0; i < alters.Length; i++)
					Mx[rowNum][i] = alters[i] == null ? null : alters[i].Optimize();
			}
		}
	}
}
