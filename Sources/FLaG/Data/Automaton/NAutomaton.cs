using System;
using System.Collections.Generic;
using FLaG.Output;
using FLaG.Data.Helpers;
using Gram=FLaG.Data.Grammars;
using System.Drawing;
using FLaG.Automaton.Diagrams;
using System.Drawing.Drawing2D;

namespace FLaG.Data.Automaton
{
	class NAutomaton
	{
		public NAutomaton DeepClone ()
		{
			NAutomaton a = new NAutomaton();

			a.InitialStatus = InitialStatus;

			foreach (NTransitionFunc func in Functions)
				a.AddFunc(func);

			foreach (NStatus endStatus in EndStatuses)
				a.AddEndStatus(endStatus);

			return a;
		}

		public FLaG.Data.Automaton.NAutomaton MakeMirror (ref int LastUseNumber, ref int AddionalAutomatonsNum)
		{
			NAutomaton a = DeepClone();
			
			a.Number = ++AddionalAutomatonsNum;
						
			return a;
		}

		public void MakeNonIntersectStatusesWith (Writer writer, NAutomaton other)
		{
			if (other.Statuses.Length == 0)
				return;

			if (Statuses.Length == 0)
				return;

			int minStatus = -1;
			int maxStatus = -1;

			foreach (NStatus status in other.Statuses) 
				if (maxStatus < status.Number)
					maxStatus = status.Number.Value;

			foreach (NStatus status in Statuses) 
				if (minStatus > status.Number || minStatus < 0)
					minStatus = status.Number.Value;

			if (minStatus > maxStatus) 
				return;

			int v = maxStatus - minStatus + 1;

			writer.WriteLine (@"Переименуем состояния автомата", true);
			writer.WriteLine (@"\begin{math}");
			SaveCortege (writer);
			writer.WriteLine (@"\end{math},");
			writer.WriteLine (@"так, чтобы наименования состояний не пересекались с автоматом", true);
			writer.WriteLine (@"\begin{math}");
			other.SaveCortege (writer);
			writer.WriteLine (@"\end{math}.");

			List<NTransitionFunc> funcs = Functions;

			Functions = new List<NTransitionFunc> ();

			foreach (NTransitionFunc func in funcs) 
				AddFunc (new NTransitionFunc (new NStatus(func.OldStatus.Value, func.OldStatus.Number.Value + v), func.Symbol,
			            new NStatus (func.NewStatus.Value, func.NewStatus.Number.Value + v))
				);

			List<NStatus> endStatuses = EndStatuses;

			EndStatuses = new List<NStatus> ();

			foreach (NStatus st in endStatuses)
				AddEndStatus(new NStatus(st.Value,st.Number.Value + v));

	        InitialStatus = new NStatus(InitialStatus.Value,InitialStatus.Number.Value + v);

			writer.WriteLine (@"Таким образом, получаем автомат", true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где",true);			
			writer.WriteLine(@"\begin{math}");
			SaveQ(writer);
			writer.WriteLine(@"=");
			SaveStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- конечное множество состояний автомата,",true);
			writer.WriteLine(@"\begin{math}");
			SaveSigma(writer);
			writer.WriteLine(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- входной алфавит автомата (конечное множество допустимых входных символов),",true);
			writer.WriteLine(@"\begin{math}");
			SaveDelta(writer);
			writer.WriteLine(@"=");
			SaveFunctions(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество функций переходов,",true);
			writer.WriteLine(@"\begin{math}");
			SaveQ0(writer);
			writer.WriteLine(@"=");
			InitialStatus.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- начальное состояние автомата,",true);
			writer.WriteLine(@"\begin{math}");
			SaveS(writer);
			writer.WriteLine(@"=");
			SaveEndStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- заключительное состояние (конечное множество заключительных состояний).",true);
			writer.WriteLine();

		}

		void DrawArrow (Graphics g, PointF point, float angle)
		{
			Matrix state = g.Transform;
			
			g.TranslateTransform(point.X,point.Y);
			g.RotateTransform(angle / (float)Math.PI * 180 + 90);
			
			float r = 40;
			
			PointF[] pp = new PointF[3];
			
			pp[0] = new PointF(r / 2, (float)-(r * Math.Sqrt(3) / 2)); 
			pp[1] = new PointF(0,0); 
			pp[2] = new PointF(-r / 2, (float)-(r * Math.Sqrt(3) / 2)); 
			
			g.FillPolygon(Brushes.Black,pp);
			
			g.Transform = state;
		}
		
		private void DrawDirectedLine(Graphics g, float x1, float y1, float x2, float y2, float r, Pen pen, Font font, string text, float upped)
        {
            Matrix state = g.Transform;

            float len = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            if (len < 2.5f * r)
                return;

            g.TranslateTransform(x1, y1);

            float sin = (y1 - y2) / len;
            float cos = (x2 - x1) / len;

            Matrix matrix = new Matrix(cos, -sin, sin, cos, 0, 0);

            g.MultiplyTransform(matrix);       
			
			g.TranslateTransform(0,-upped);

            g.DrawLine(pen, 1.25f*r, 0f, len - 1.25f*r, 0f);
			
			DrawArrow(g, new PointF(len - 1.25f*r,0), (float)Math.PI);
			
			SizeF textSize = g.MeasureString(text,font);
			
			PointF pointToDraw;
			
			if (cos > 0)
				pointToDraw	= new PointF(1.25f * r + 3.0f * (len - 2.5f * r) / 4.0f - textSize.Width / 2.0f,-textSize.Height);
			else
			{
				g.TranslateTransform(len / 2.0f,0);
				g.RotateTransform(180.0f);
				g.TranslateTransform(-len / 2.0f,0);
				pointToDraw	= new PointF(1.25f * r + (len - 2.5f * r) / 4.0f - textSize.Width / 2.0f,0.0f);
			}
			
			g.DrawString(text,font,Brushes.Black,pointToDraw);

            g.Transform = state;
        }
		
		public SizeF GetStatusLabelSize(Graphics g, Font stateFont, Font subscriptFont, NStatus status)
		{
			SizeF textSize = g.MeasureString(status.Value + "",stateFont);
			
			if (status.Number.HasValue)
			{
				int val = status.Number.Value;
				
				SizeF indexSize = g.MeasureString(val.ToString(),subscriptFont);
				
				float xSize = textSize.Width + 5 + indexSize.Width;
				float ySize = Math.Max(textSize.Height, 0.6f * textSize.Height + indexSize.Height);
				
				textSize = new SizeF(xSize,ySize);
			}
			
			return textSize;
		}

		public double MakeRByFontAndStatuses (Font stateFont, Font subscriptFont, NStatus[] statuses)
		{
			double r = 0;
			
			Bitmap bitmap = new Bitmap(1,1);
			
			Graphics g = Graphics.FromImage(bitmap);
			
			foreach (NStatus status in statuses)
			{
				SizeF textSize = GetStatusLabelSize(g,stateFont,subscriptFont, status);
				
				if (r < textSize.Width)
					r = textSize.Width;
				
				if (r < textSize.Height)
					r = textSize.Height;
			}
			
			g.Dispose();
			bitmap.Dispose();
			
			return r;
		}

		public void DrawCenteredStatusName (Graphics g, Font stateFont, Font subscriptFont, RectangleF rect, NStatus status)
		{
			SizeF labelSize = GetStatusLabelSize(g,stateFont,subscriptFont,status);
			
			float centerX = (rect.Left + rect.Right) / 2;
			float centerY = (rect.Top + rect.Bottom) / 2;
			
			float leftX = centerX - labelSize.Width / 2;
			float TopY = centerY - labelSize.Height / 2;
			
			g.DrawString(status.Value + "", stateFont,Brushes.Black,new PointF(leftX,TopY));
			
			SizeF letterSize = g.MeasureString(status.Value + "",stateFont);
			
			if (status.Number.HasValue)
			{
				int v = status.Number.Value;	
				leftX += letterSize.Width + 5;
				TopY += 0.6f * letterSize.Height;
			
				g.DrawString(v.ToString(),subscriptFont,Brushes.Black,new PointF(leftX,TopY));
			}
		}
		
		public Image MakeDiagram ()
		{
			Font stateFont = new Font("Times New Roman", 100, FontStyle.Italic, GraphicsUnit.Point);
			Font transitionFont = new Font("Times New Roman", 70, FontStyle.Italic, GraphicsUnit.Point);
			Font subscriptFont = new Font("Times New Roman", 62, FontStyle.Italic, GraphicsUnit.Point);
			
			NStatus[] statuses = Statuses;
			NStatus[] endStatuses = EndStatuses.ToArray();
			
			List<Arrow> arrows = new List<Arrow>();
			
			foreach (NTransitionFunc func in Functions)
			{
				Arrow arrow = new Arrow();
				
				arrow.A = Array.BinarySearch<NStatus>(statuses, func.OldStatus);
				arrow.B = Array.BinarySearch<NStatus>(statuses, func.NewStatus);
				
				int index = arrows.BinarySearch(arrow);
				
				if (index < 0)
					arrows.Insert(~index,arrow);
				else
					arrow = arrows[index];
				
				arrow.AddSymbol(func.Symbol);
			}
			
			double r = MakeRByFontAndStatuses(stateFont,subscriptFont, statuses);
			double startStateR = r - 10.0;
			
			double alpha = 2 * Math.PI / statuses.Length;
			
			double l;
			
			if (statuses.Length > 1)
				l = r / (1 - Math.Cos(alpha)) + 2 * r;
			else
				l = 0;
			
			double sideSize = 2 * (l + 4 * r);
			
			Bitmap bitmap = new Bitmap((int)sideSize,(int)sideSize);
			
			bitmap.SetResolution(600,600);
			
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.SmoothingMode = SmoothingMode.AntiAlias;
				
				Pen pen = new Pen(Brushes.Black,5.0f);
				Pen boldPen = new Pen(Brushes.Black,10.0f);				
				
				g.TranslateTransform((float)sideSize / 2, (float)sideSize / 2);
				
				for (int i = 0; i < statuses.Length; i++)
				{
					double lc = l * Math.Cos(i * alpha) - r;
					double tc = l * Math.Sin(i * alpha) - r;
					
					Pen drawPen = Array.BinarySearch<NStatus>(endStatuses,statuses[i]) >= 0 ? boldPen : pen;
					
					RectangleF rect = new RectangleF((float)lc,(float)tc,(float)(2 * r),(float)(2 * r));
					
					g.DrawEllipse(drawPen, rect);
					
					if (InitialStatus.CompareTo(statuses[i]) == 0)
					{
						lc = l * Math.Cos(i * alpha) - startStateR;
						tc = l * Math.Sin(i * alpha) - startStateR;

						rect = new RectangleF((float)lc,(float)tc,(float)(2 * startStateR),(float)(2 * startStateR));

						g.DrawEllipse(pen,rect);
					}

					DrawCenteredStatusName(g, stateFont, subscriptFont, rect, statuses[i]);
				}
			
				foreach (Arrow arrow in arrows)
				{
					// обычная дуга
					if (arrow.A != arrow.B)
					{
						double xA = l * Math.Cos(arrow.A * alpha);
						double yA = l * Math.Sin(arrow.A * alpha);
						
						double xB = l * Math.Cos(arrow.B * alpha);
						double yB = l * Math.Sin(arrow.B * alpha);
						
						Arrow rearrow = new Arrow();
						rearrow.A = arrow.B;
						rearrow.B = arrow.A;
						
						float upped = arrows.BinarySearch(rearrow) >= 0 ? 15.0f : 0.0f;
						
						DrawDirectedLine(g, (float)xA, (float)yA, (float)xB, (float)yB, (float)r,pen,transitionFont,arrow.Text, upped);						
					}
					// петля
					else
					{
						Matrix state = g.Transform;
						
						float beta = (float)(arrow.A * alpha * 180 / Math.PI + 90);
						
						g.TranslateTransform((float)(l * Math.Cos(arrow.A * alpha)),(float)(l * Math.Sin(arrow.A * alpha)));						
						g.RotateTransform(beta);
						
						double arcTop = -r * (Math.Sqrt(3) + 1.0);
						
						g.DrawArc(pen,new RectangleF((float)-r,(float)arcTop,(float)(2*r),(float)(2*r)),-225,270);
						
						float gamma = -225 * (float)Math.PI / 180;
						
						DrawArrow(g,new PointF((float)(r * Math.Cos(gamma)),(float)(arcTop + r + r * Math.Sin(gamma))),gamma + (float)Math.PI / 2.0f);
						
						string text = arrow.Text;
						
						SizeF textSize = g.MeasureString(text,transitionFont);
						
						if (Math.Cos(beta * Math.PI / 180) >= 0.0f)
						{
							PointF pointToDraw = new PointF(-textSize.Width / 2.0f,(float)(arcTop - textSize.Height));
							
							g.DrawString(text,transitionFont,Brushes.Black,pointToDraw);
						}
						else
						{
							g.RotateTransform(180);
							
							PointF pointToDraw = new PointF(-textSize.Width / 2.0f,-(float)(arcTop));
							
							g.DrawString(text,transitionFont,Brushes.Black,pointToDraw);
						}
						
			            g.Transform = state;
					}
				}
				
				pen.Dispose();
				boldPen.Dispose();
			}
			
			transitionFont.Dispose();
			stateFont.Dispose();
			subscriptFont.Dispose();
			
			return bitmap;
		}
		
		public Gram.Grammar MakeGrammar (Writer writer, int grammarNumber, bool isLeft)
		{
			if (isLeft)
				return MakeLeftGrammar(writer,grammarNumber);
			else
				return MakeRightGrammar(writer,grammarNumber);
		}
		
		private Gram.Grammar MakeLeftGrammar(Writer writer, int grammarNumber)
		{
			Gram.Grammar g = new Gram.Grammar();
			g.Number = grammarNumber;
			g.IsLeft = true;
			
			RuleByTargetSymbolComparer ruleComparer = new RuleByTargetSymbolComparer();
			
			int max = 0;
			
			foreach (NStatus status in Statuses)
				if (status.Number != null && status.Number > max)
					max = status.Number.Value;
			
			max++;
			g.TargetSymbol = Gram.Unterminal.GetInstance(max);
			
			foreach (NTransitionFunc func in Functions)
			{
				Gram.Rule rule = new Gram.Rule();
				rule.Prerequisite = Gram.Unterminal.GetInstance(func.NewStatus.Number.Value);
				
				int index = g.Rules.BinarySearch(rule,ruleComparer);
				if (index >= 0)
					rule = g.Rules[index];
				else
					g.Rules.Insert(~index,rule);
				
				Gram.Chain chain = new Gram.Chain();
				
				chain.Symbols.Add(Gram.Unterminal.GetInstance(func.OldStatus.Number.Value));
				Gram.Terminal t = new Gram.Terminal();
				t.Value = func.Symbol.Value;
				chain.Symbols.Add(t);
				
				index = rule.Chains.BinarySearch(chain);
				if (index < 0)
					rule.Chains.Insert(~index,chain);
			}
			
			Gram.Rule r = new Gram.Rule();
			r.Prerequisite = Gram.Unterminal.GetInstance(InitialStatus.Number.Value);
			
			int ind = g.Rules.BinarySearch(r, ruleComparer);
			if (ind >= 0)
				r = g.Rules[ind];
			else
				g.Rules.Insert(~ind,r);
			
			Gram.Chain c = new Gram.Chain();
			
			ind = r.Chains.BinarySearch(c);
			if (ind < 0)
				r.Chains.Insert(~ind,c);
			
			r = new Gram.Rule();
			r.Prerequisite = g.TargetSymbol;
			
			ind = g.Rules.BinarySearch(r,ruleComparer);
			if (ind >= 0)
				r = g.Rules[ind];
			else
				g.Rules.Insert(~ind,r);

			foreach (NStatus endStatus in EndStatuses)
			{
				Gram.Chain chain = new Gram.Chain();
				chain.Symbols.Add(Gram.Unterminal.GetInstance(endStatus.Number.Value));
				
				int index = r.Chains.BinarySearch(chain);
				if (index < 0)
					r.Chains.Insert(~index,chain);
			}
			
			writer.WriteLine(@"Выполним построение леволинейной грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveCortege(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"по минимальному конечному автомату",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На первом шаге алгоритма определяем множество",true);
			writer.WriteLine(@"нетерминальных символов грамматики, которое строится",true);
			writer.WriteLine(@"на основании множества состояний автомата",true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveN(writer);
			writer.WriteLine(@"=");
			SaveQ(writer);
			writer.WriteLine(@"\cup");
			writer.WriteLine(@"\{");
			g.TargetSymbol.Save(writer,g.IsLeft);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"=");
			SaveStatuses(writer);
			writer.WriteLine(@"\cup");
			g.TargetSymbol.Save(writer,g.IsLeft);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			g.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На следующем шаге алгоритма определяем множество терминальных символов",true);
			writer.WriteLine(@"грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"которое строится из алфавита допустимых входных символов автомата",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}, т.е.");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveSigmaWithNum(writer);
			writer.WriteLine(@"=");
			SaveSigma(writer);
			writer.WriteLine(@"=");
			g.SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На третьем шаге алгоритма рассматриваем множество функций переходов автомата,",true);
			writer.WriteLine(@"а также конечные состояния автомата, строим множество правил вывода ",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"результирующей грамматики.",true);
			writer.WriteLine();
			writer.WriteLine(@"Таким образом, множество правил",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"примет следующий вид",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"=");
			g.SaveRules(writer);
			writer.WriteLine(@"\end{math}.	");
			writer.WriteLine();			
			writer.WriteLine(@"Далее определяем целевой символ ",true);
			writer.WriteLine(@"\begin{math}");
			Gram.Unterminal.GetInstance(g.Number).Save(writer,g.IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"результирующей грамматики",true);
			writer.WriteLine();						
			writer.WriteLine(@"\begin{math}");
			Gram.Unterminal.GetInstance(g.Number).Save(writer,g.IsLeft);
			writer.WriteLine(@"\equiv");
			g.TargetSymbol.Save(writer,g.IsLeft);
			writer.WriteLine(@"\end{math}.");
			
			return g;
		}
	
		private Gram.Grammar MakeRightGrammar(Writer writer, int grammarNumber)
		{
			Gram.Grammar g = new Gram.Grammar();
			g.Number = grammarNumber;
			g.IsLeft = false;
			
			RuleByTargetSymbolComparer ruleComparer = new RuleByTargetSymbolComparer();
			
			g.TargetSymbol = Gram.Unterminal.GetInstance(InitialStatus.Number.Value);
			
			foreach (NTransitionFunc func in Functions)
			{
				Gram.Rule rule = new Gram.Rule();
				rule.Prerequisite = Gram.Unterminal.GetInstance(func.OldStatus.Number.Value);
				
				int index = g.Rules.BinarySearch(rule,ruleComparer);
				if (index >= 0)
					rule = g.Rules[index];
				else
					g.Rules.Insert(~index,rule);
				
				Gram.Chain chain = new Gram.Chain();
				
				Gram.Terminal t = new Gram.Terminal();
				t.Value = func.Symbol.Value;
				chain.Symbols.Add(t);
				chain.Symbols.Add(Gram.Unterminal.GetInstance(func.NewStatus.Number.Value));
				
				index = rule.Chains.BinarySearch(chain);
				if (index < 0)
					rule.Chains.Insert(~index,chain);
			}
			
			foreach (NStatus endStatus in EndStatuses)
			{
				Gram.Rule rule = new Gram.Rule();
				rule.Prerequisite = Gram.Unterminal.GetInstance(endStatus.Number.Value);
				
				int index = g.Rules.BinarySearch(rule, ruleComparer);
				if (index >= 0)
					rule = g.Rules[index];
				else
					g.Rules.Insert(~index,rule);
				
				Gram.Chain chain = new Gram.Chain();
				
				index = rule.Chains.BinarySearch(chain);
				if (index < 0)
					rule.Chains.Insert(~index,chain);
			}
			
			writer.WriteLine(@"Выполним построение праволинейной грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveCortege(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"по минимальному конечному автомату",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На первом шаге алгоритма определяем множество",true);
			writer.WriteLine(@"нетерминальных символов грамматики, которое строится",true);
			writer.WriteLine(@"на основании множества состояний автомата",true);
			writer.WriteLine(@"\begin{math}");
			SaveM(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveN(writer);
			writer.WriteLine(@"=");
			SaveQ(writer);
			writer.WriteLine(@"=");
			writer.WriteLine(@"\{");
			g.SaveUnterminals(writer);
			writer.WriteLine(@"\}");
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На следующем шаге алгоритма определяем множество терминальных символов",true);
			writer.WriteLine(@"грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"которое строится из алфавита допустимых входных символов автомата",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}, т.е.");
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveSigmaWithNum(writer);
			writer.WriteLine(@"=");
			SaveSigma(writer);
			writer.WriteLine(@"=");
			g.SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"На третьем шаге алгоритма рассматриваем множество функций переходов автомата,",true);
			writer.WriteLine(@"а также конечные состояния автомата, строим множество правил вывода ",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"результирующей грамматики.",true);
			writer.WriteLine();
			writer.WriteLine(@"Таким образом, множество правил",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"грамматики",true);
			writer.WriteLine(@"\begin{math}");
			g.SaveG(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"примет следующий вид",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			g.SaveP(writer);
			writer.WriteLine(@"=");
			g.SaveRules(writer);
			writer.WriteLine(@"\end{math}.	");
			writer.WriteLine();			
			writer.WriteLine(@"Далее определяем целевой символ ",true);
			writer.WriteLine(@"\begin{math}");
			Gram.Unterminal.GetInstance(g.Number).Save(writer,g.IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"результирующей грамматики",true);
			writer.WriteLine();						
			writer.WriteLine(@"\begin{math}");
			Gram.Unterminal.GetInstance(g.Number).Save(writer,g.IsLeft);
			writer.WriteLine(@"\equiv");
			g.TargetSymbol.Save(writer,g.IsLeft);
			writer.WriteLine(@"\end{math}.");
			
			return g;			
		}
		
		public Gram.Grammar MakeGrammar(Writer writer, int grammarNumber)
		{
			return MakeGrammar(writer,grammarNumber,IsLeft);
		}
		
		public void Minimize(Writer writer)
		{
			writer.WriteLine(@"Выполним минимизацию состояний для построенного детерминированного автомата",true);
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"то есть определим пятерку вида",true);
			Number++;
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math}.");
			writer.WriteLine();
			writer.WriteLine(@"Алгоритм минимизации конечного автомата заключается в следующем:",true);
			writer.WriteLine(@"из автомата исключаются все недостижимые состояния; строятся классы",true);
			writer.WriteLine(@"эквивалентности автомата; классы эквивалентности состояний исходного ДКА",true);
			writer.WriteLine(@"становятся состояниями результирующего конечного автомата; множество функций",true);
			writer.WriteLine(@"переходов результирующего конечного автомата строятся на основе",true);
			writer.WriteLine(@"множества функций переходов исходного ДКА.",true);
			writer.WriteLine();
			writer.WriteLine(@"Выполним по шагам приведенный алгоритм минимизации количества состояний ДКА.",true);
			writer.WriteLine(@"На первом шаге этого алгоритма нужно выполнить удаление недостижимых состояний.",true);
			writer.WriteLine(@"Так как удаление недостижимых состояний уже было произведено, то этот шаг алгоритма",true);
			writer.WriteLine(@"мы пропускаем.",true);
			writer.WriteLine();
			writer.WriteLine(@"На следующем шаге алгоритма минимизации конечного автомата строим классы эквивалентности автомата.",true);
			writer.WriteLine(@"По определению множество классов 0-эквилентности имеет вид",true);
			
			EqualitySetCollection R = new EqualitySetCollection();
			
			NStatus[] statuses = Statuses;
			
			EqualitySet r1 = new EqualitySet();
			r1.Number = 1;
			foreach (NStatus status in statuses)
			{
				if (EndStatuses.BinarySearch(status) < 0)
					r1.AddStatus(status);
			}
			
			EqualitySet r2 = new EqualitySet();
			r2.Number = 2;
			foreach (NStatus status in EndStatuses)
				r2.AddStatus(status);
		
			R.AddEqualitySet(r1);
			R.AddEqualitySet(r2);
			R.Number = 0;
			
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			R.SaveR(writer);
			writer.WriteLine(@"=");
			R.SaveRR(writer);
			writer.WriteLine(@"=");
			R.SaveSets(writer,IsLeft);
			writer.WriteLine(@"\end{math}.");
			
			ClassEqualityComparerByGroupNum equalityComparer = new ClassEqualityComparerByGroupNum();
			
			bool somethingChanged;
			
			do
			{
				somethingChanged = false;
				EqualitySetCollection newR = new EqualitySetCollection();
				
				newR.Number = R.Number+1;
				
				writer.WriteLine();
				writer.WriteLine(@"Итак, вычисляем");
				writer.WriteLine(@"\begin{math}");
				newR.SaveR(writer);
				writer.WriteLine(@"\end{math}");				
				
				int rr = 1;
				
				foreach (EqualitySet st in R.Set)
				{
					List<ClassEqualityCollection> classEqualitySetList = new List<ClassEqualityCollection>();
					foreach (NStatus state in st.Set)
					{
						ClassEqualityCollection classEqualitySet = new ClassEqualityCollection();
						
						foreach (NTransitionFunc func in Functions)
						{
							if (func.OldStatus.CompareTo(state) != 0)
								continue;
							
							ClassEquality equality = new ClassEquality();
							equality.GroupNum = R.GetStatusGroupNum(func.NewStatus);
							
							int index = classEqualitySet.Set.BinarySearch(equality,equalityComparer);
							if (index < 0)
								classEqualitySet.Set.Insert(~index,equality);
							else
								equality = classEqualitySet.Set[index];
							
							equality.AddSymbol(func.Symbol);
						}
						
						classEqualitySet.Set.Sort();
						
						int ind = classEqualitySetList.BinarySearch(classEqualitySet);
						
						if (ind < 0)
							classEqualitySetList.Insert(~ind,classEqualitySet);
						else
							classEqualitySet = classEqualitySetList[ind];
						
						classEqualitySet.AddStatus(state);
					}					
					
					for (int i = 0 ; i < classEqualitySetList.Count; i++)
					{
						EqualitySet es = new EqualitySet();
						es.Number = i+rr;
						foreach (NStatus sss in classEqualitySetList[i].Statuses)
							es.AddStatus(sss);
						newR.AddEqualitySet(es);
						
						writer.WriteLine();
						writer.WriteLine(@"\begin{math}");
						es.SaveR(writer,newR.Number);
						writer.WriteLine(@"=");
						es.SaveSet(writer,IsLeft);
						writer.WriteLine(@"\end{math},");
						writer.WriteLine(@"---");
						
						for (int j = 0; j < classEqualitySetList[i].Set.Count; j++)
						{
							if (j != 0)	
								writer.WriteLine(@";");
							
							writer.WriteLine(@"по символам",true);
							writer.WriteLine(@"\begin{math}");
							SaveSymbols(writer,classEqualitySetList[i].Set[j].Symbols.ToArray());
							writer.WriteLine(@"\end{math},");	
							writer.WriteLine(@"переходят в класс",true);
							writer.WriteLine(@"\begin{math}");
							R.Set[classEqualitySetList[i].Set[j].GroupNum].SaveR(writer,R.Number);
							writer.Write(@"\end{math}");	
						}
						
						writer.WriteLine(@".");
						
					}
					
					rr += classEqualitySetList.Count;
				}
				
				somethingChanged = R.CompareTo(newR) != 0;
				
				writer.WriteLine();
				writer.WriteLine(@"Таким образом, множество классов " + newR.Number + "-эквивалентности",true);
				writer.WriteLine(@"примет вид",true);
				writer.WriteLine();
				writer.WriteLine(@"\begin{math}");
				newR.SaveR(writer);
				writer.WriteLine(@"=");
				newR.SaveRR(writer);
				writer.WriteLine(@"=");
				newR.SaveSets(writer,IsLeft);
				writer.WriteLine(@"\end{math}.");
				writer.WriteLine();
				
				if (somethingChanged)
				{
					writer.WriteLine(@"Видно, что множества классов ",true);
					writer.WriteLine(R.Number + "-эквивалентности и ",true);
					writer.WriteLine(newR.Number + "-эквивалентности не совпадают, значит продолжаем выполнение алгоритма. ",true);
						
					R = newR;
				}
				else
				{
					writer.WriteLine(@"Видно, что множества классов ",true);
					writer.WriteLine(R.Number + "-эквивалентности и ",true);
					writer.WriteLine(newR.Number + "-эквивалентности совпадают, значит выполнение алгоритма");
					writer.WriteLine(@"построения классов эквивалентности конечного автомата останавливается. ",true);
				}
			} while (somethingChanged);
			
			bool changed = false;
			
			foreach (EqualitySet sR in R.Set)
			{
				if (sR.Set.Count > 1)
				{
					changed = true;
					break;
				}
			}
			
			writer.WriteLine(@"Так как в результате построения множества классов эквивалентности",true);
			if (changed)
				writer.WriteLine(@"произошло объединение состояний автомата в один из классов",true);
			else
				writer.WriteLine(@"не произошло объединение состояний автомата в один из классов",true);
			writer.Write(@"\emph{n}");
			writer.WriteLine(@"-эквивалентности, то исходный автомат",true);
			if (changed)
				writer.WriteLine(@"не является минимальным.",true);
			else
				writer.WriteLine(@"является минимальным.",true);
			writer.WriteLine(@"Следовательно, конечный автомат имеет вид",true);
			
			foreach (EqualitySet sR in R.Set)
			{
				if (sR.Set.Count < 2)
					continue;
				
				NStatus status;
				
				if (sR.Set.BinarySearch(InitialStatus) >= 0)
					status = InitialStatus;
				else
					status = sR.Set[0];
					
				NTransitionFunc[] functions = Functions.ToArray();
				Functions.Clear();
				
				foreach (NTransitionFunc func in functions)
				{
					NStatus OldStatus, NewStatus;
					
					if (sR.Set.BinarySearch(func.OldStatus) >= 0)		
						OldStatus = status;
					else
						OldStatus = func.OldStatus;
					
					if (sR.Set.BinarySearch(func.NewStatus) >= 0)		
						NewStatus = status;
					else
						NewStatus = func.NewStatus;
					
					AddFunc(new NTransitionFunc(OldStatus,func.Symbol,NewStatus));
				}
				
				for (int i = 0; i < EndStatuses.Count; i++)
				{
					if (sR.Set.BinarySearch(EndStatuses[i]) >= 0)
						EndStatuses[i] = status;
				}
				
				NStatus[] endStatuses = EndStatuses.ToArray();
				EndStatuses.Clear();
				
				foreach (NStatus ssss in endStatuses)
					AddEndStatus(ssss);	
			}
			
			writer.WriteLine(@"\begin{math}");
			SaveCortege(writer);
			writer.WriteLine(@"\end{math},");
			writer.WriteLine(@"где",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveQ(writer);
			writer.WriteLine(@"=");
			SaveStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- конечное множество состояний автомата;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveSigma(writer);
			writer.WriteLine(@"=");
			SaveAlphabet(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- входной алфавит автомата (конечное множество допустимых входных символов);",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveDelta(writer);
			writer.WriteLine(@"=");
			SaveFunctions(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество функций переходов;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveQ0(writer);
			writer.WriteLine(@"=");
			InitialStatus.Save(writer,IsLeft);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- начальное состояние автомата;",true);
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			SaveS(writer);
			writer.WriteLine(@"=");
			SaveEndStatuses(writer);
			writer.WriteLine(@"\end{math}");
			writer.WriteLine(@"--- множество заключительных состояний.",true);
		}
		
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
			automaton.SaveStatuses(writer, true);
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
			writer.WriteLine(@"Выполним построение множества фукнции перехода детерминированного автомата.",true);	
			writer.WriteLine(@"В результате получаем множества вида",true);	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveDelta(writer);
			writer.WriteLine(@"=");
			automaton.SaveFunctions(writer,true);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine();
			writer.WriteLine(@"Начальным состоянием ДКА",true);	
			writer.WriteLine(@"\begin{math}");
			automaton.SaveM(writer);
			writer.WriteLine(@"\end{math}");			
			writer.WriteLine(@"будет состояние вида",true);	
			writer.WriteLine(@"\begin{math}");
			automaton.InitialStatus.Save(writer,automaton.IsLeft,automaton.ProducedFromDFA);
			writer.WriteLine(@"\end{math}.");			
			writer.WriteLine();
			writer.WriteLine(@"Множество заключительных состояний примет вид",true);	
			writer.WriteLine();
			writer.WriteLine(@"\begin{math}");
			automaton.SaveS(writer);
			writer.WriteLine(@"=");
			automaton.SaveEndStatuses(writer,true);
			writer.WriteLine(@"\end{math}");			
			
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
		
		public void SaveSymbols(Writer writer, Symbol[] symbols)
		{
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
		
		public void SaveAlphabet(Writer writer)
		{
			SaveSymbols(writer,Alphabet);
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
		
		public void SaveM(Writer writer)
		{
			writer.Write(@"{");
			writer.Write('M'.ToString(), true);
			writer.Write("_{");
			writer.Write(Number);
			writer.Write(@"}}");
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

