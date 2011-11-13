using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FLaG.Data;

namespace FLaG.Output
{
    class Writer : StreamWriter
    {
        public static string docBookNS = "http://docbook.org/ns/docbook";
        public static string xmlNS = "http://www.w3.org/XML/1998/namespace";
        public static string mathmlNS = "http://www.w3.org/1998/Math/MathML";
		
		public Writer(Stream stream) 
			: base(stream)
		{
			
		}
		
		public Writer(Stream stream, Encoding encoding) 
			: base(stream, encoding)
		{
			
		}
		
		public Writer(Stream stream, Encoding encoding, int bufferSize) 
			: base(stream, encoding, bufferSize)
		{
			
		}
		
		public Writer(string path) 
			: base(path)
		{
			
		}
	
		public Writer(string path, bool append) 
			: base(path, append)
		{
			
		}
		
		public Writer(string path, bool append, Encoding encoding) 
			: base(path, append, encoding)
		{
			
		}
		
		public Writer(string path, bool append, Encoding encoding, int bufferSize) 
			: base(path, append, encoding, bufferSize)
		{
			
		}
		
		public void WriteEscapedString(string s)
		{
			Write(EscapeForLaTeX(s));
		}
		
		public string EscapeForLaTeX(string s)
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
		
        public void WriteStartDoc()
        {			
			WriteLine(@"\documentclass[a4paper,12pt]{article}");
			WriteLine(@"\usepackage[a4paper, includefoot, left=3cm, right=1.5cm, " + 
				"top=2cm, bottom=2cm, headsep=1cm, footskip=1cm]{geometry}");
			WriteLine(@"\usepackage{amssymb,amsfonts,amsmath,cite,enumerate,float,indentfirst}");
			WriteLine(@"\usepackage[T2A,T2D,T1]{fontenc}");			
			WriteLine(@"\usepackage{ucs}");
			WriteLine(@"\usepackage[utf8x]{inputenc}");
			WriteLine(@"\usepackage[english, russian]{babel}");
			WriteLine(@"\usepackage{cmap}");
			WriteLine(@"\setcounter{tocdepth}{2}");
			WriteLine(@"\makeatletter");
			// Заменяем библиографию с квадратных скобок на точку
			WriteLine(@"\renewcommand{\@biblabel}[1]{#1.}");				
			WriteLine(@"\makeatother");
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumi}{\arabic{enumi}}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumi}{\arabic{enumi}} ");			
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumii}{.\arabic{enumii}}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumii}{\arabic{enumi}.\arabic{enumii}.}");				
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\theenumiii}{.\arabic{enumiii}}");
			// Меняем везде перечисления на цифра.цифра
			WriteLine(@"\renewcommand{\labelenumiii}{\arabic{enumi}.\arabic{enumii}.\arabic{enumiii}.}");
			// Команда для вставки изображения
			//WriteLine(@"\newcommand{\imgh}[3]{\begin{figure}[h]\center{\includegraphics[width=#1]{#2}}\caption{#3}\label{ris:#2}\end{figure}}");
			WriteLine(@"\begin{document}");
			WriteLine(@"\setcounter{secnumdepth}{-1}");
			WriteLine(@"\newcounter{sectocnonumdepth}");
			WriteLine(@"\setcounter{sectocnonumdepth}{2}");
			WriteLine(@"\begin{titlepage}");
			WriteLine(@"\newpage");
			WriteLine();
			WriteLine(@"\begin{center}");
			WriteLine(@"Казанский научно-исследовательский технический универстит им.\,А.\,Н.~Туполева");
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
			// TODO: загружать из XML
			WriteLine(@"\savebox{\lbox}{\hbox{Подавалова Ленуза Дамировна}}"); 
			WriteLine(@"\newlength{\maxl}");
			WriteLine(@"\setlength{\maxl}{\wd\lbox}");
			WriteLine(@"\hfill\parbox{11cm}{");
			// TODO: подставлять из XML
			WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Студент:\hfill\hbox to\maxl{Подавалова~Л.\,Д.\hfill}\\");
			WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Преподаватель:\hfill\hbox to\maxl{\hfill}\\");
			WriteLine(@"\\");
			WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Группа:\hfill\hbox to\maxl{4416}\\");			
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
			WriteLine(@"\tableofcontents");
			WriteLine(@"\newpage");
        }

        public void Step1()
        {
            // TODO: реализовать шаг 1
        }

        public void Step2_1(Lang inputLang, Lang outputLang)
        {
//            WriteStartElement("section", docBookNS);
//            WriteAttributeString("id", xmlNS, "step-2-1");
//
//            WriteElementString("title", docBookNS, "Шаг 2.1");
//            WriteStartElement("para", docBookNS);
//            WriteString("Перед построением регулярной грамматики для регулярного ");
//            WriteString("языка требуется, чтобы данный язык был определен с помощью ");
//            WriteString("регулярного выражения, которое в свою очередь представляет ");
//            WriteString("регулярное множество. Чтобы множество, определяющее исходный язык ");
//            WriteElementString("emphasis", docBookNS, "L");
//            WriteString(", было регулярным необходимо сделать следующие эквивалентные ");
//            WriteString("преобразования:");
//            WriteEndElement(); // para
//
//            WriteStartElement("math","math",mathmlNS);
//			  WriteAttributeString("linebreakstyle","after");
//
//            WriteStartElement("mrow",mathmlNS);
//            WriteElementString("mi",mathmlNS,"L");
//            WriteElementString("mo", mathmlNS, "=");
//            inputLang.Save(OutputWriter);
//			  WriteStartElement("mo", mathmlNS);
//			  WriteAttributeString("linebreak","newline");
//			  WriteString("=");
//			  WriteEndElement();
//            outputLang.Save(OutputWriter);
//            WriteEndElement(); // mrow
//
//            WriteEndElement(); // math
//
//            WriteEndElement(); // section
        }

        public void WriteEndDoc()
        {
			WriteLine(@"\end{document}");
        }
    }
}
