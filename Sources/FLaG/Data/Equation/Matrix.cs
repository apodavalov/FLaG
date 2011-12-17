using System;
using FLaG.Output;
using Gram=FLaG.Data.Grammars;
using System.Collections.Generic;

namespace FLaG.Data.Equation
{
	class Matrix
	{
		private Expression[][] Mx
		{
			get;
			set;
		}
		
		private Gram.Unterminal[] Unterminals
		{
			get;
			set;
		}
		
		public bool IsLeft
		{
			get;
			set;
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
						Mx[i][j].Save(writer);
						writer.Write(@"}");
					}
					
					if (!IsLeft && j < Mx[i].Length - 1)
						Unterminals[j].Save(writer,IsLeft);
				}
				
				writer.WriteLine(@"}\\");
			}
  			writer.WriteLine(@"\end{array} \right.");
		}

		private int[] FindWithFreeMemberOnly()
		{
			List<int> list = new List<int>();
			
			for (int i = 0; i < Mx.Length; i++)
			{
				bool isAllNull = true;
				for (int j = 0; j < Mx[i].Length - 1; j++)
					if (Mx[i][j] != null)
					{
						isAllNull = false;
						break;
					}
				
				if (isAllNull && Mx[i][Mx[i].Length - 1] != null)
					list.Add(i);
			}
			
			return list.ToArray();
		}

		private int ChooseWithMinimumFreeMember(int[] rows)
		{
			if (rows.Length == 0)
				return -1;
			
			// TODO: сделать правильный выбор самого простого выражения
			return new Random().Next(rows.Length);
		}

		private bool IterateSimply()
		{
			int[] rows = FindWithFreeMemberOnly();
			
			bool res;
			
			do
			{
				int rowNumInRows = ChooseWithMinimumFreeMember(rows);
				
				if (rowNumInRows < 0)
					return false;
				
				int row = rows[rowNumInRows];
				
				res = false;
				
				Expression expr = Mx[row][Mx[row].Length - 1];
				
				for (int i = 0; i < Mx.Length; i++)
				{
					if (Mx[i][row] != null)
					{
						res = true;
						
						Concat concat = new Concat();
						if (IsLeft)
							concat.Expressions.Add(expr.DeepClone());
						
						concat.Expressions.Add(Mx[i][row]);
						Mx[i][row] = null;
						
						if (!IsLeft)
							concat.Expressions.Add(expr.DeepClone());
						
						if (Mx[i][Mx[i].Length - 1] == null)
							Mx[i][Mx[i].Length - 1] = concat.Optimize();
						else
						{
							Alter alter = new Alter();
							alter.Expressions.Add(Mx[i][Mx[i].Length - 1]);
							alter.Expressions.Add(concat);
							Mx[i][Mx[i].Length - 1] = alter.Optimize();
						}
					}
				}
				
				if (res == false)
				{
					List<int> list = new List<int>(rows);
					list.RemoveAt(rowNumInRows);
					rows = list.ToArray();
				}
				
			} while (!res);
			
			return true;
		}

		private int[] FindWithAlphaBetaAllowed()
		{
			List<int> list = new List<int>();
			
			for (int i = 0; i < Mx.Length; i++)
				if (Mx[i][i] != null)	
					list.Add(i);
			
			return list.ToArray();
		}

		private int ChooseWithAlphaBeta(int[] rows)
		{
			if (rows.Length == 0)
				return -1;
			
			// TODO: сделать правильный выбор самого простого выражения
			return rows[new Random().Next(rows.Length)];
		}

		private bool IterateAlphaBeta()
		{
			int[] rows = FindWithAlphaBetaAllowed();
			
			int row = ChooseWithAlphaBeta(rows);
			
			if (row < 0)
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

		private int[][] FindCycles ()
		{
			List<int[]> cycles = new List<int[]>(); // циклы
			
			int[] titles = new int[Mx.Length]; // номера ячеек в исходной матрице
			 
			for (int i = 0; i < titles.Length;i++) // изначально все присутствуют
				titles[i] = i;
			
			while (titles.Length > 0)
			{
				int[] labels = new int[titles.Length]; // метки
				int[] wentFrom = new int[titles.Length]; // из какой вершины мы пришли сюда
				
				for (int i = 0; i < labels.Length; i++)
					wentFrom[i] = labels[i] = -1;
				
				int t = 0;
				
				labels[0] = t;
				
				HashSet<int> V = new HashSet<int>();
				V.Add(0);
				
				HashSet<int> newV = new HashSet<int>();
				
				do 
				{
					t++;
					foreach (int v in V)
					{
						int row = titles[v]; // определяем ряд в исходной матрице
						
						for (int i = 0; i < Mx[row].Length - 1; i++)
							if (Mx[row][i] != null)
							{
								// есть путь - получаем индекс
								int index = Array.BinarySearch<int>(titles,i);
								
								if (index >= 0)
								{
									// вершина в множестве
									if (labels[index] >= 0) 
									{
										// найден цикл - формируем и добавляем
										int[] cycle = new int[t - labels[index]];
									
										cycle[cycle.Length - 1] = titles[v];
										int top = v;									
									
										for (int j = 1; j < cycle.Length; j++)									
										{
											cycle[cycle.Length - 1 - j] = titles[wentFrom[top]];
											top = wentFrom[top];
										}
									
										cycles.Add(cycle);
									}
									else
									{
										labels[index] = t;
										wentFrom[index] = v;
										newV.Add(index);
									}
								}
							}
					}
					
					V = newV;
					newV = new HashSet<int>();
				} while (V.Count > 0);
				
				List<int> titlesNew = new List<int>(titles);
				
				for (int i = titles.Length - 1; i >= 0; i--)
					if (labels[i] >= 0)
						titlesNew.RemoveAt(i);
				
				titles = titlesNew.ToArray();
			}
			
			return cycles.ToArray();
		}

		private int[] ChooseCycleToRemove (int[][] cycles)
		{
			if (cycles.Length == 0)
				return null;
			
			int[] cycle = cycles[0];
			
			for (int i = 1; i < cycles.Length; i++)
				if (cycles[i].Length > cycle.Length)
					cycle = cycles[i];
			
			return cycle;
		}

		private bool EliminateCycle(Writer writer, ref bool first)
		{
			int[][] cycles = FindCycles();
			
			int[] cycle = ChooseCycleToRemove(cycles);
			
			if (cycle == null || cycle.Length < 2)
				return false;
			
			Expression[] rowToModify = Mx[cycle[0]];
			
			for (int i = 1; i < cycle.Length; i++)			
			{
				int rowModifyFromNumber = cycle[i];
				Expression[] rowModifyFrom = Mx[rowModifyFromNumber];
				
				Expression expr = rowToModify[rowModifyFromNumber];
				rowToModify[rowModifyFromNumber] = null;
				
				for (int j = 0; j < rowModifyFrom.Length; j++)
					if (rowModifyFrom[j] != null)
					{
						Concat concat = new Concat();
						if (IsLeft)
							concat.Expressions.Add(rowModifyFrom[j]);
						
						concat.Expressions.Add(expr.DeepClone());
						
						if (!IsLeft)
							concat.Expressions.Add(rowModifyFrom[j]);
						
						if (rowToModify[j] == null)
							rowToModify[j] = concat.Optimize();
						else
						{
							Alter alter = new Alter();
							alter.Expressions.Add(rowToModify[j]);
							alter.Expressions.Add(concat);
							rowToModify[j] = alter.Optimize();
						}
					}
				
				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				
				if (!first)
					writer.Write(@"\Rightarrow ");
				else
					first = false;
				
				Save(writer);
				writer.WriteLine(@"\end{math}");
			}
			
			IterateAlphaBeta();
			
			return true;
		}
		
		public void Solve(Writer writer)
		{
			writer.WriteLine(@"Система уравнений с регулярными коэффициентами примет следующий вид", true);
			writer.WriteLine(@"\begin{math}");
			Save(writer);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine();		
			writer.WriteLine(@"Найдем решение данной системы.", true);
			
			bool somethingChanged;
			bool first = true;
			
			do
			{
				somethingChanged = IterateSimply();
				if (!somethingChanged)
					somethingChanged = IterateAlphaBeta();
				if (!somethingChanged)
					somethingChanged = EliminateCycle(writer, ref first);
				
				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				
				if (!first)
					writer.Write(@"\Rightarrow ");
				else
					first = false;
				
				Save(writer);
				writer.WriteLine(@"\end{math}");
			} while (somethingChanged);
		}
		
		public Matrix (Gram.Grammar g)
		{
			Unterminals = g.DeepUnterminals;
			
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
