﻿using FLaG.IO.Input;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.Languages;
using FLaGLib.Data.RegExps;
using FLaGLib.Data.StateMachines;
using System;
using System.IO;
using System.Text;

namespace FLaG.IO.Output
{
    public static class TaskDescriptionExtensions
    {
        public static void Solve(this TaskDescription taskDescription, string baseTexFileName)
        {
            FileInfo baseFileInfo = new FileInfo(baseTexFileName);

            string baseFullFileName = baseFileInfo.FullName.Substring(0, baseFileInfo.FullName.Length - baseFileInfo.Extension.Length);
            string baseTexFullFileName = baseFileInfo.FullName;

            using (StreamWriter streamWriter = new StreamWriter(baseTexFullFileName, false, new UTF8Encoding(false)))
            {
                WriteProlog(streamWriter, taskDescription.Author, taskDescription.Variant);
                
                WriteBody(streamWriter, taskDescription.Language, baseFullFileName);
                
                WriteEpilog(streamWriter);
            }
        }

        private static void WriteBody(StreamWriter streamWriter, Entity language, string baseFullFileName)
        {
            Counter diagramCounter = new Counter();

            WriteTask(streamWriter, language);

            Expression expression = CheckLanguageType(streamWriter, language);

            WriteConvertToExpression(streamWriter, language, expression);

            Tuple<StateMachine, int> leftGrammarStateMachine = ConvertToStateMachine(streamWriter, diagramCounter, expression, baseFullFileName, GrammarType.Left);
            Tuple<StateMachine, int> rightGrammarStateMachine = ConvertToStateMachine(streamWriter, diagramCounter, expression, baseFullFileName, GrammarType.Right);
            Tuple<StateMachine, int> expressionStateMachine = ConvertToStateMachine(streamWriter, diagramCounter, expression, baseFullFileName);

            leftGrammarStateMachine = OptimizeStateMachine(streamWriter, diagramCounter, leftGrammarStateMachine, baseFullFileName);
            rightGrammarStateMachine = OptimizeStateMachine(streamWriter, diagramCounter, rightGrammarStateMachine, baseFullFileName);
            expressionStateMachine = OptimizeStateMachine(streamWriter, diagramCounter, expressionStateMachine, baseFullFileName);

            Expression leftGrammarExpression = ConvertToExpression(streamWriter, leftGrammarStateMachine, expression, GrammarType.Left);
            Expression rightGrammarExpression = ConvertToExpression(streamWriter, rightGrammarStateMachine, expression, GrammarType.Right);
            Expression leftStateMachineExpression = ConvertToExpression(streamWriter, expressionStateMachine, expression, GrammarType.Left);
            Expression rightStateMachineExpression = ConvertToExpression(streamWriter, expressionStateMachine, expression, GrammarType.Right);

            ConvertToEntity(streamWriter, expression, language);
        }

        private static void WriteTask(StreamWriter streamWriter, Entity language)
        {
            throw new NotImplementedException();
        }

        private static void ConvertToEntity(StreamWriter streamWriter, Expression expression, Entity language)
        {
            throw new NotImplementedException();
        }

        private static Expression ConvertToExpression(StreamWriter streamWriter, Tuple<StateMachine, int> stateMachine, Expression result, GrammarType grammarType)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> OptimizeStateMachine(StreamWriter streamWriter, Counter diagramCounter, Tuple<StateMachine, int> leftGrammarStateMachine, string baseFullFileName)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter streamWriter, Counter diagramCounter,
            Expression expression, string baseFullFileName)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter streamWriter, Counter diagramCounter,
            Expression expression, string baseFullFileName, GrammarType grammarType)
        {
            throw new NotImplementedException();
        }

        private static void WriteConvertToExpression(StreamWriter streamWriter, Entity language, Expression expression)
        {
            throw new NotImplementedException();
        }

        private static Expression CheckLanguageType(StreamWriter streamWriter, Entity language)
        {
            throw new NotImplementedException();
        }

        private static void WriteProlog(StreamWriter writer, AuthorDescription author, string variant)
        {
            writer.WriteLine(@"\documentclass[a4paper,12pt]{article}");
            writer.WriteLine(@"\usepackage{indentfirst}");
            writer.WriteLine(@"\usepackage[a4paper, includefoot, left=1.5cm, right=1.5cm, " +
                "top=2cm, bottom=2cm, headsep=1cm, footskip=1cm]{geometry}");
            writer.WriteLine(@"\usepackage{mathtools, mathtext, amssymb}");
            writer.WriteLine(@"\usepackage[T1,T2A]{fontenc}");
            writer.WriteLine(@"\usepackage{ucs}");
            writer.WriteLine(@"\usepackage[utf8x]{inputenc}");
            writer.WriteLine(@"\usepackage[english, russian]{babel}");
            writer.WriteLine(@"\usepackage{cmap}");
            writer.WriteLine(@"\usepackage{graphicx}");
            writer.WriteLine(@"\usepackage{fixltx2e}");
            writer.WriteLine(@"\usepackage{float}");
            writer.WriteLine(@"\makeatletter");
            writer.WriteLine(@"\renewcommand{\@biblabel}[1]{#1.}");
            writer.WriteLine(@"\makeatother");
            writer.WriteLine(@"\renewcommand{\theenumi}{\arabic{enumi}.}");
            writer.WriteLine(@"\renewcommand{\labelenumi}{\arabic{enumi}.} ");
            writer.WriteLine(@"\renewcommand{\theenumii}{.\arabic{enumii}.}");
            writer.WriteLine(@"\renewcommand{\labelenumii}{\arabic{enumi}.\arabic{enumii}.}");
            writer.WriteLine(@"\renewcommand{\theenumiii}{.\arabic{enumiii}.}");
            writer.WriteLine(@"\renewcommand{\labelenumiii}{\arabic{enumi}.\arabic{enumii}.\arabic{enumiii}.}");
            writer.WriteLine(@"\newcommand{\imgh}[4]{\begin{figure}[H]\center{\includegraphics[width=#1]{#2}}\caption{#3}\label{#4}\end{figure}}");
            writer.WriteLine(@"\newcommand{\subsubsubsection}[1]{\paragraph{#1}\mbox{}\par}");
            writer.WriteLine(@"\tolerance=10000");
            writer.WriteLine(@"\begin{document}");
            writer.WriteLine(@"\begin{titlepage}");
            writer.WriteLine(@"\newpage");
            writer.WriteLine();
            writer.WriteLine(@"\begin{center}");
            writer.WriteLine(@"Казанский научно-исследовательский технический университет им.\,А.\,Н.~Туполева");
            writer.WriteLine(@"\end{center}");
            writer.WriteLine();
            writer.WriteLine(@"\vspace{8em}");
            writer.WriteLine();
            writer.WriteLine(@"\begin{center}");
            writer.WriteLine(@"\Large Кафедра прикладной математики и информатики");
            writer.WriteLine(@"\end{center}");
            writer.WriteLine();
            writer.WriteLine(@"\vspace{2em}");
            writer.WriteLine();
            writer.WriteLine(@"\begin{center}");
            writer.WriteLine(@"\textsc{\textbf{Домашнее задание}}");
            writer.WriteLine(@"\end{center}");
            writer.WriteLine(@"\begin{center}");
            writer.WriteLine(@"по дисциплине");
            writer.WriteLine(@"\end{center}");
            writer.WriteLine(@"\begin{center}");
            writer.WriteLine(@"<<Теория автоматов и формальных языков>>");
            writer.WriteLine(@"\end{center}");
            writer.WriteLine(@"\begin{center}");
            writer.Write(@"Вариант № ");
            writer.WriteLineLatex(variant);
            writer.WriteLine(@"\end{center}");
            writer.WriteLine();
            writer.WriteLine(@"\vspace{6em}");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(@"\newbox{\lbox}");
            writer.Write(@"\savebox{\lbox}{\hbox{");
            writer.WriteLatex(string.Join(author.LastName,author.FirstName,author.SecondName));
            writer.WriteLine(@"}}");
            writer.WriteLine(@"\newlength{\maxl}");
            writer.WriteLine(@"\setlength{\maxl}{\wd\lbox}");
            writer.WriteLine(@"\hfill\parbox{11cm}{");
            writer.Write(@"\hspace*{5cm}\hspace*{-5cm}Выполнил:\hfill\hbox to\maxl{");
            writer.WriteLatex(author.LastName);
            writer.Write("~");
            writer.WriteLatex(author.FirstNameInitial);
            writer.Write(@".\,");
            writer.WriteLatex(author.SecondNameInitial);
            writer.Write(@".");
            writer.WriteLine(@"\hfill}\\");
            writer.WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Проверил:\hfill\hbox to\maxl{\hfill}\\");
            writer.WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Дата сдачи:\hfill\hbox to\maxl{\_\_\_\_\_\_\_\_\_\_\_\hfill}\\");
            writer.WriteLine(@"\hspace*{5cm}\hspace*{-5cm}Оценка:\hfill\hbox to\maxl{\_\_\_\_\_\_\_\_\_\_\_\hfill}\\");
            writer.WriteLine(@"\\");
            writer.Write(@"\hspace*{5cm}\hspace*{-5cm}Группа:\hfill\hbox to\maxl{");
            writer.WriteLatex(author.Group);
            writer.WriteLine(@"}\\");
            writer.WriteLine(@"}");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(@"\vspace{\fill}");
            writer.WriteLine();
            writer.WriteLine(@"\begin{center}");
            writer.Write(@"Казань \\");
            writer.WriteLine(DateTime.Now.Year);
            writer.WriteLine(@"\end{center}");
            writer.WriteLine();
            writer.WriteLine(@"\end{titlepage}");
            writer.WriteLine();
            writer.WriteLine(@"\newpage");
            writer.WriteLine(@"\setcounter{tocdepth}{4}");
            writer.WriteLine(@"\setcounter{secnumdepth}{-1}");
            writer.WriteLine(@"\newcounter{sectocnonumdepth}");
            writer.WriteLine(@"\setcounter{sectocnonumdepth}{4}");
            writer.WriteLine(@"\tableofcontents");
            writer.WriteLine();
            writer.WriteLine(@"\newpage");
        }

        private static void WriteEpilog(StreamWriter writer)
        {
            writer.WriteLine(@"\end{document}");
        }
    }
}
