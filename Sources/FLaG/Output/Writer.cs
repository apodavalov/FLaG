using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FLaG.Data;
using FLaG.Data.Grammars;
using FLaG.Data.Automaton;
using FLaG.Data.Equation;
using System.Drawing;

namespace FLaG.Output
{
    class Writer : StreamWriter
    {
		private Lang lang;
		private Grammar LeftSidedGrammar;
		private Grammar RightSidedGrammar;
		private int FirstLeftSidedFreeNumber;
		private int FirstRightSidedFreeNumber;
		private NAutomaton nLeftAutomaton;
		private NAutomaton nRightAutomaton;
		
		private DAutomaton dLeftAutomaton;
		private DAutomaton dRightAutomaton;
		
		public string OutputFileNamePrefix
		{
			get;
			set;
		}
		
		public Writer(Stream stream, Lang lang) 
			: base(stream)
		{
			this.lang = lang;
		}
		
		public Writer(Stream stream, Encoding encoding, Lang lang) 
			: base(stream, encoding)
		{
			this.lang = lang;
		}
		
		public Writer(Stream stream, Encoding encoding, int bufferSize, Lang lang) 
			: base(stream, encoding, bufferSize)
		{
			this.lang = lang;
		}
		
		public Writer(string path, Lang lang) 
			: base(path)
		{
			this.lang = lang;
		}
	
		public Writer(string path, bool append, Lang lang) 
			: base(path, append)
		{
			this.lang = lang;
		}
		
		public Writer(string path, bool append, Encoding encoding, Lang lang) 
			: base(path, append, encoding)
		{
			this.lang = lang;
		}
		
		public Writer(string path, bool append, Encoding encoding, int bufferSize, Lang lang) 
			: base(path, append, encoding, bufferSize)
		{
			this.lang = lang;
		}
		
		public void Write(string s, bool escape)
		{
			if (escape)
				Write(EscapeForLaTeX(s));
			else
				Write(s);			
		}
		
		public void WriteLine(string s, bool escape)
		{
			if (escape)
				WriteLine(EscapeForLaTeX(s));
			else
				WriteLine(s);
		}
		
		private string EscapeForLaTeX(string s)
		{
			StringBuilder sb = new StringBuilder();
			
			foreach (char c in s)
				switch (c)
				{
					case '#':
						sb.Append(@"\#");
						break;
					case '$':
						sb.Append(@"\$");
						break;
					case '%':
						sb.Append(@"\%");
						break;				
					case '^':
						sb.Append(@"\textasciicircum{}");
						break;				
					case '&':
						sb.Append(@"\&");
						break;				
					case '_':
						sb.Append(@"\_");
						break;		
					case '{':
						sb.Append(@"\{");
						break;					
					case '}':
						sb.Append(@"\}");
						break;					
					case '~':
						sb.Append(@"\~{}");
						break;					
					case '\\':
						sb.Append(@"\textbackslash{}");
						break;									
					default:
						sb.Append(c);
						break;
				}
			
			return sb.ToString();
		}
		
        private void WriteStartDoc()
        {			
			WriteLine(@"\documentclass[a4paper,12pt]{article}");
			WriteLine(@"\usepackage{indentfirst}");
			WriteLine(@"\usepackage[a4paper, includefoot, left=3cm, right=1.5cm, " + 
				"top=2cm, bottom=2cm, headsep=1cm, footskip=1cm]{geometry}");
			WriteLine(@"\usepackage{mathtools, mathtext, amssymb}");
			WriteLine(@"\usepackage[T1,T2A]{fontenc}");			
			WriteLine(@"\usepackage{ucs}");
			WriteLine(@"\usepackage[utf8x]{inputenc}");
			WriteLine(@"\usepackage[english, russian]{babel}");
			WriteLine(@"\usepackage{cmap}");
			WriteLine(@"\usepackage{graphicx}");
			WriteLine(@"\makeatletter");
			// Заменяем библиографию с квадратных скобок на точку
			WriteLine(@"\renewcommand{\@biblabel}[1]{#1.}");				
			WriteLine(@"\makeatother");
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumi}{\arabic{enumi}.}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumi}{\arabic{enumi}.} ");			
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumii}{.\arabic{enumii}.}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumii}{\arabic{enumi}.\arabic{enumii}.}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumiii}{.\arabic{enumiii}.}");
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumiii}{\arabic{enumi}.\arabic{enumii}.\arabic{enumiii}.}");
			// Команда для вставки изображения
			WriteLine(@"\newcommand{\imgh}[4]{\begin{figure}[h]\center{\includegraphics[width=#1]{#2}}\caption{#3}\label{#4}\end{figure}}");
			WriteLine(@"\tolerance=10000");			
			WriteLine(@"\begin{document}");
			WriteLine(@"\begin{titlepage}");
			WriteLine(@"\newpage");
			WriteLine();
			WriteLine(@"\begin{center}");
			WriteLine(@"Казанский научно-исследовательский технический университет им.\,А.\,Н.~Туполева");
			WriteLine(@"\end{center}");
			WriteLine();
			WriteLine(@"\vspace{8em}");
			WriteLine();
			WriteLine(@"\begin{center}");
			WriteLine(@"\Large Кафедра прикладной математики и информатики");
			WriteLine(@"\end{center}");
			WriteLine();
			WriteLine(@"\vspace{2em}");
			WriteLine();
			WriteLine(@"\begin{center}");
			WriteLine(@"\textsc{\textbf{Трансляция языков программирования}}");
			WriteLine(@"\end{center}");
			WriteLine();
			WriteLine(@"\vspace{6em}");
			WriteLine();
			WriteLine();
			WriteLine();
			WriteLine(@"\newbox{\lbox}");
			Write(@"\savebox{\lbox}{\hbox{");
			Write(lang.LastName + " " + lang.FirstName + " " + lang.SecondName,true);
			WriteLine(@"}}");
			WriteLine(@"\newlength{\maxl}");
			WriteLine(@"\setlength{\maxl}{\wd\lbox}");
			WriteLine(@"\hfill\parbox{11cm}{");			
			string fn = lang.FirstName.Length > 0 ? lang.FirstName.Substring(0,1) : "";
			string sn = lang.SecondName.Length > 0 ? lang.SecondName.Substring(0,1) : "";			
			Write(@"\hspace*{5cm}\hspace*{-5cm}Студент:\hfill\hbox to\maxl{");
			Write(lang.LastName,true);
			Write("~");
			Write(fn,true);
			Write(@".\,");
			Write(sn,true);
			Write(@".");
			WriteLine(@"\hfill}\\");
			WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Преподаватель:\hfill\hbox to\maxl{\hfill}\\");
			WriteLine(@"\\");
			Write(@"\hspace*{5cm}\hspace*{-5cm}Группа:\hfill\hbox to\maxl{");
			Write(lang.Group,true);
			WriteLine(@"}\\");
			WriteLine(@"}");
			WriteLine();
			WriteLine();
			WriteLine(@"\vspace{\fill}");
			WriteLine();
			WriteLine(@"\begin{center}");
			WriteLine(@"Казань \\2011");
			WriteLine(@"\end{center}");			
			WriteLine();
			WriteLine(@"\end{titlepage}");
			WriteLine();
			WriteLine(@"\newpage");
			WriteLine(@"\setcounter{tocdepth}{3}");
			WriteLine(@"\setcounter{secnumdepth}{-1}");
			WriteLine(@"\newcounter{sectocnonumdepth}");
			WriteLine(@"\setcounter{sectocnonumdepth}{3}");
			WriteLine(@"\tableofcontents");
			WriteLine();
			WriteLine(@"\newpage");
        }

        public void Step1()
        {
            // TODO: реализовать шаг 1
        }
		
		private void Step2()
		{
			Write(@"\section{");
			Write("Этап 2",true);
			WriteLine(@"}");
		}		

        private void Step2_1()
        {
			Write(@"\subsection{");
			Write("Этап 2.1",true);
			WriteLine(@"}");
            WriteLine("Перед построением регулярной грамматики для регулярного", true);
            WriteLine("языка требуется, чтобы данный язык был определен с помощью" , true);
            WriteLine("регулярного выражения, которое в свою очередь представляет", true);			
            WriteLine("регулярное множество. Чтобы множество, определяющее исходный язык", true);
			Write(@"\emph{L}");
            WriteLine(", было регулярным необходимо сделать следующие эквивалентные ", true);
            WriteLine("преобразования:", true);			
			WriteLine();
			WriteLine(@"\begin{equation}\label{eq:s2b1}");
			WriteLine(@"\begin{split}");
			Write(@"L &= ");			
			lang.Save(this);
			WriteLine(@"= \\");	
			Write(@"&=");
			lang = lang.ToRegularSet();			
			lang.Save(this);
			WriteLine(@"\end{split}");
			WriteLine(@"\end{equation}");
        }
		
		private void Step2_2()
		{
			Write(@"\subsection{");
			Write("Этап 2.2",true);
			WriteLine(@"}");
			Write("Для определения регулярного множества ", true);
			Write(@"(\ref{eq:s2b1}) ");
			WriteLine("используются регулярные выражения.", true);
			Write("Представим язык ", true);
			Write(@"\emph{L} ");
			Write("с помощью регулярного выражения вида:", true);
			WriteLine(@"\begin{equation}\label{eq:s2b2}");
			Write("p = ");
			lang = lang.ToRegularExp();
			lang.SaveAsRegularExp(this,false);
			WriteLine();
			WriteLine(@"\end{equation}");
		}
		
		private void Step2_3()
		{
			List<Entity> entities = lang.MarkDeepest();
			Write(@"\subsection{");
			Write("Этап 2.3",true);
			WriteLine(@"}");
			Write("Построим праволинейную грамматику для выражения ",true);
			Write(@"(\ref{eq:s2b2})");
			WriteLine(@". Воспользуемся рекурсивным определением регулярного выражения ", true);
			WriteLine(@"для построения последовательности праволинейных грамматик для каждого ", true);
			WriteLine(@"элементарного регулярного выражения, входящих в состав выражения ", true);
			Write(@"(\ref{eq:s2b2})");
			WriteLine(@". Собственно последняя грамматика и будет являться искомой. Определим ", true);
			WriteLine(@" совокупность выражений, входящих в состав ", true);
			Write(@"(\ref{eq:s2b2})");
			Write(@".");
			WriteLine(@"\begin{equation*}");			
			lang.SaveAsRegularExp(this,true);		
			WriteLine();
			WriteLine(@"\end{equation*}");
			WriteLine();
			WriteLine(@"Построим леволинейные и праволинейные грамматики для указанных выражений.",true);
			WriteLine(@"Для всех грамматик алфавит будет представлять множество вида ",true);
			WriteLine(@"\begin{math}");
			lang.SaveAlphabet(this);
			WriteLine(@"\end{math}.");
			
			// Вычисляем грамматики для языка.
			// Все данные для вычисления грамматик в необходимом порядке очередности
			// Находятся в entities			
			
			WriteLine(@"\begin{enumerate}");
			
			int LastUseNumber = entities.Count + 1;
			int AddionalGrammarsNumber = entities.Count + 1;
			
			// Создаем леволинейные грамматики
			for (int i = 0; i < entities.Count; i++)
				entities[i].GenerateGrammar(this,true,ref LastUseNumber, ref AddionalGrammarsNumber);
			
			FirstLeftSidedFreeNumber = Math.Max(AddionalGrammarsNumber,LastUseNumber);
			
			WriteLine(@"\end{enumerate}");
			
			// FIXME: А если нет ни одной?
			LeftSidedGrammar = entities[entities.Count - 1].Grammar;
			
			// Уничтожаем сформированные грамматики 
			for (int i = 0; i < entities.Count; i++)
				entities[i].Grammar = null;			
						
			WriteLine(@"\begin{enumerate}");
			
			LastUseNumber = entities.Count + 1;
			AddionalGrammarsNumber = entities.Count + 1;
			
			// Создаем праволиненые грамматики
			for (int i = 0; i < entities.Count; i++)
				entities[i].GenerateGrammar(this,false,ref LastUseNumber,ref AddionalGrammarsNumber);	
			
			FirstRightSidedFreeNumber = Math.Max(AddionalGrammarsNumber,LastUseNumber);
			
			WriteLine(@"\end{enumerate}");
			
			// FIXME: А если нет ни одной?
			RightSidedGrammar = entities[entities.Count - 1].Grammar;
		}
		
		private void Step2_4()
		{
			Write(@"\subsection{");
			Write("Этап 2.4",true);
			WriteLine(@"}");
			
			WriteLine(@"На этом шаге производим преобразования (приведение) грамматики.",true);
			WriteLine(@"Цель этого преобразования заключается в проверке языка на пустоту,",true);
			WriteLine(@"в удалении недостижимых символов граммактики, т.е. символов, которые",true);
			WriteLine(@"не встречаются ни в одной сентенциальной форме грамматики, бесплодных",true);
			WriteLine(@"символов, для которых в грамматике нет правил вывода, пустых правил",true);
			WriteLine(@"(правил вида",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"A \rightarrow \varepsilon");
			WriteLine(@"\end{math})");
			WriteLine(@", которые дают лишний переход конечного автомата, что приводит к замедлению",true);
			WriteLine(@"алгоритма разбора цепочки, цепных правил (правил вида",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"A \rightarrow B");
			WriteLine(@"\end{math}");
			WriteLine(@"т.е. правил которые могут привести к зацикливанию алгоритма.");
		}
		
		private void Step2_4_1()
		{
			Write(@"\subsubsection{");
			Write("Этап 2.4.1",true);
			WriteLine(@"}");
			
			LeftSidedGrammar.Normalize();			
			LeftSidedGrammar.CheckLangForEmpty(this);
			RightSidedGrammar.Normalize();
			RightSidedGrammar.CheckLangForEmpty(this);
		}
		
		private bool Step2_4_2(bool isLeft, bool again)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.4.2",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			if (again)
				Write(", повтор",true);
			Write(")",true);
			WriteLine(@"}");
			
			bool changed;
			
			if (isLeft)			
				changed = LeftSidedGrammar.RemoveUnreachedSyms(this, FirstLeftSidedFreeNumber++);
			else
				changed = RightSidedGrammar.RemoveUnreachedSyms(this, FirstRightSidedFreeNumber++);
			
		
			return changed;
		}
		
		private bool Step2_4_3(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.4.3",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			bool changed;
			
			if (isLeft)	
				changed = LeftSidedGrammar.RemoveUselessSyms(this, FirstLeftSidedFreeNumber++);
			else
				changed = RightSidedGrammar.RemoveUselessSyms(this, FirstRightSidedFreeNumber++);			

			return changed;
		}
		
		private bool Step2_4_4(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.4.4",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			bool changed;
			
			if (isLeft)
				changed = LeftSidedGrammar.RemoveEmptyRules(this, FirstLeftSidedFreeNumber++);
			else
				changed = RightSidedGrammar.RemoveEmptyRules(this, FirstRightSidedFreeNumber++);
			
			return changed;
		}
		
		private bool Step2_4_5(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.4.5",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			bool changed;
			
			if (isLeft)
				changed = LeftSidedGrammar.RemoveChainRules(this, FirstLeftSidedFreeNumber++);
			else
				changed = RightSidedGrammar.RemoveChainRules(this, FirstRightSidedFreeNumber++);

			return changed;
		}
		
		private void Step2_5_1(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.5.1",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			if (isLeft)
				LeftSidedGrammar.MakeAutomatonGrammar(this,FirstLeftSidedFreeNumber++);
			else
				RightSidedGrammar.MakeAutomatonGrammar(this,FirstRightSidedFreeNumber++);		
			
			WriteLine(@"После приведения регулярной грамматики к автоматной, переходим",true);
			WriteLine(@"к построению конечного автомата для данной грамматики.",true);
		}
		
		public void StepOptimizeGrammatic(bool isLeft)
		{
			bool somethingChanged;
			
			Step2_4_2(isLeft,false);
			
			WriteLine(@"После удаления недостижимых символов следующим шагом приведения",true);
			WriteLine(@"грамматики является удаление бесплодных (бесполезных) символов.",true);		
			
			somethingChanged = Step2_4_3(isLeft);
			
			WriteLine(@"После удаления бесплодных символов следующим шагом приведения",true);
			WriteLine(@"грамматики является удаление пустых правил (или",true);
			WriteLine(@"\begin{math}\varepsilon\end{math}");
			WriteLine(@"-правил).",true);
			
			somethingChanged |=	Step2_4_4(isLeft);
			
			WriteLine(@"После удаления пустых правил следующим шагом приведения",true);
			WriteLine(@"грамматики является удаление цепных правил.",true);
			
			somethingChanged |= Step2_4_5(isLeft);
			
			if (!somethingChanged)
			{
				WriteLine(@"Итак, так как в результате приведения грамматики не произошло",true);
				WriteLine(@"ее изменение, то переходим к построению конечного автомата.",true);
			}
			else
			{
				WriteLine(@"Итак, так как в результате приведения грамматики произошло",true);
				WriteLine(@"ее изменение, то мы должны повторить алгоритм приведения снова.",true);
				Step2_4_2(isLeft,true);
				WriteLine(@"Переходим к построению конечного автомата для данной грамматики.",true);
			}		
		}
		
		private void Step2_5()
		{
			Write(@"\subsection{");
			Write("Этап 2.5",true);
			WriteLine(@"}");
			
			WriteLine(@"На этом шаге для приведенной грамматики строим конечный автомат.",true);			
		}
		
		private void Step2_5_2(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.5.2",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			Grammar grammar = isLeft ? LeftSidedGrammar : RightSidedGrammar;
			
			NAutomaton automaton = grammar.MakeAutomaton(this);
			
			if (isLeft)
				nLeftAutomaton = automaton;
			else
				nRightAutomaton = automaton;		
		}
		
		private void Step2_5_3(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.5.3",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;		
			
			string fileSuffix = isLeft ? "1l.png" : "1r.png";
			string label = isLeft ? "img:rl1" : "img:rr1";
			string caption = "Диаграмма состояний недетерминированного конечного автомата";
			
			WriteLine(@"Построим диаграмму состояний автомата",true);
			WriteLine(@"\begin{math}");
			automaton.SaveM(this);
			WriteLine(@"\end{math}.");
			WriteLine(@"Диаграмма состояний конечного автомата ---",true);
			WriteLine(@"это неупорядоченный ориентированный помеченный граф, ",true);
			WriteLine(@"вершины которого помечены именами состояний",true);
			WriteLine(@"автомата и в котором есть дуга из вершины",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"A");
			WriteLine(@"\end{math}");
			WriteLine(@"к вершине ",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"B");
			WriteLine(@"\end{math},");
			WriteLine(@"если есть такой символ",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"t \in \Sigma");
			WriteLine(@"\end{math},");			
			WriteLine(@"для которого существует функция перехода",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"\delta(A,t)=B");
			WriteLine(@"\end{math}");			
			WriteLine(@"во множестве",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"\delta");
			WriteLine(@"\end{math}");			
			WriteLine(@"конечного автомата",true);
			WriteLine(@"\begin{math}");
			automaton.SaveM(this);
			WriteLine(@"\end{math}.");
			WriteLine(@"Кроме того, эта дуга помечается списком, состоящих из всех",true);
			WriteLine(@"\begin{math}");
			WriteLine(@"t \in \Sigma");
			WriteLine(@"\end{math}, для которых есть функция перехода");			
			WriteLine(@"\begin{math}");
			WriteLine(@"\delta(A,t)=B");
			WriteLine(@"\end{math}.");		
			WriteLine();

			using (Image image = automaton.MakeDiagram())
				InsertImage(image,OutputFileNamePrefix + fileSuffix,label,caption);
			WriteLine();
			Write(@"Диаграмма состояний конечного автомата представлена на рис. ",true);
			Write(@"\ref{" + label + "}");
			WriteLine(@".",true);
		}
		
		private void Step2_6(bool isLeft)
		{
			Write(@"\subsection{");
			Write("Этап 2.6",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			WriteLine(@"На этом шаге проверяем, является ли построенный конечный автомат детерминированным.",true);
			WriteLine(@"Если он недетерминированный, то строим для него детерминированный конечный автомат (ДКА).",true);
			WriteLine(@"Один из наиболее важных результатов теории конечных автоматов состоит в том, что класс языков,",true);
			WriteLine(@"определяемых нетерминированными конечными автоматами, совпадает с классом языков, определяемых",true);
			WriteLine(@"детерминированными конечными автоматами. Это означает, что для любого недерминированного конечного",true);
			WriteLine(@"автомата (НКА) всегда можно построить детерминированный конечный автомат, определяющий тот же язык.",true);
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;
			
			bool dfa = automaton.IsDFA();
			
			WriteLine(@"Рассматривая множество функций переходов построенного конечного автомата",true);
			WriteLine(@"\begin{math}");
			automaton.SaveM(this);
			WriteLine(@"\end{math}.");
			Write(@"Видим, что автомат является ",true);
			if (dfa)
			{
				WriteLine("ДКА т.к. каждое состояние имеет ровно одну функцию перехода для каждого возможного символа.",true);
				DAutomaton newAutomaton = automaton.MakeSimpliest();
				if (isLeft)
					dLeftAutomaton = newAutomaton;
				else
					dRightAutomaton = newAutomaton;
			}
			else
				WriteLine("НКА т.к. не каждое состояние имеет ровно одну функцию перехода для каждого возможного символа.", true);
		}
	
		private void Step2_7(bool isLeft)
		{
			Write(@"\subsection{");
			Write("Этап 2.7",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;
			DAutomaton newAutomaton = automaton.MakeDeterministic(this);
			
			if (isLeft)
				dLeftAutomaton = newAutomaton;
			else
				dRightAutomaton = newAutomaton;
		}
		
		private void Step2_7_1(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.7.1",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			DAutomaton automaton = isLeft ? dLeftAutomaton : dRightAutomaton;
			NAutomaton nAutomaton = automaton.RemoveUnreachedStates(this);
			
			if (isLeft)
				nLeftAutomaton = nAutomaton;
			else
				nRightAutomaton = nAutomaton;
		}
		
		private void Step2_7_2(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.7.2",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;	
			
			string fileSuffix = isLeft ? "2l.png" : "2r.png";
			string label = isLeft ? "img:rl2" : "img:rr2";
			string caption = "Диаграмма состояний детерминированного конечного автомата";
			
			WriteLine(@"Построим диаграмму состояний детерминированного конечного автомата",true);
			WriteLine(@"\begin{math}");
			automaton.SaveM(this);
			WriteLine(@"\end{math}");
			Write(@"(см. рис. ",true);
			Write(@"\ref{" + label + "}");
			WriteLine(@").",true);
			
			using (Image image = automaton.MakeDiagram())
				InsertImage(image,OutputFileNamePrefix + fileSuffix,label,caption);
		}
		
		private void Step2_8(bool isLeft)
		{
			Write(@"\subsection{");
			Write("Этап 2.8",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;
			
			automaton.Minimize(this);
		}
		
		private void Step2_9(bool isLeft)
		{
			Write(@"\subsection{");
			Write("Этап 2.9",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;		
			
			string fileSuffix = isLeft ? "3l.png" : "3r.png";
			string label = isLeft ? "img:rl3" : "img:rr3";
			string caption = "Диаграмма состояний минимального ДКА";
			
			WriteLine(@"Построим диаграмму состояний минимального автомата",true);
			WriteLine(@"\begin{math}");
			automaton.SaveM(this);
			WriteLine(@"\end{math}");
			Write(@"(см. рис. ",true);
			Write(@"\ref{" + label + "}");
			WriteLine(@").",true);
			
			using (Image image = automaton.MakeDiagram())
				InsertImage(image,OutputFileNamePrefix + fileSuffix,label,caption);
		}

        private void WriteEndDoc()
        {
			WriteLine(@"\end{document}");
        }

		public void Step2_10 ()
		{
			Write(@"\subsection{");
			Write("Этап 2.10",true);
			WriteLine(@"}");
			WriteLine(@"Для проверки правильности построения конечного автомата выполним",true);
			WriteLine(@"обратные преобразования, то есть рассмотрим множество каких входных",true);
			WriteLine(@"цепочек допускает данный конечный автомат.",true);
		}

		public void Step2_10_1 (bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.10.1",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			NAutomaton automaton = isLeft ? nLeftAutomaton : nRightAutomaton;
			
			Grammar grammar = automaton.MakeGrammar(this, isLeft ? FirstLeftSidedFreeNumber++ : FirstRightSidedFreeNumber++);
			
			if (isLeft)
				LeftSidedGrammar = grammar;
			else
				RightSidedGrammar = grammar;
		}
		
		public void Step2_10_2(bool isLeft)
		{
			Write(@"\subsubsection{");
			Write("Этап 2.10.2",true);
			if (isLeft)
				Write(" (левосторонняя",true);
			else
				Write(" (правосторонняя",true);
			Write(")",true);
			WriteLine(@"}");
			
			Grammar g = isLeft ? LeftSidedGrammar : RightSidedGrammar;
			
			WriteLine(@"Используя теорию уравнений с регулярными коэффициентами, выполним построение",true);
			WriteLine(@"регулярного выражения для грамматики",true);
			WriteLine(@"\begin{math}");
			g.SaveG(this);
			WriteLine(@"\end{math}.");
			WriteLine();
			
			Matrix matrix = new Matrix(g);
			Expression exp = matrix.Solve(this);
			
			WriteLine();
			WriteLine(@"Таким образом, мы определили все неизвестные. Доказано, что решение для",true);
			WriteLine(@"\begin{math}");
			matrix.Unterminals[matrix.TargetSymbolIndex].Save(this,isLeft);
			WriteLine(@"\end{math}");
			WriteLine(@"будет представлять собой искомое регулярное выражение, обозначающее язык,",true);
			WriteLine(@"заданный грамматикой",true);
			WriteLine(@"\begin{math}");
			g.SaveG(this);
			WriteLine(@"\end{math}.");
			WriteLine(@"Таким образом искомое регулярное выражение примет вид",true);
			WriteLine();
			WriteLine(@"\begin{math}");
			matrix.Unterminals[matrix.TargetSymbolIndex].Save(this,isLeft);
			WriteLine(@"=");
			exp.Save(this);
			WriteLine(@"\end{math}.");
		}
		
		public void InsertImage(Image image, string fileName, string label, string caption)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			image.Save(fileInfo.FullName); 		
			int widthInInches = (int)(image.Width / image.HorizontalResolution);
			Write(@"\imgh{" + widthInInches + "in}{");
			Write(Path.Combine("Output",fileInfo.Name),true);
			Write(@"}{");
			Write(caption,true);
			Write(@"}{");
			Write(label,true);
			WriteLine(@"}");
		}
		
		public void Out()
		{
        	WriteStartDoc();
            Step1();
			Step2();
            Step2_1();
			Step2_2();				
			Step2_3();
			Step2_4();			
			Step2_4_1();
			
			StepOptimizeGrammatic(true);
			StepOptimizeGrammatic(false);
			
			Step2_5();
			
			Step2_5_1(true);
			Step2_5_1(false);
			
			Step2_5_2(true);
			Step2_5_2(false);
			
			Step2_5_3(true);
			Step2_5_3(false);			
			
			Step2_6(true);
			Step2_6(false);
			
			if (dLeftAutomaton == null)
				Step2_7(true);
			if (dRightAutomaton == null)
				Step2_7(false);
			
			Step2_7_1(true);
			Step2_7_1(false);
			
			Step2_7_2(true);
			Step2_7_2(false);
			
			Step2_8(true);
			Step2_8(false);
			
			Step2_9(true);
			Step2_9(false);			
			
			Step2_10();
			
			Step2_10_1(true);
			Step2_10_1(false);
			
			Step2_10_2(true);
			Step2_10_2(false);

            WriteEndDoc();
		}
    }
}
