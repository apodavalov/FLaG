using FLaG.Output;
using System;
using System.Collections.Generic;

namespace FLaG.Data.Equation
{
	class Alter : Expression
	{
		public HashSet<Expression> Expressions
		{
			get;
			private set;
		}
		
		public Alter()
		{
			Expressions = new HashSet<Expression>();
		}
		
		public override void LetBeEmpty()
		{
			foreach (Expression e in Expressions)
				e.LetBeEmpty();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Alter))
				return false;
			
			Alter o = (Alter)obj;
			
			return Expressions.SetEquals(o.Expressions);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			
			foreach (Expression e in Expressions)
				hashCode ^= e.GetHashCode();
			
			return hashCode;
		}

		public override Expression DeepClone ()
		{
			Alter alter = new Alter();
			foreach (Expression e in Expressions)
				alter.Expressions.Add(e.DeepClone());
			
			return alter;
		}

		public bool IsConcatASuperSetOfConcatB(Concat concatA, Concat concatB)
		{
			if (concatB.Expressions.Count > concatA.Expressions.Count)
				return false;
					
			int minLength = concatB.Expressions.Count;
			
			for (int i = 0; i < minLength; i++)
			{
				bool res = false;
				
				if (concatA.Expressions[i].Equals(concatB.Expressions[i]))
					res = true;	
				else if (concatA.Expressions[i] is Repeat)
				{
					Repeat repeatA = (Repeat)concatA.Expressions[i];
					
					if (!repeatA.AtLeastOne)
					{
						if (concatB.Expressions[i] is Repeat)
						{
							Repeat repeatB = (Repeat)concatB.Expressions[i];
							if (repeatA.Expression.Equals(repeatB.Expression))
								res = true;
						}
						else if (repeatA.Expression.Equals(concatB.Expressions[i]))
							res = true;
					}
					else if (repeatA.Expression.Equals(concatB.Expressions[i]))
						res = true;
				}
				
				if (!res)
					return false;
			}
			
			for (int i = minLength; i < concatA.Expressions.Count; i++)
				if (!concatA.Expressions[i].IsLetEmpty())
					return false;
			
			return true;
		}
		
		public bool IsASuperSetOfB(Expression A, Expression B)
		{
			Concat concatA;
			if (A is Concat)
				concatA = (Concat)A;
			else
			{
				concatA = new Concat();
				concatA.Expressions.Add(A);
			}
			
			Concat concatB;
			if (B is Concat)
				concatB = (Concat)B;
			else
			{
				concatB = new Concat();
				concatB.Expressions.Add(B);
			}

			int[][] m = new int[concatA.Expressions.Count + 1][];
			
			for (int i = 0; i < m.Length; i++)
				m[i] = new int[concatB.Expressions.Count + 1];
			
			for (int i = 0; i < m.Length; i++)
				m[i][0] = 0;
			
			for (int i = 0; i < m[0].Length; i++)
				m[0][i] = 0;
			
			for (int i = 1; i < m.Length; i++)
				for (int j = 1; j < m[i].Length; j++)
				{
					if (concatA.Expressions[i-1].Equals(concatB.Expressions[j-1]))
						m[i][j] = m[i-1][j-1] + 1;
					else
						m[i][j] = Math.Max(m[i][j-1], m[i-1][j]);
				}
			
			Concat common = new Concat();
			
			int x = m.Length - 1;
			int y = m[x].Length - 1;
			
			while (m[x][y] != 0)
			{
				if (concatA.Expressions[x - 1].Equals(concatB.Expressions[y - 1]))
				{
					common.Expressions.Add(concatA.Expressions[x - 1]);
					x--;
					y--;
				}
				else
				{
					if (m[x-1][y] == m[x][y])
						x--;
					else
						y--;
				}
			}
			
			common.Expressions.Reverse();
			
			int concatACounter = 0;
			int concatBCounter = 0;
			
			List<Concat> needToRemove = new List<Concat>();
			List<Concat> needToAdd = new List<Concat>();
			
			Concat needToRemoveConcat;
			Concat needToAddConcat;
			
			for (int i = 0; i < common.Expressions.Count; i++)
			{
				needToRemoveConcat = new Concat();
				needToAddConcat = new Concat();
				
				while (!common.Expressions[i].Equals(concatA.Expressions[concatACounter]))
				{
					needToRemoveConcat.Expressions.Add(concatA.Expressions[concatACounter]);
					concatACounter++;
				}
				
				while (!common.Expressions[i].Equals(concatB.Expressions[concatBCounter]))
				{
					needToAddConcat.Expressions.Add(concatB.Expressions[concatBCounter]);
					concatBCounter++;
				}
				
				if (needToRemoveConcat.Expressions.Count != 0 || needToAddConcat.Expressions.Count != 0)
				{
					needToRemove.Add(needToRemoveConcat);
					needToAdd.Add(needToAddConcat);
				}
				
				concatACounter++;
				concatBCounter++;
			}
			
			needToRemoveConcat = new Concat();
			needToAddConcat = new Concat();
			
			while (concatACounter < concatA.Expressions.Count)
			{
				needToRemoveConcat.Expressions.Add(concatA.Expressions[concatACounter]);
				concatACounter++;
			}
			
			while (concatBCounter < concatB.Expressions.Count)
			{
				needToAddConcat.Expressions.Add(concatB.Expressions[concatBCounter]);
				concatBCounter++;
			}
			
			if (needToRemoveConcat.Expressions.Count != 0 || needToAddConcat.Expressions.Count != 0)
			{
				needToRemove.Add(needToRemoveConcat);
				needToAdd.Add(needToAddConcat);
			}
			
			bool allYes = true;
			
			int count = needToRemove.Count;
			
			for (int i = 0; i < count; i++)
				if (!IsConcatASuperSetOfConcatB(needToRemove[i],needToAdd[i]))
				{
					allYes = false;
					break;
				}
			
			return allYes;
			
		}
		
		private Expression ExtractFromBrackets(Expression A, Expression B)
		{
			Concat concatA;
			if (A is Concat)
				concatA = (Concat)A;
			else
			{
				concatA = new Concat();
				concatA.Expressions.Add(A);
			}
			
			Concat concatB;
			if (B is Concat)
				concatB = (Concat)B;
			else
			{
				concatB = new Concat();
				concatB.Expressions.Add(B);
			}
			
			int left = 0; // метка слева первого символа от начала, где совпадения заканчиваются
			int right = 0; // метка справа первого символа от конца, где совпадения заканчиваются
			
			int count = Math.Min(concatA.Expressions.Count, concatB.Expressions.Count);
						
			while (left < count && concatA.Expressions[left].Equals(concatB.Expressions[left]))
				left++;
			
			while (right < count - left && concatA.Expressions[concatA.Expressions.Count - 1 - right].
				Equals(concatB.Expressions[concatB.Expressions.Count - 1 - right]))
				right++;
			
			if (left == 0 && right == 0)
				return null;
			
			Concat newConcat = new Concat();
			Concat leftConcat = new Concat();
			Alter middleAlter = new Alter();
			Concat rightConcat = new Concat();
			
			for (int i = 0; i < left; i++)
				leftConcat.Expressions.Add(concatA.Expressions[i]);
			
			for (int i = 0; i < right; i++)
				rightConcat.Expressions.Insert(0,concatB.Expressions[concatB.Expressions.Count - 1 - i]);
			
			Concat middleConcat = new Concat();
			
			for (int i = left; i < concatA.Expressions.Count - right; i++)
				middleConcat.Expressions.Add(concatA.Expressions[i]);
			
			middleAlter.Expressions.Add(middleConcat);
			
			middleConcat = new Concat();
			
			for (int i = left; i < concatB.Expressions.Count - right; i++)
				middleConcat.Expressions.Add(concatB.Expressions[i]);
			
			middleAlter.Expressions.Add(middleConcat);
			
			newConcat.Expressions.Add(leftConcat);		
			newConcat.Expressions.Add(middleAlter);		
			newConcat.Expressions.Add(rightConcat);		
			
			return newConcat.Optimize();
		}

		public override Expression Optimize ()
		{
			List<Expression> list = new List<Expression>();
			
			foreach (Expression e in Expressions)
				list.Add(e);
			
			Expressions.Clear();
			
			for (int i = 0; i < list.Count; i++)
				list[i] = list[i].Optimize();
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;			
			
				// поднимаем альтернативы
				for (int i = 0; i < list.Count; i++)
					if (list[i] is Alter)
					{
						Alter a = (Alter)list[i];
						list.RemoveAt(i);
						foreach (Expression e in a.Expressions)
							list.Insert(i++,e);
						i--;						
						somethingChanged = true;
					}
				
				bool haveEmpty = false;
				
				for (int i = 0; i < list.Count; i++)
					if (list[i].IsLetEmpty())
					{
						haveEmpty = true;
						break;
					}
				
				if (haveEmpty)
					for (int i = 0; i < list.Count; i++)
					{
						list[i].LetBeEmpty();
						list[i] = list[i].Optimize();
					}
				
				bool haveLetEmptyButNotEmpty = false;
				
				for (int i = 0; i < list.Count; i++)
					if (!(list[i] is Empty) && list[i].IsLetEmpty())
					{
						haveLetEmptyButNotEmpty = true;
						break;
					}
				
				if (haveLetEmptyButNotEmpty)
					for (int i = 0; i < list.Count; i++)
						if (list[i] is Empty)
						{
							list.RemoveAt(i);
							i--;
							somethingChanged = true;
						}
				
				bool removedSubset;
				
				do
				{
					removedSubset = false;
					for (int i = 0; i < list.Count; i++)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (i == j) continue;
							
							if (IsASuperSetOfB(list[i],list[j]))
							{
								list.RemoveAt(j);	
								j--;
								if (i > j)
									i--;
								removedSubset = true;
								somethingChanged = true;
							}
						}
					}
				} while (removedSubset);
				
				for (int i = 0; i < list.Count; i++)
					for (int j = 0; j < list.Count; j++)
					{
						if (i == j) continue;
					
						Expression e = ExtractFromBrackets(list[i],list[j]);
					
						if  (e != null)
						{
							list.RemoveAt(j);						
						
							if (i > j)
								i--;

							list.RemoveAt(i);
						
							list.Add(e);
						
							j--;
						
							somethingChanged = true;
						}
					}
				
			} while (somethingChanged);
			
			foreach (Expression e in list)
				Expressions.Add(e);

			if (list.Count == 0)
				return new Empty();
			else if (list.Count == 1)
				return list[0];
			else
				return this;				
		}

		public override void Save(Writer writer)
		{
			bool first = true;

            foreach (Expression e in Expressions)
            {
                if (!first)
					writer.Write(@" + ");
				else
					first = false;

                e.Save(writer);
            }                      
		}

		public override bool IsLetEmpty()
		{
			foreach (Expression e in Expressions)
			{
				if (e.IsLetEmpty())
					return true;
			}
			
			return false;
		}
	}
}

