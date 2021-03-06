﻿using FLaG.IO.Input;
using FLaGLib.Collections;
using FLaGLib.Data;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.Helpers;
using FLaGLib.Data.Languages;
using FLaGLib.Data.RegExps;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FLaG.IO.Output
{
    public static class TaskDescriptionExtensions
    {
        private const string _OriginalLanguageLabel = "originalLanguage";
        private const string _GeneratedLanguageLabel = "generatedLanguage";
        private const string _OriginalRegularSetLabel = "originalRegularSet";
        private const string _OriginalRegularExpressionLabel = "originalRegularExpression";
        private const string _RussianCaseIsNotSupportedMessage = "Russian case type {0} is not supported.";
        private const string _OriginalRegularExpressionExpandedLabel = "originalRegularExpressionExpanded";
        private const string _DiagramLabel = "diagram{0}";
        private const string _RegularExpressionText = "регулярное выражение";
        private const string _SectionCaptionRegexFormat = "{0} [{1}]";
        private const string _SectionCaptionGrammarFormat = "{0}";
        private static readonly CultureInfo _RussianCulture = CultureInfo.GetCultureInfo("ru-RU");

        public static void Solve(this TaskDescription taskDescription, string baseTexFileName)
        {
            FileInfo baseFileInfo = new FileInfo(baseTexFileName);

            string baseFullFileName = baseTexFileName.Substring(0, baseTexFileName.Length - baseFileInfo.Extension.Length).Replace(".","-");

            using (StreamWriter streamWriter = new StreamWriter(baseTexFileName, false, new UTF8Encoding(false)))
            {
                WriteProlog(streamWriter, taskDescription.Author, taskDescription.Variant);
                
                WriteBody(streamWriter, taskDescription.Language, baseFullFileName);
                
                WriteEpilog(streamWriter);
            }
        }

        private static void WriteBody(StreamWriter writer, Entity language, string baseFullFileName)
        {
            Counter diagramCounter = new Counter();

            WriteTask(writer, language);

            Expression expression = WriteCheckLanguageType(writer, language);

            WriteConvertToExpression(writer, expression);

            int stateMachineNumber = 1;

            Tuple<StateMachine, int> leftGrammarStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName, GrammarType.Left, stateMachineNumber++);
            Tuple<StateMachine, int> rightGrammarStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName, GrammarType.Right, stateMachineNumber++);
            Tuple<StateMachine, int> expressionStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName, stateMachineNumber++);

            leftGrammarStateMachine = OptimizeStateMachine(writer, diagramCounter, leftGrammarStateMachine, baseFullFileName, GetGrammarTypeRussianName(GrammarType.Left), stateMachineNumber);

            Expression leftGrammarExpression = ConvertToExpression(writer, leftGrammarStateMachine, expression, GrammarType.Left,
                string.Format(_RussianCulture, _SectionCaptionGrammarFormat, GetGrammarTypeRussianName(GrammarType.Left)), 1);

            rightGrammarStateMachine = OptimizeStateMachine(writer, diagramCounter, rightGrammarStateMachine, baseFullFileName, GetGrammarTypeRussianName(GrammarType.Right), leftGrammarStateMachine.Item2 + 1);

            Expression rightGrammarExpression = ConvertToExpression(writer, rightGrammarStateMachine, expression, GrammarType.Right,
                string.Format(_RussianCulture, _SectionCaptionGrammarFormat, GetGrammarTypeRussianName(GrammarType.Right)), 2);

            expressionStateMachine = OptimizeStateMachine(writer, diagramCounter, expressionStateMachine, baseFullFileName, _RegularExpressionText, rightGrammarStateMachine.Item2 + 1);

            Expression leftStateMachineExpression = ConvertToExpression(writer, expressionStateMachine, expression, GrammarType.Left, 
                string.Format(_RussianCulture, _SectionCaptionRegexFormat, GetGrammarTypeRussianName(GrammarType.Left), _RegularExpressionText),3);
            Expression rightStateMachineExpression = ConvertToExpression(writer, expressionStateMachine, expression, GrammarType.Right, 
                string.Format(_RussianCulture, _SectionCaptionRegexFormat, GetGrammarTypeRussianName(GrammarType.Right), _RegularExpressionText),4);

            ConvertToEntity(writer, expression, language);
        }

        private static void WriteTask(StreamWriter writer, Entity language)
        {
            writer.WriteLine(@"\section{Задание}");
            writer.WriteLine(@"Задан язык:");
            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _OriginalLanguageLabel);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            writer.Write(@"L &= ");
            WriteLanguage(writer, language);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.Write(@"\end{equation}");
        }

        private static void ConvertToEntity(StreamWriter writer, Expression expression, Entity language)
        {
            WriteSection(writer, @"Этап 2.11.3", subcount: 1);

            writer.Write(@"Так как регулярные выражения совпали далее не будем подразделять шаги. ");
            writer.Write(@"Для полученного регулярного выражения построим регулярное множество, которое примет следующий вид");

            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _GeneratedLanguageLabel);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            writer.Write(@"L' &= ");
            WriteLanguage(writer, language);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.WriteLine(@"\end{equation}");
            writer.WriteLine();

            WriteSection(writer, @"Этап 2.11.4", subcount: 1);

            writer.Write(@"Сравним полученное множество ");
            WriteEquationRef(writer, _GeneratedLanguageLabel);
            writer.Write(@" с исходным ");
            WriteEquationRef(writer, _OriginalLanguageLabel);
            writer.WriteLine(@". Языки задаются абсолютно одинаково. Рассматривать цепочки смысла не имеет. Языки эквивалентны.");
            writer.WriteLine();
        }

        private static Expression ConvertToExpression(StreamWriter writer, Tuple<StateMachine, int> stateMachine, Expression result, GrammarType grammarType, string sectionCaption, int grammarNumber)
        {
            WriteSection(writer, "Этап 2.11", sectionCaption);

            writer.WriteLine(@"Для проверки правильности построения конечного автомата выполним обратные преобразования, то есть рассмотрим множество входных цепочек допускает данный автомат.");

            WriteSection(writer, "Этап 2.11.1", sectionCaption, 1);

            writer.Write(@"Выполним построение ");
            writer.Write(GetGrammarTypeRussianName(grammarType, RussianCaseType.Genitive));
            writer.Write(@" грамматики ");

            writer.Write(@"\begin{math}");
            WriteGrammarTuple(writer, grammarNumber);
            writer.Write(@"\end{math}");

            writer.Write(@" по минимальному детерминированному конечному автомату ");

            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachine.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            writer.Write(@"Итак, получаем грамматику ");

            Grammar grammar = stateMachine.Item1.MakeGrammar(grammarType);

            WriteGrammarEx(writer, grammar, grammarNumber);
            writer.WriteLine(".");
            writer.WriteLine();

            WriteSection(writer, "Этап 2.11.2", sectionCaption, 1);

            writer.Write(@"Используя теорию уравнений с регулярными коэффициентами, выполним построение регулярного выражения для грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarTuple(writer, grammarNumber);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            writer.WriteLine(@"Система уравнений с регулярными коэффициентами примет следующий вид: ");

            Expression expression = grammar.MakeExpression(grammarType, bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));

            writer.Write(@"Таким образом, мы определили все неизвестные. Доказано, что решение для ");
            writer.Write(@"\begin{math}");
            WriteNonTerminal(writer, grammar.Target);
            writer.Write(@"\end{math}");
            writer.Write(@" будет представлять собой искомое выражение, обозначающее язык, заданный грамматикой ");
            writer.Write(@"\begin{math}");
            WriteGrammarTuple(writer, grammarNumber);
            writer.Write(@"\end{math}. ");
            writer.Write(@"Таким образом, искомое регулярное выражение примет следующий вид ");
            writer.Write(@"\begin{math}");
            WriteNonTerminal(writer, grammar.Target);
            writer.Write(@" = ");
            WriteExpression(writer, expression);
            writer.Write(@" = ");
            WriteExpression(writer, result);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            return result;
        }

        private static void WriteMatrix(StreamWriter writer, Matrix matrix)
        {
            writer.WriteLine(@"\begin{cases}");

            writer.WriteLine(@"\begin{array}{l l}");

            for (int i = 0; i < matrix.RowCount; i++)
            {
                writer.Write(@"{");

                WriteNonTerminal(writer, matrix.NonTerminals[i]);

                writer.Write(@" = ");

                bool first = true;

                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (matrix[i,j] == null)
                    {
                        continue;
                    }

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@" + ");
                    }

                    WriteTargetExpression(writer, matrix.GrammarType, matrix[i, j], j < matrix.ColumnCount - 1 ? matrix.NonTerminals[j] : null);
                }

                writer.WriteLine(@"}\\");
            }

            writer.WriteLine(@"\end{array}");
            writer.WriteLine(@"\end{cases}");
        }

        private static void WriteTargetExpression(StreamWriter writer, GrammarType grammarType, Expression expression, NonTerminalSymbol nonTerminalSymbol)
        {
            if (grammarType == GrammarType.Left && nonTerminalSymbol != null)
            {
                WriteNonTerminal(writer, nonTerminalSymbol);
            }

            if (expression.ExpressionType != ExpressionType.Empty || nonTerminalSymbol == null)
            {
                writer.Write(@"{");

                bool needBrackets = expression.ExpressionType == ExpressionType.BinaryUnion || expression.ExpressionType == ExpressionType.Union;

                if (needBrackets)
                {
                    writer.Write(@"(");
                }

                WriteExpression(writer, expression);
                
                if (needBrackets)
                {
                    writer.Write(@")");
                }

                writer.Write(@"}");
            }

            if (grammarType == GrammarType.Right && nonTerminalSymbol != null)
            {
                WriteNonTerminal(writer, nonTerminalSymbol);
            }
        }

        private static void OnBeginPostReport(StreamWriter writer, Matrix postReport)
        {
            writer.WriteLine(@"\begin{math}");
            WriteMatrix(writer, postReport);
            writer.WriteLine(@"\end{math}");
            writer.WriteLine();
            writer.WriteLine(@"Найдем решение данной системы.");
            writer.WriteLine();
            writer.WriteLine(@"\begin{math}");
            WriteMatrix(writer, postReport);
            writer.WriteLine(@"\end{math}");
            writer.WriteLine();
        }

        private static void OnIteratePostReport(StreamWriter writer, Matrix postReport)
        {
            writer.WriteLine(@"\begin{math}");
            writer.WriteLine(@"\Rightarrow");
            WriteMatrix(writer, postReport);
            writer.WriteLine(@"\end{math}");
            writer.WriteLine();
        }

        private static Tuple<StateMachine, int> OptimizeStateMachine(StreamWriter writer, Counter diagramCounter, Tuple<StateMachine, int> stateMachine, string baseFullFileName, string sectionCaption, int firstAvailableStateMachineNumber)
        {
            Tuple<StateMachine, int> newStateMachine;

            bool isDeterministic = CheckDeterministic(writer, stateMachine, sectionCaption);

            if (!isDeterministic)
            {
                newStateMachine = MakeDeterministic(writer, stateMachine, sectionCaption, firstAvailableStateMachineNumber++, 6);
                stateMachine = newStateMachine;
            }
            else
            {
                WriteSection(writer, string.Format(_RussianCulture, "Этап 2.{0}", 6), sectionCaption);

                writer.WriteLine("Данный этап пропускаем, так как конечный автомат является детеминированным.");
                writer.WriteLine();
            }

            newStateMachine = RemoveUnreachableStates(writer, stateMachine, sectionCaption, firstAvailableStateMachineNumber++, 7);
            stateMachine = newStateMachine;

            if (!isDeterministic)
            {
                newStateMachine = Reorganize(writer, stateMachine, firstAvailableStateMachineNumber++);
                stateMachine = newStateMachine;
            }

            WriteDiagramStep(writer, baseFullFileName, stateMachine, diagramCounter.Next(), "Этап 2.{0}", sectionCaption, 8);

            newStateMachine = Minimize(writer, stateMachine, sectionCaption, firstAvailableStateMachineNumber++, 9);
            stateMachine = newStateMachine;

            WriteDiagramStep(writer, baseFullFileName, stateMachine, diagramCounter.Next(), "Этап 2.{0}", sectionCaption, 10);

            return stateMachine;
        }

        private static Tuple<StateMachine, int> Minimize(StreamWriter writer, Tuple<StateMachine, int> stateMachine, string sectionCaption, int stateMachineNumber, int stepNumber)
        {
            WriteSection(writer, string.Format(_RussianCulture, "Этап 2.{0}", stepNumber), sectionCaption);

            writer.Write("Выполним минимизацию детерминированного конечного автомата ");
            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachine.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            StateMachine newStateMachine;

            writer.WriteLine(@"\begin{enumerate}");
            newStateMachine = stateMachine.Item1.Minimize(bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            writer.Write(@"В результате получаем следующий детерминированный конечный автомат ");
            WriteStateMachineEx(writer, newStateMachine, stateMachineNumber);
            writer.WriteLine(@".");
            writer.WriteLine(); 

            return new Tuple<StateMachine, int>(newStateMachine, stateMachineNumber);
        }

        private static void OnIteratePostReport(StreamWriter writer, MinimizingIterationPostReport postReport)
        {
            writer.Write(@"\item Вычисляем ");
            writer.Write(@"\begin{math}");
            WriteSetsOfEquivalenceSign(writer, postReport.Iteration);
            writer.Write(@"\end{math}:");

            foreach (Tuple<SetOfEquivalence,int> transition in postReport.SetsOfEquivalence.Select((value,index) => new Tuple<SetOfEquivalence, int>(value,index)))
            {
                writer.WriteLine(@"\newline");

                writer.Write(@"\begin{math}");
                WriteSetOfEquivalenceSign(writer, transition.Item2, postReport.Iteration);
                writer.Write(@" = ");
                WriteStateSet(writer, transition.Item1);
                writer.Write(@"\end{math}");
                writer.Write(@" --- ");

                bool first = true;

                foreach (SetOfEquivalenceTransition transition1 in transition.Item1.Transitions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"; ");
                    }

                    writer.Write(@"по символам ");
                    writer.Write(@"\begin{math}");
                    WriteAlphabet(writer, transition1.Symbols);
                    writer.Write(@"\end{math} ");
                    writer.Write(@"переходят в класс ");
                    writer.Write(@"\begin{math}");
                    WriteSetOfEquivalenceSign(writer, transition1.IndexOfCurrentSetOfEquivalence, postReport.Iteration - 1);
                    writer.Write(@"\end{math}");
                }

                writer.WriteLine(@".");
            }

            writer.WriteLine();

            writer.WriteLine("Таким образом, множество классов ");
            writer.WriteLatex(postReport.Iteration.ToString(_RussianCulture));
            writer.WriteLine(@"-эквивалентности примет вид: ");
            writer.Write(@"\begin{math}");
            WriteSetsOfEquivalence(writer, postReport.SetsOfEquivalence, postReport.Iteration);
            writer.WriteLine(@"\end{math}. ");
            
            writer.Write("Видно, что множество классов ");
            writer.WriteLatex((postReport.Iteration-1).ToString(_RussianCulture));
            writer.Write(@"-эквивалентности и ");
            writer.WriteLatex(postReport.Iteration.ToString(_RussianCulture));
            writer.Write(@"-эквивалентности ");

            if (postReport.IsLastIteration)
            {
                writer.WriteLine("совпадают.");
            }
            else
            {
                writer.WriteLine("не совпадают.");
            }

            writer.WriteLine();
        }

        private static void OnBeginPostReport(StreamWriter writer, MinimizingBeginPostReport postReport)
        {
            writer.Write(@"\item Множество классов ");
            writer.WriteLatex(postReport.Iteration.ToString(_RussianCulture));
            writer.WriteLine(@"-эквивалентности имеет вид: ");
            writer.WriteLine(@"\newline");
            writer.Write(@"\begin{math}");
            WriteSetsOfEquivalence(writer, postReport.SetsOfEquivalence, postReport.Iteration);
            writer.Write(@"\end{math}.");
        }

        private static void WriteSetsOfEquivalence(StreamWriter writer, SetsOfEquivalence setsOfEquivalence, int iteration)
        {
            writer.Write("R(");
            writer.WriteLatex(iteration.ToString(_RussianCulture));
            writer.Write(")");
            writer.Write(@" = \{");

            bool first = true;

            foreach (int index in setsOfEquivalence.Select((value,index) => index))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(@"\comma ");
                }

                WriteSetOfEquivalenceSign(writer, index, iteration);
            }

            writer.Write(@"\} = \{");

            first = true;
            
            foreach (SetOfEquivalence setOfEquivalence in setsOfEquivalence)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(@"\comma ");
                }

                WriteStateSet(writer, setOfEquivalence);
            }

            writer.Write(@"\}");
        }

        private static void WriteSetsOfEquivalenceSign(StreamWriter writer, int iteration)
        {
            writer.Write("{R");
            writer.Write("(");
            writer.WriteLatex(iteration.ToString(_RussianCulture));
            writer.Write(")}");
        }


        private static void WriteSetOfEquivalenceSign(StreamWriter writer, int index, int iteration)
        {
            writer.Write("{{r_{");
            writer.WriteLatex((index + 1).ToString(_RussianCulture));
            writer.Write("}}(");
            writer.WriteLatex(iteration.ToString(_RussianCulture));
            writer.Write(")}");
        }

        private static Tuple<StateMachine, int> Reorganize(StreamWriter writer, Tuple<StateMachine, int> stateMachine, int stateMachineNumber)
        {
            writer.Write(@"Для упрощения дальнейших преобразований выполним переобозначения состояний детерминированного конечного автомата ");
            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachine.Item2);
            writer.Write(@"\end{math}. ");

            StateMachine newStateMachine = stateMachine.Item1.Reorganize(map => OnStateMachineMap(writer, map));

            writer.Write(@"Итак, получаем детерминированный конечный автомат ");
            WriteStateMachineEx(writer, newStateMachine, stateMachineNumber);

            writer.WriteLine(@".");
            writer.WriteLine();

            return new Tuple<StateMachine, int>(newStateMachine, stateMachineNumber);
        }

        private static void OnStateMachineMap(StreamWriter writer, IReadOnlyDictionary<Label, Label> map)
        {
            writer.Write(@"Введем новые состояния соответствующие старым: ");

            bool first = true;

            foreach (KeyValuePair<Label, Label> stateState in map)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(@", ");
                }

                writer.Write(@"\begin{math}");
                WriteState(writer, stateState.Key);
                writer.Write(@" \equiv ");
                WriteState(writer, stateState.Value);
                writer.Write(@"\end{math}");
            }

            writer.WriteLine(".");
            writer.WriteLine();
        }

        private static Tuple<StateMachine, int> RemoveUnreachableStates(StreamWriter writer, Tuple<StateMachine, int> stateMachine, string sectionCaption, int stateMachineNumber, int stepNumber)
        {
            WriteSection(writer, string.Format(_RussianCulture, "Этап 2.{0}", stepNumber), sectionCaption);

            writer.Write("Выполним удаление недостижимых символов детерминированного конечного автомата ");

            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachine.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            StateMachine newStateMachine;

            writer.WriteLine(@"\begin{enumerate}");
            newStateMachine = stateMachine.Item1.RemoveUnreachableStates(bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();
            
            writer.Write(@"В результате получаем следующий детерминированный конечный автомат ");
            WriteStateMachineEx(writer, newStateMachine, stateMachineNumber);
            writer.WriteLine(@".");
            writer.WriteLine();

            return new Tuple<StateMachine, int>(newStateMachine, stateMachineNumber);
        }

        private static void OnIteratePostReport(StreamWriter writer, RemovingUnreachableStatesIterationPostReport postReport)
        {
            writer.Write(@"\item ");
            writer.Write(@"\begin{math}");
            WriteApproachedStateSetSign(writer, postReport.Iteration);
            writer.Write(@" = ");
            WriteStateSet(writer, postReport.CurrentApproachedStates);
            writer.Write(@"\comma ");
            WriteApproachedStateSetSign(writer, postReport.Iteration);
            writer.Write(@" \setminus ");
            WriteReachableStateSetSign(writer);
            writer.Write(@" = ");
            WriteStateSet(writer, postReport.CurrentApproachedStates);
            writer.Write(@" \setminus ");
            WriteStateSet(writer, postReport.CurrentReachableStates);
            writer.Write(@" = ");
            WriteStateSet(writer, postReport.CurrentApproachedMinusCurrentReachableStates);

            if (postReport.IsLastIteration)
            {
                writer.Write(@" = ");
                writer.Write(@"\varnothing");
            }
            else
            {
                writer.Write(@" \neq ");
                writer.Write(@"\varnothing");
                writer.Write(@"\comma ");
                WriteReachableStateSetSign(writer);
                writer.Write(@" = ");
                WriteReachableStateSetSign(writer);
                writer.Write(@" \cup ");
                WriteStateSet(writer, postReport.CurrentApproachedStates);
                writer.Write(@" = ");
                WriteStateSet(writer, postReport.CurrentReachableStates);
                writer.Write(@" \cup ");
                WriteStateSet(writer, postReport.CurrentApproachedStates);
                writer.Write(@" = ");
                WriteStateSet(writer, postReport.NextReachableStates);
            }

            writer.WriteLine(@"\end{math}.");
        }

        private static void OnBeginPostReport(StreamWriter writer, RemovingUnreachableStatesBeginPostReport postReport)
        {
            writer.Write(@"\item ");
            writer.Write(@"\begin{math}");
            WriteReachableStateSetSign(writer);
            writer.Write(@" = ");
            WriteStateSet(writer, postReport.ReachableStates);
            writer.Write(@"\comma ");
            WriteApproachedStateSetSign(writer, postReport.Iteration);
            writer.Write(@" = ");
            WriteStateSet(writer, postReport.ApproachedStates);
            writer.WriteLine(@"\end{math}.");
        }

        private static void WriteReachableStateSetSign(StreamWriter writer)
        {
            WriteSymbol(writer, "R", null);
        }

        private static void WriteApproachedStateSetSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, "P", number);
        }

        private static Tuple<StateMachine, int> MakeDeterministic(StreamWriter writer, Tuple<StateMachine, int> stateMachine, string sectionCaption, int stateMachineNumber, int stepNumber)
        {
            WriteSection(writer, string.Format(_RussianCulture, "Этап 2.{0}", stepNumber), sectionCaption);

            writer.Write("Построим для недетерминированного конечного автомата ");

            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachine.Item2);
            writer.Write(@"\end{math}");

            writer.Write(" детерминированный конечный автомат ");

            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, stateMachineNumber);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            writer.Write(@"Итак, получаем конечный автомат ");
            WriteMetaStateMachineEx(writer, stateMachine.Item1, stateMachineNumber);
            writer.WriteLine(".");
            writer.WriteLine();

            return new Tuple<StateMachine, int>(stateMachine.Item1.ConvertToDeterministicIfNot(), stateMachineNumber);
        }

        private static void WriteMetaStateMachineEx(StreamWriter writer, StateMachine stateMachine, int number)
        {
            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, number);
            writer.Write(@"\end{math}, где ");

            writer.Write(@"\begin{math}");
            WriteStateSetSign(writer, number);
            writer.Write(" = ");
            WriteMetaStateSet(writer, stateMachine.GetMetaStates());
            writer.Write(@"\end{math} --- конечное множество состояний автомата, ");

            writer.Write(@"\begin{math}");
            WriteAlphabetSign(writer, number);
            writer.Write(" = ");
            WriteAlphabet(writer, stateMachine.Alphabet);
            writer.Write(@"\end{math} --- входной алфавит автомата (конечное множество допустимых входных символов), ");

            writer.Write(@"\begin{math}");
            WriteTransitionSetSign(writer, number);
            writer.Write(" = ");
            WriteMetaTransitionSet(writer, stateMachine.GetMetaTransitions());
            writer.Write(@"\end{math} --- множество функций переходов, ");

            writer.Write(@"\begin{math}");
            WriteInitialStateSign(writer, number);
            writer.Write(" = ");
            WriteMetaState(writer, stateMachine.GetMetaInitialState());
            writer.Write(@"\end{math} --- начальное состояние автомата, \linebreak");

            writer.Write(@"\begin{math}");
            WriteFinalStateSetSign(writer, number);
            writer.Write(" = ");
            WriteMetaStateSet(writer, stateMachine.GetMetaFinalStates());
            writer.Write(@"\end{math} --- конечное множество заключительных состояний");
        }

        private static void WriteMetaStateSet(StreamWriter writer, MetaFinalState metaFinalStates)
        {
            writer.Write(@"\{");

            writer.Write(@"[");
            writer.Write(@"{q_1},");
            writer.Write(@"\dots,");
            writer.Write(@"{q_j}");

            if (metaFinalStates.OptionalStates.Count > 0)
            {
                writer.Write(@"{q_{j+1}},");
                writer.Write(@"\dots,");
                writer.Write(@"{q_{j+k}}");
            }

            writer.Write(@"]");
            writer.Write(@"\comma ");

            writer.Write(@"\{");
            writer.Write(@"q_1,");
            writer.Write(@"\dots,");
            writer.Write(@"q_j");
            writer.Write(@"\}");

            writer.Write(@" \subseteq ");

            WriteStateSet(writer,metaFinalStates.RequiredStates);

            writer.Write(@"\comma j = \overline{1,");
            writer.WriteLatex(metaFinalStates.RequiredStates.Count.ToString(_RussianCulture));
            writer.Write(@"}");

            if (metaFinalStates.OptionalStates.Count > 0)
            {
                writer.Write(@"\comma ");

                writer.Write(@"\{");
                writer.Write(@"{q_{j+1}},");
                writer.Write(@"\dots,");
                writer.Write(@"{q_{j+k}}");
                writer.Write(@"\}");

                writer.Write(@" \subseteq ");

                WriteStateSet(writer, metaFinalStates.OptionalStates);

                writer.Write(@"\comma k = \overline{0,");
                writer.WriteLatex(metaFinalStates.OptionalStates.Count.ToString(_RussianCulture));
                writer.Write(@"}");
            }

            writer.Write(@"\}");
        }

        private static void WriteMetaState(StreamWriter writer, Label metaState)
        {
            WriteLabel(writer, metaState);
        }

        private static void WriteMetaTransitionSet(StreamWriter writer, IEnumerable<MetaTransition> metaTransitions)
        {
            bool first = true;

            if (metaTransitions.Any())
            {
                writer.Write(@"\{");

                foreach (MetaTransition metaTransition in metaTransitions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\semicolon ");
                    }

                    writer.Write(@"\newline ");
                    WriteMetaTransition(writer, metaTransition);
                }

                writer.Write(@"\newline ");

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static void WriteMetaTransition(StreamWriter writer, MetaTransition metaTransition)
        {
            writer.Write(@"\delta(");
            writer.Write(@"[");

            foreach (Label state in metaTransition.CurrentRequiredStates)
            {
                WriteState(writer, state);
            }

            if (metaTransition.CurrentOptionalStates.Count > 0)
            {
                writer.Write(@"q_1,");
                writer.Write(@"\dots,");
                writer.Write(@"q_k");
            }

            writer.Write(@"]");

            writer.Write(@", ");
            WriteSymbol(writer, metaTransition.Symbol);
            writer.Write(@") = ");

            writer.Write(@"[");
            
            foreach (Label state in metaTransition.NextStates)
            {
                WriteState(writer, state);
            }

            writer.Write(@"]");

            if (metaTransition.CurrentOptionalStates.Count > 0)
            {
                writer.Write(@",\newline ");
                writer.Write(@"\{");
                writer.Write(@"q_1,");
                writer.Write(@"\dots,");
                writer.Write(@"q_k");
                writer.Write(@"\}");

                writer.Write(@" \subseteq ");
                WriteStateSet(writer, metaTransition.CurrentOptionalStates);
                writer.Write(@"\comma k = \overline{0,");
                writer.WriteLatex(metaTransition.CurrentOptionalStates.Count.ToString(_RussianCulture));
                writer.Write(@"}");
            }
        }

        private static void WriteMetaStateSet(StreamWriter writer, IEnumerable<Label> states)
        {
            int count = states.Count();

            if (count > 0)
            {
                writer.Write(@"\{");
                writer.Write(@"[");
                writer.Write(@"q_1,");
                writer.Write(@"\dots,");
                writer.Write(@"q_k");
                writer.Write(@"]\comma ");
                writer.Write(@"\{");
                writer.Write(@"q_1,");
                writer.Write(@"\dots,");
                writer.Write(@"q_k");
                writer.Write(@"\}");
                writer.Write(@" \subseteq ");
                WriteStateSet(writer, states);
                writer.Write(@"\comma k = \overline{1,");
                writer.WriteLatex(states.Count().ToString(_RussianCulture));
                writer.Write(@"}");
                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static bool CheckDeterministic(StreamWriter writer, Tuple<StateMachine, int> stateMachine, string sectionCaption)
        {
            WriteSection(writer, "Этап 2.5", sectionCaption);
            writer.Write("На этом шаге проверяем, являются ли построенный конечный автомат детерминированным. ");
            bool deterministic = stateMachine.Item1.IsDeterministic();

            writer.Write("Рассматриваем множество функций переходов построенного конечного автомата ");
            writer.Write(@"\begin{math}");
            WriteStateMachineSign(writer, stateMachine.Item2);
            writer.Write(@"\end{math}. ");
            
            if (deterministic)
            {
                writer.WriteLine(@"Видим, что автомат является детерминированным, т.к. каждое состояние имеет ровно одну функцию перехода для каждого возможного символа.");
            }
            else
            {
                writer.WriteLine(@"Видим, что автомат является недетерминированным, т.к. не каждое состояние имеет ровно одну функцию перехода для каждого возможного символа.");
            }

            writer.WriteLine();

            return deterministic;
            
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter writer, Counter diagramCounter,
            Expression expression, string baseFullFileName, int number)
        {
            WriteRegexSection(writer, "Этап 2.3");

            writer.WriteLine("Данный этап при построении автомата из регулярного выражения пропускаем.");
            writer.WriteLine();

            WriteRegexSection(writer, "Этап 2.4");
            writer.Write("В процессе построения будем использовать изолированную от остальных пунктов нумерацию конечных автоматов. Построим ");
            writer.Write("конечный автомат для выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Воспользуемся рекурсивным определением регулярного выражения для построения последовательности ");
            writer.Write("конечных автоматов для каждого элементарного выражения, входящих в состав выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Собственно последний конечный автомат и будет являться искомым. Определим совокупность выражений, ");
            writer.Write("входящих в состав исходного выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.WriteLine();
            writer.WriteLine();
            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _OriginalRegularExpressionExpandedLabel);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            WriteExpressionEx(writer, expression);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.WriteLine(@"\end{equation}");
            writer.WriteLine();
            writer.Write(@"Построим ");
            writer.Write("конечные автоматы для указанных выражений. Каждую грамматику будем нумеровать по номеру выражения, ");
            writer.WriteLine("для которого строится данная грамматика.");
            writer.WriteLine();

            writer.WriteLine(@"\begin{enumerate}");

            int stateMachineNumber = -1;

            StateMachine stateMachine = expression.MakeStateMachine(m => stateMachineNumber = OnStateMachinePostReport(writer, m, diagramCounter, baseFullFileName));

            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            writer.Write("Далее по тексту (в следующих пунктах) будем обозначать полученный конечный автомат ");
            writer.Write(@"\begin{math}");
            WriteStateMachineSign(writer, stateMachineNumber);
            writer.Write(@"\end{math}");
            writer.Write(@" следующим образом: ");

            writer.Write(@"\begin{math}");
            WriteStateMachineSign(writer, number);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            return new Tuple<StateMachine, int>(stateMachine, number);
        }

        private static int OnStateMachinePostReport(StreamWriter writer, StateMachinePostReport stateMachinePostReport, Counter diagramCounter, string baseFullFileName)
        {
            writer.Write(@"\item ");
            writer.Write("Для выражения вида ");
            writer.Write(@"\begin{math}");
            WriteExpression(writer, stateMachinePostReport.New.Expression, true);
            writer.Write(@"\end{math}, являющегося ");
            writer.WriteLatex(GetExpressionTypeRussianName(stateMachinePostReport.New.Expression.ExpressionType, RussianCaseType.Ablative));

            if (stateMachinePostReport.Dependencies.Count > 0)
            {
                if (stateMachinePostReport.Dependencies.Count < 2)
                {
                    writer.Write(" выражения с построенным конечным автоматом ");
                }
                else
                {
                    writer.Write(" выражений с построенными конечными автоматами ");
                }

                bool first = true;

                foreach (StateMachineExpressionWithOriginal dependency in stateMachinePostReport.Dependencies)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(", ");
                    }

                    writer.Write(@"\begin{math}");
                    WriteStateMachineSign(writer, dependency.StateMachineExpression.Number);
                    writer.Write(@"\end{math}");

                    if (dependency.OriginalStateMachineExpression != null)
                    {
                        writer.Write(" (построен из конечного автомата ");
                        writer.Write(@"\begin{math}");
                        WriteStateMachineSign(writer, dependency.OriginalStateMachineExpression.Number);
                        writer.Write(@"\end{math}");
                        writer.Write(@" путем соответствующей замены индексов)");
                    }
                }
            }

            writer.Write(", построим конечный автомат ");
            WriteStateMachineEx(writer, stateMachinePostReport.New.StateMachine, stateMachinePostReport.New.Number);

            int number = diagramCounter.Next();

            writer.Write(". Диаграмма состояний конечного автомата представлена на рис. ");
            WriteDiagramRef(writer, number);
            writer.WriteLine(".");
            writer.WriteLine();

            using (Image image = stateMachinePostReport.New.StateMachine.DrawDiagram())
            {
                WriteDiagram(writer, image, baseFullFileName, number, "Диаграмма состояний конечного автомата");
            }

            writer.WriteLine();

            return stateMachinePostReport.New.Number;
        }

        private static void WriteDiagramStep(StreamWriter writer, string baseFullFileName, Tuple<StateMachine, int> stateMachine, int imageNumber,
            string stepTitlePattern, string sectionCaption, int stepNumber, int subcount = 0)
        {
            WriteSection(writer, string.Format(_RussianCulture, stepTitlePattern, stepNumber), sectionCaption, subcount);

            writer.Write("Построим диаграмму состояний конечного автомата ");
            writer.Write(@"\begin{math}");
            WriteStateMachineSign(writer, stateMachine.Item2);
            writer.Write(@"\end{math} (см. рис. ");
            WriteDiagramRef(writer, imageNumber);
            writer.WriteLine(@").");
            writer.WriteLine();

            using (Image image = stateMachine.Item1.DrawDiagram())
            {
                WriteDiagram(writer, image, baseFullFileName, imageNumber, "Диаграмма состояний конечного автомата.");
            }

            writer.WriteLine();
        }

        private static void WriteDiagram(StreamWriter writer, Image image, string baseFullFileName, int number, string caption)
        {
            WriteImage(writer, image, string.Format(CultureInfo.InvariantCulture, "{0}-{1:00}.png", baseFullFileName, number), string.Format(CultureInfo.InvariantCulture, _DiagramLabel, number), caption);
        }

        private static void WriteImage(StreamWriter writer, Image image, string fileName, string label, string caption)
        {
            image.Save(fileName);

            int widthInMm = (int)(image.Width / image.HorizontalResolution * 25.4f);

            if (widthInMm > 160)
            {
                widthInMm = 160;
            }

            writer.Write(@"\imgh{" + widthInMm + "mm}{");
            writer.WriteLatex(fileName);
            writer.Write(@"}{");
            writer.WriteLatex(caption);
            writer.Write(@"}{img:");
            writer.WriteLatex(label);
            writer.Write(@"}");
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter writer, Counter diagramCounter,
            Expression expression, string baseFullFileName, GrammarType grammarType, int number)
        {
            WriteSection(writer, grammarType, "Этап 2.3");

            Tuple<Grammar,int> grammar = ConvertToGrammar(writer, expression, grammarType);

            grammar = OptimizeGrammar(writer, grammar, grammarType);

            StateMachine stateMachine = grammar.Item1.MakeStateMachine(grammarType);

            WriteSection(writer, grammarType, "Этап 2.3.3.2", 2);

            writer.Write("Выполним построение конечного автомата ");
            writer.Write(@"\begin{math}");
            WriteStateMachineSign(writer, number);
            writer.Write(@"\end{math}");
            writer.Write(@" для автоматной грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}. Обозначения для полученных выше грамматик использовать больше не будем. ");
            writer.WriteLine();
            writer.Write(@"Строим конечный автомат ");

            WriteStateMachineEx(writer, stateMachine, number);

            writer.WriteLine(".");
            writer.WriteLine();

            WriteDiagramStep(writer, baseFullFileName, new Tuple<StateMachine, int>(stateMachine, number), diagramCounter.Next(), "Этап 2.3.3.{0}", GetGrammarTypeRussianName(grammarType), 3, 2);

            WriteSection(writer, grammarType, "Этап 2.4");

            writer.WriteLine("Данный этап при построении автомата из грамматики пропускаем.");
            writer.WriteLine();

            return new Tuple<StateMachine, int>(stateMachine, number);
        }

        private static void WriteStateMachineEx(StreamWriter writer, StateMachine stateMachine, int number)
        {
            writer.Write(@"\begin{math}");
            WriteStateMachineTuple(writer, number);
            writer.Write(@"\end{math}, где ");

            writer.Write(@"\begin{math}");
            WriteStateSetSign(writer, number);
            writer.Write(" = ");
            WriteStateSet(writer, stateMachine.States);
            writer.Write(@"\end{math} --- конечное множество состояний автомата, ");

            writer.Write(@"\begin{math}");
            WriteAlphabetSign(writer, number);
            writer.Write(" = ");
            WriteAlphabet(writer, stateMachine.Alphabet);
            writer.Write(@"\end{math} --- входной алфавит автомата (конечное множество допустимых входных символов), ");

            writer.Write(@"\begin{math}");
            WriteTransitionSetSign(writer, number);
            writer.Write(" = ");
            WriteTransitionSet(writer, stateMachine.Transitions);
            writer.Write(@"\end{math} --- множество функций переходов, ");

            writer.Write(@"\begin{math}");
            WriteInitialStateSign(writer, number);
            writer.Write(" = ");
            WriteState(writer, stateMachine.InitialState);
            writer.Write(@"\end{math} --- начальное состояние автомата, ");

            writer.Write(@"\begin{math}");
            WriteFinalStateSetSign(writer, number);
            writer.Write(" = ");
            WriteStateSet(writer, stateMachine.FinalStates);
            writer.Write(@"\end{math} --- конечное множество заключительных состояний");
        }

        private static void WriteFinalStateSetSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, "F", number);
        }

        private static void WriteState(StreamWriter writer, Label state)
        {
            WriteLabel(writer, state);
        }

        private static void WriteInitialStateSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, "q", number);
        }

        private static void WriteTransitionSet(StreamWriter writer, IEnumerable<Transition> transitions)
        {
            bool first = true;

            if (transitions.Any())
            {
                writer.Write(@"\{");

                foreach (Transition transition in transitions)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\comma ");
                    }

                    WriteTransition(writer, transition);
                }

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static void WriteTransition(StreamWriter writer, Transition transition)
        {
            writer.Write(@"\delta(");
            WriteState(writer, transition.CurrentState);
            writer.Write(@", ");
            WriteSymbol(writer, transition.Symbol);
            writer.Write(@") = ");
            WriteState(writer, transition.NextState);
        }

        private static void WriteTransitionSetSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, @"\delta", number);
        }

        private static void WriteSymbol(StreamWriter writer, char symbol)
        {
            writer.WriteLatex(symbol.ToString(_RussianCulture));
        }  

        private static void WriteAlphabet(StreamWriter writer, IEnumerable<char> alphabet)
        {
            bool first = true;

            if (alphabet.Any())
            {
                writer.Write(@"\{");

                foreach (char symbol in alphabet)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\comma ");
                    }

                    WriteSymbol(writer, symbol);
                }

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static void WriteAlphabetSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, @"\Sigma", number);
        }

        private static void WriteStateSet(StreamWriter writer, IEnumerable<Label> states)
        {
            bool first = true;

            if (states.Any())
            {
                writer.Write(@"\{");

                foreach (Label state in states)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\comma ");
                    }

                    WriteState(writer, state);
                }

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static void WriteStateSetSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, "Q", number);
        }

        private static void WriteStateMachineTuple(StreamWriter writer, int number)
        {
            WriteStateMachineSign(writer, number);
            writer.Write(@"=\left(");
            WriteStateSetSign(writer, number);
            writer.Write(", ");
            WriteAlphabetSign(writer, number);
            writer.Write(", ");
            WriteTransitionSetSign(writer, number);
            writer.Write(", ");
            WriteInitialStateSign(writer, number);
            writer.Write(", ");
            WriteFinalStateSetSign(writer, number);
            writer.Write(@"\right)");
        }

        private static void WriteStateMachineSign(StreamWriter writer, int number)
        {
            WriteSymbol(writer, "M", number);
        }

        private static Tuple<Grammar, int> OptimizeGrammar(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3.2", 1);

            writer.Write("На этом шаге производим преобразование (приведение) грамматики. ");
            writer.Write("Цель этого преобразования заключается в удалении недостижимых ");
            writer.Write("символов грамматики, т.е. символов, которые не встречаются ни в одной ");
            writer.Write("сентенциальной форме грамматики, бесплодных символов, для которых в ");
            writer.Write("грамматике нет правил вывода, пустых правил (правил вида ");
            writer.Write(@"\begin{math}");
            writer.Write(@"A \rightarrow \varepsilon");
            writer.Write(@"\end{math}");
            writer.Write("), ");
            writer.Write("которые дают лишний переход конечного автомата, что приводит к замедлению ");
            writer.Write("алгоритма разбора цепочки, цепных правил (правил вида ");
            writer.Write(@"\begin{math}");
            writer.Write(@"A \rightarrow B");
            writer.Write(@"\end{math}");
            writer.Write("), т.е. правил, ");
            writer.WriteLine("которые могут привести к зацикливанию алгоритма.");
            writer.WriteLine();

            CheckLangIsEmpty(writer, grammar, grammarType);

            bool changed = false;

            Tuple<Grammar, int> newGrammar = grammar;

            newGrammar = RemoveUselessSymbols(writer, grammar, grammarType, 2);
            grammar = newGrammar;
            newGrammar = RemoveUnreachableSymbols(writer, grammar, grammarType, 3);
            grammar = newGrammar;
            newGrammar = RemoveEmptyRules(writer, grammar, grammarType);
            changed |= grammar.Item2 != newGrammar.Item2;
            grammar = newGrammar;
            newGrammar = RemoveChainRules(writer, grammar, grammarType);
            changed |= grammar.Item2 != newGrammar.Item2;
            grammar = newGrammar;

            if (changed)
            {
                newGrammar = RemoveUselessSymbols(writer, grammar, grammarType, 6);
                grammar = newGrammar;
                newGrammar = RemoveUnreachableSymbols(writer, grammar, grammarType, 7);
                grammar = newGrammar;
            }

            newGrammar = MakeStateMachineGrammar(writer, grammar, grammarType);
            grammar = newGrammar;

            return grammar;
        }

        private static Tuple<Grammar, int> MakeStateMachineGrammar(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType)
        {
            int number = grammar.Item2+1;
            
            WriteSection(writer, grammarType, "Этап 2.3.3", 1);

            writer.WriteLine("На этом шаге для приведенный грамматики строим конечный автомат.");
            writer.WriteLine();

            WriteSection(writer, grammarType, "Этап 2.3.3.1", 2);

            writer.Write("Приведем грамматику ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.Write(@"\end{math}");
            writer.WriteLine(" к автоматному виду.");

            Grammar newGrammar = grammar.Item1.MakeStateMachineGrammar(grammarType, bpr => OnBeginPostReport(writer,bpr,number), ipr => OnIteratePostReport(writer, ipr, number));

            writer.WriteLine();

            writer.Write("Итак, окончательно грамматика ");
            WriteGrammarEx(writer, newGrammar, number);
            writer.WriteLine(".");
            writer.WriteLine();

            return new Tuple<Grammar, int>(newGrammar, number);
        }

        private static void OnBeginPostReport(StreamWriter writer, IReadOnlySet<Rule> postReport, int number)
        {
            writer.WriteLine();
            writer.Write(@"\begin{math}");
            WriteRuleSetSign(writer, number);
            writer.Write(@" = ");
            WriteRuleSet(writer, postReport);
            writer.WriteLine(@"\end{math}.");
        }

        private static void OnIteratePostReport(StreamWriter writer, MakeStateMachineGrammarPostReport postReport, int number)
        {
            writer.WriteLine();
            writer.Write("Обработаем правило ");
            writer.Write(@"\begin{math}");
            WriteRule(writer, postReport.Target, postReport.Chain);
            writer.Write(@"\end{math}.");
            writer.Write(@" Данное правило переносится ");
            writer.Write(postReport.Converted ? @"с изменениями" : @"без изменений");
            writer.Write(@" в ");
            writer.Write(@"\begin{math}");
            WriteRuleSetSign(writer, number);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();
            writer.Write(@"\begin{math}");
            WriteRuleSetSign(writer, number);
            writer.Write(@" = ");
            WriteRuleSetSign(writer, number);
            writer.Write(@" \cup ");
            WriteRuleSet(writer, postReport.NewRules);
            writer.Write(@" = ");
            WriteRuleSet(writer, postReport.PreviousRules);
            writer.Write(@" \cup ");
            WriteRuleSet(writer, postReport.NewRules);
            writer.Write(@" = ");
            WriteRuleSet(writer, postReport.NextRules);
            writer.WriteLine(@"\end{math}.");
        }

        private static Tuple<Grammar, int> RemoveChainRules(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3.2.5", 2);

            writer.Write("Удалим цепные правила грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            Grammar newGrammar;

            bool removed = grammar.Item1.RemoveChainRules(out newGrammar,
                bpr => OnChainBeginPostReport(writer, bpr),
                ipr => OnChainIteratePostReport(writer, ipr),
                epr => OnChainEndPostReport(writer, epr));

            int newGrammarNumber = grammar.Item2;

            if (removed)
            {
                newGrammarNumber++;

                writer.Write(@"В результате выполнения алгоритма произошло удаление цепных правил. ");
                writer.Write(@"Получаем грамматику ");
                WriteGrammarEx(writer, newGrammar, newGrammarNumber);
                writer.WriteLine(@".");
            }
            else
            {
                writer.WriteLine(@"В результате выполнения алгоритма удаление цепных правил не произошло.");
            }

            writer.WriteLine();

            return new Tuple<Grammar, int>(newGrammar, newGrammarNumber);
        }

        private static void OnChainBeginPostReport(StreamWriter writer, ChainRulesBeginPostReport postReport)
        {
            writer.WriteLine(@"\begin{enumerate}");
            writer.Write(@"\item ");
            WriteNonTerminalSymbolMap(writer, postReport.Iteration, postReport.SymbolMap);
            writer.WriteLine(@".");
        }

        private static void WriteNonTerminalSymbolMap(StreamWriter writer, int iteration,
            IReadOnlyDictionary<NonTerminalSymbol, ChainRulesIterationTuple> symbolMap)
        {
            bool first = true;

            foreach (KeyValuePair<NonTerminalSymbol, ChainRulesIterationTuple> value in symbolMap)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                writer.Write(@"\begin{math}");
                WriteWorkSetSign(writer, value.Key, iteration);
                writer.Write(@" = ");
                WriteWorkSetSign(writer, value.Key, iteration - 1);
                writer.Write(@" \cup ");
                WriteSymbolSet(writer, value.Value.New);
                writer.Write(@" = ");
                WriteSymbolSet(writer, value.Value.Previous);
                writer.Write(@" \cup ");
                WriteSymbolSet(writer, value.Value.New);
                writer.Write(@" = ");
                WriteSymbolSet(writer, value.Value.Next);
                writer.Write(@"\end{math}");
            }
        }

        private static void WriteNonTerminalSymbolMap(StreamWriter writer, int iteration, IReadOnlyDictionary<NonTerminalSymbol,IReadOnlySet<NonTerminalSymbol>> symbolMap)
        {
            bool first = true;

            foreach (KeyValuePair<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> value in symbolMap)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                writer.Write(@"\begin{math}");
                WriteWorkSetSign(writer, value.Key, iteration);
                writer.Write(@" = ");
                WriteSymbolSet(writer, value.Value);
                writer.Write(@"\end{math}");
            }
        }

        private static void WriteWorkSetSign(StreamWriter writer, NonTerminalSymbol nonTerminal, int? number = null)
        {
            writer.Write(@"{");
            WriteSymbol(writer, "N", number);
            writer.Write(@"^");
            WriteNonTerminal(writer,nonTerminal);
            writer.Write(@"}");
        }

        private static void OnChainIteratePostReport(StreamWriter writer, ChainRulesIterationPostReport postReport)
        {
            writer.Write(@"\item ");
            WriteNonTerminalSymbolMap(writer, postReport.Iteration, postReport.SymbolMap);
            writer.WriteLine(@".");
            writer.Write(@"Построение множеств ");
            writer.Write(@"\begin{math}");
            writer.Write(@"{N_i}^X");
            writer.Write(@"\end{math}");
            writer.Write(@" для нетерминалов ");
            writer.Write(@"\begin{math}");
            WriteSymbolSet(writer, postReport.SymbolMap.Where(kv => kv.Value.IsLastIteration).Select(kv => kv.Key));
            writer.Write(@"\end{math}");
            writer.Write(@" заканчиваем, так как они не изменились на данном шаге.");

            if (!postReport.IsLastIteration)
            {
                writer.Write(@" Продолжаем построение ");
                writer.Write(@"\begin{math}");
                writer.Write(@"{N_i}^X");
                writer.Write(@"\end{math}");
                writer.Write(@" для нетерминалов ");
                writer.Write(@"\begin{math}");
                WriteSymbolSet(writer, postReport.SymbolMap.Where(kv => !kv.Value.IsLastIteration).Select(kv => kv.Key));
                writer.WriteLine(@"\end{math}.");
            }
            else
            {
                writer.Write(@" Построение множеств ");
                writer.Write(@"\begin{math}");
                writer.Write(@"{N_i}^X");
                writer.Write(@"\end{math}");
                writer.Write(@" для всех нетерминалов грамматики закончено, приступаем к формированию грамматики ");
                writer.WriteLine(@"без цепных правил.");
            }

            writer.WriteLine();
        }

        private static void OnChainEndPostReport(StreamWriter writer, ChainRulesEndPostReport postReport)
        {
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();
            writer.Write("Исключаем из построенных множеств нетерминалы, для которых эти множества построены: ");

            bool first = true;
            
            foreach (KeyValuePair<NonTerminalSymbol, ChainRulesEndTuple> value in postReport.SymbolMap)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                writer.Write(@"\begin{math}");
                WriteWorkSetSign(writer, value.Key);
                writer.Write(" = ");
                WriteWorkSetSign(writer, value.Key, value.Value.Iteration);
                writer.Write(@" \setminus ");
                WriteSymbolSet(writer, value.Key.AsSequence());
                writer.Write(" = ");
                WriteSymbolSet(writer, value.Value.NonTerminals);
                writer.Write(@" \setminus ");
                WriteSymbolSet(writer, value.Key.AsSequence());
                writer.Write(" = ");
                WriteSymbolSet(writer, value.Value.FinalNonTerminals);
                writer.Write(@"\end{math}");
            }

            writer.WriteLine(".");
            writer.WriteLine();
        }

        private static Tuple<Grammar, int> RemoveUnreachableSymbols(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType, int substep)
        {
            WriteSection(writer, grammarType, string.Format(_RussianCulture, "Этап 2.3.2.{0}", substep), 2);

            writer.Write("Удалим недостижимые символы грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            Grammar newGrammar;

            writer.WriteLine(@"\begin{enumerate}");
            bool removed = grammar.Item1.RemoveUnreachableSymbols(out newGrammar, bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            int newGrammarNumber = grammar.Item2;

            if (removed)
            {
                newGrammarNumber++;

                writer.Write(@"В результате выполнения алгоритма произошло удаление недостижимых символов. ");
                writer.Write(@"Получаем грамматику ");
                WriteGrammarEx(writer, newGrammar, newGrammarNumber);
                writer.WriteLine(@".");
            }
            else
            {
                writer.WriteLine(@"В результате выполнения алгоритма удаление недостижимых символов не произошло.");
            }

            writer.WriteLine();

            return new Tuple<Grammar, int>(newGrammar, newGrammarNumber);
        }

        private static Tuple<Grammar, int> RemoveUselessSymbols(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType, int substep)
        {
            WriteSection(writer, grammarType, string.Format(_RussianCulture, "Этап 2.3.2.{0}", substep), 2);

            writer.Write("Удалим бесполезные символы грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            Grammar newGrammar;

            writer.WriteLine(@"\begin{enumerate}");
            bool removed = grammar.Item1.RemoveUselessSymbols(out newGrammar, bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            int newGrammarNumber = grammar.Item2;

            if (removed)
            {
                newGrammarNumber++;

                writer.Write(@"В результате выполнения алгоритма произошло удаление бесполезных символов. ");
                writer.Write(@"Получаем грамматику ");
                WriteGrammarEx(writer, newGrammar, newGrammarNumber);
                writer.WriteLine(@".");
            }
            else
            {
                writer.WriteLine(@"В результате выполнения алгоритма удаление бесполезных символов не произошло.");                
            }

            writer.WriteLine();

            return new Tuple<Grammar,int>(newGrammar, newGrammarNumber);
        }

        private static Tuple<Grammar, int> RemoveEmptyRules(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3.2.4", 2);

            writer.Write("Удалим пустые правила грамматики ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            Grammar newGrammar;

            writer.WriteLine(@"\begin{enumerate}");
            bool removed = grammar.Item1.RemoveEmptyRules(out newGrammar, bpr => OnBeginPostReport(writer, bpr), ipr => OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            int newGrammarNumber = grammar.Item2;

            if (removed)
            {
                newGrammarNumber++;

                writer.Write(@"В результате выполнения алгоритма произошло удаление пустых правил. ");
                writer.Write(@"Получаем грамматику ");
                WriteGrammarEx(writer, newGrammar, newGrammarNumber);
                writer.WriteLine(@".");
            }
            else
            {
                writer.WriteLine(@"В результате выполнения алгоритма удаление пустых правил не произошло.");
            }

            writer.WriteLine();

            return new Tuple<Grammar, int>(newGrammar, newGrammarNumber);
        }

        private static bool CheckLangIsEmpty(StreamWriter writer, Tuple<Grammar, int> grammar, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3.2.1", 2);
            writer.Write("Проверим на пустоту грамматику ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammar.Item2);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();
            int lastIteration = -1;
            writer.WriteLine(@"\begin{enumerate}");
            bool isEmpty = grammar.Item1.IsLangEmpty(bpr => OnBeginPostReport(writer, bpr), ipr => lastIteration = OnIteratePostReport(writer, ipr));
            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            writer.Write("Так как ");
            writer.Write(@"\begin{math}");

            WriteNonTerminal(writer, grammar.Item1.Target);

            if (isEmpty)
            {
                writer.Write(@" \notin ");
            }
            else
            {
                writer.Write(@" \in ");
            }

            WriteWorkSetSign(writer, lastIteration);

            writer.Write(@"\end{math}, ");

            if (isEmpty)
            {
                writer.Write(@"язык пуст.");
            }
            else
            {
                writer.Write(@"язык не пуст.");
            }

            writer.WriteLine();

            return isEmpty;
        }

        private static void OnBeginPostReport<T>(StreamWriter writer, BeginPostReport<T> beginPostReport) where T : FLaGLib.Data.Grammars.Symbol
        {
            writer.Write(@"\item ");
            writer.Write(@"\begin{math}");
            WriteWorkSetSign(writer, beginPostReport.Iteration);
            writer.Write(@" = ");
            WriteSymbolSet(writer, beginPostReport.SymbolSet);
            writer.WriteLine(@"\end{math}.");
        }

        private static int OnIteratePostReport<T>(StreamWriter writer, IterationPostReport<T> iteratePostReport) where T : FLaGLib.Data.Grammars.Symbol
        {
            writer.Write(@"\item ");
            writer.Write(@"\begin{math}");
            WriteWorkSetSign(writer, iteratePostReport.Iteration);
            writer.Write(@" = ");
            WriteWorkSetSign(writer, iteratePostReport.Iteration - 1);
            writer.Write(@" \cup ");
            WriteSymbolSet(writer, iteratePostReport.NewSymbolSet);
            writer.Write(@" = ");
            WriteSymbolSet(writer, iteratePostReport.PreviousSymbolSet);
            writer.Write(@" \cup ");
            WriteSymbolSet(writer, iteratePostReport.NewSymbolSet);
            writer.Write(@" = ");
            WriteSymbolSet(writer, iteratePostReport.NextSymbolSet);
            writer.WriteLine(@"\end{math}.");

            return iteratePostReport.Iteration;
        }

        private static Tuple<Grammar,int> ConvertToGrammar(StreamWriter writer, Expression expression, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3.1",1);
            writer.Write("В процессе построения будем использовать изолированную от остальных пунктов нумерацию грамматик. Построим ");
            writer.WriteLatex(GetGrammarTypeRussianName(grammarType, RussianCaseType.Accusative));
            writer.Write(" грамматику для выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Воспользуемся рекурсивным определением регулярного выражения для построения последовательности ");
            writer.WriteLatex(GetGrammarTypeRussianName(grammarType, RussianCaseType.Genitive, false));
            writer.Write(" грамматик для каждого элементарного выражения, входящих в состав выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Собственно последняя грамматика и будет являться искомой. Определим совокупность выражений, ");
            writer.Write("входящих в состав исходного выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.WriteLine();
            writer.WriteLine();
            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _OriginalRegularExpressionExpandedLabel, grammarType);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            WriteExpressionEx(writer, expression);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.WriteLine(@"\end{equation}");
            writer.WriteLine();
            writer.Write(@"Построим ");
            writer.WriteLatex(GetGrammarTypeRussianName(grammarType, RussianCaseType.Accusative,false));
            writer.Write(" грамматики для указанных выражений. Каждую грамматику будем нумеровать по номеру выражения, ");
            writer.WriteLine("для которого строится данная грамматика.");
            writer.WriteLine();

            writer.WriteLine(@"\begin{enumerate}");

            int grammarNumber = -1;

            Grammar grammar = expression.MakeGrammar(grammarType, g => grammarNumber = OnGrammarPostReport(writer, g));

            writer.WriteLine(@"\end{enumerate}");
            writer.WriteLine();

            writer.Write("Далее по тексту (в следующих пунктах) будем обозначать полученную грамматику ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammarNumber);
            writer.Write(@"\end{math}");
            writer.Write(@" следующим образом: ");

            grammarNumber = 1;

            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammarNumber);
            writer.WriteLine(@"\end{math}.");
            writer.WriteLine();

            return new Tuple<Grammar, int>(grammar, grammarNumber);
        }

        private static int OnGrammarPostReport(StreamWriter writer, GrammarPostReport grammarPostReport)
        {
            writer.Write(@"\item ");
            writer.Write("Для выражения вида ");
            writer.Write(@"\begin{math}");
            WriteExpression(writer, grammarPostReport.New.Expression, true);
            writer.Write(@"\end{math}, являющегося ");
            writer.WriteLatex(GetExpressionTypeRussianName(grammarPostReport.New.Expression.ExpressionType, RussianCaseType.Ablative));

            if (grammarPostReport.Dependencies.Count > 0)
            {
                if (grammarPostReport.Dependencies.Count < 2)
                {
                    writer.Write(" выражения с построенной грамматикой ");
                }
                else
                {
                    writer.Write(" выражений с построенными грамматиками ");
                }

                bool first = true;

                foreach (GrammarExpressionWithOriginal dependency in grammarPostReport.Dependencies)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(", ");
                    }

                    writer.Write(@"\begin{math}");
                    WriteGrammarSign(writer, dependency.GrammarExpression.Number);
                    writer.Write(@"\end{math}");

                    if (dependency.OriginalGrammarExpression != null)
                    {
                        writer.Write(" (построена из грамматики ");
                        writer.Write(@"\begin{math}");
                        WriteGrammarSign(writer, dependency.OriginalGrammarExpression.Number);
                        writer.Write(@"\end{math}");
                        writer.Write(@" путем соответствующей замены индексов)");
                    }
                }

            }

            writer.Write(", построим грамматику ");
            WriteGrammarEx(writer, grammarPostReport.New.Grammar, grammarPostReport.New.Number);

            writer.WriteLine(".");

            return grammarPostReport.New.Number;
        }

        private static void WriteGrammarEx(StreamWriter writer, Grammar grammar, int number)
        {
            writer.Write(@"\begin{math}");
            WriteGrammarTuple(writer, number);
            writer.Write(@"\end{math}, где ");

            writer.Write(@"\begin{math}");
            WriteNonTerminalSetSign(writer, number);
            writer.Write(" = ");
            WriteSymbolSet(writer, grammar.NonTerminals);
            writer.Write(@"\end{math} --- множество нетерминальных символов грамматики, ");

            writer.Write(@"\begin{math}");
            WriteTerminalSetSign(writer, number);
            writer.Write(" = ");
            WriteSymbolSet(writer, grammar.Alphabet);
            writer.Write(@"\end{math} --- множество терминальных символов (алфавит) грамматики, ");

            writer.Write(@"\begin{math}");
            WriteRuleSetSign(writer, number);
            writer.Write(" = ");
            WriteRuleSet(writer, grammar.Rules);
            writer.Write(@"\end{math} --- множество правил вывода для данной грамматики, ");

            writer.Write(@"\begin{math}");
            WriteTargetNonTerminalSign(writer, number);
            writer.Write(" = ");
            WriteNonTerminal(writer, grammar.Target);
            writer.Write(@"\end{math} --- целевой символ грамматики ");

            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, number);
            writer.Write(@"\end{math}");
        }

        private static void WriteNonTerminal(StreamWriter writer, NonTerminalSymbol nonTerminal)
        {
            WriteLabel(writer, nonTerminal.Label);
        }

        private static void WriteRuleSet(StreamWriter writer, IEnumerable<Rule> rules)
        { 
            bool first = true;

            if (rules.Any())
            {
                writer.Write(@"\{");

                foreach (Rule rule in rules)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\comma ");
                    }

                    WriteRule(writer, rule);
                }

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }

        private static void WriteRule(StreamWriter writer, NonTerminalSymbol target, Chain chain)
        {
            WriteNonTerminal(writer, target);

            writer.Write(@" \rightarrow ");

            WriteChain(writer, chain);
        }

        private static void WriteRule(StreamWriter writer, Rule rule)
        {
            WriteNonTerminal(writer, rule.Target);

            writer.Write(@" \rightarrow ");

            WriteChainSet(writer, rule.Chains);
        }

        private static void WriteChainSet(StreamWriter writer, IEnumerable<Chain> chains)
        {
            bool first = true;

            foreach (Chain chain in chains)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(@" \mid ");
                }

                WriteChain(writer, chain);
            }
        }

        private static void WriteChain(StreamWriter writer, Chain chain)
        {
            if (chain.Any())
            {
                foreach (FLaGLib.Data.Grammars.Symbol symbol in chain)
                {
                    WriteSymbol(writer, symbol);
                }
            }
            else
            {
                writer.Write(@"{\varepsilon}");
            }
        }

        private static void WriteSymbol(StreamWriter writer, FLaGLib.Data.Grammars.Symbol symbol)
        {
            switch (symbol.SymbolType)
            {
                case SymbolType.NonTerminal:
                    WriteNonTerminal(writer, (NonTerminalSymbol)symbol);
                    break;
                case SymbolType.Terminal:
                    WriteTerminal(writer, (TerminalSymbol)symbol);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Symbol type {0} is not supported.", symbol.SymbolType));
            }
        }

        private static void WriteTerminal(StreamWriter writer, TerminalSymbol symbol)
        {
            writer.WriteLatex(symbol.Symbol.ToString(_RussianCulture));
        }

        private static void WriteSymbolSet<T>(StreamWriter writer, IEnumerable<T> symbolSet) where T : FLaGLib.Data.Grammars.Symbol
        {
            bool first = true;

            if (symbolSet.Any())
            {
                writer.Write(@"\{");

                foreach (FLaGLib.Data.Grammars.Symbol symbol in symbolSet)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(@"\comma ");
                    }

                    WriteSymbol(writer, symbol);
                }

                writer.Write(@"\}");
            }
            else
            {
                writer.Write(@"{\varnothing}");
            }
        }
        
        private static void WriteLabel(StreamWriter writer, Label label)
        {
            if (label.LabelType == LabelType.Complex)
            {
                writer.Write("[");
            }

            foreach (SingleLabel singleLabel in label.Sublabels)
            {
                WriteSingleLabel(writer, singleLabel);
            }

            if (label.LabelType == LabelType.Complex)
            {
                writer.Write("]");
            }
        }

        private static void WriteSingleLabel(StreamWriter writer, SingleLabel singleLabel)
        {
            WriteSymbol(writer, singleLabel.Sign.ToString(_RussianCulture), singleLabel.SignIndex);
        }

        private static void WriteWorkSetSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, "C", number);
        }

        private static void WriteGrammarSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, "G", number);
        }

        private static void WriteNonTerminalSetSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, "N", number);
        }

        private static void WriteRuleSetSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, "P", number);
        }

        private static void WriteTerminalSetSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, @"\Sigma", number);
        }

        private static void WriteTargetNonTerminalSign(StreamWriter writer, int? number)
        {
            WriteSymbol(writer, "S", number);
        }

        private static void WriteGrammarTuple(StreamWriter writer, int? number)
        {
            WriteGrammarSign(writer, number);
            writer.Write(@"=\left(");
            WriteNonTerminalSetSign(writer, number);
            writer.Write(", ");
            WriteTerminalSetSign(writer, number);
            writer.Write(", ");
            WriteRuleSetSign(writer, number);
            writer.Write(", ");
            WriteTargetNonTerminalSign(writer, number);
            writer.Write(@"\right)");
        }

        private static void WriteSymbol(StreamWriter writer, string symbol, int? value)
        {
            writer.Write("{");
            writer.Write("{");
            writer.Write(symbol);
            writer.Write("}");

            if (value != null)
            {
                writer.Write("_{");
                writer.WriteLatex(value.Value.ToString(_RussianCulture));
                writer.Write("}");
            }

            writer.Write("}");
        }


        private static void WriteSection(StreamWriter writer, string title, string subtitle = null, int subcount = 0)
        {
            writer.Write(@"\");

            for (int i = 0; i < subcount; i++)
            {
                writer.Write("sub");
            }

            writer.Write("section{");
            writer.WriteLatex(title);

            if (!string.IsNullOrEmpty(subtitle))
            {
                writer.Write(" (");
                writer.Write(subtitle);
                writer.Write(")");
            }

            writer.WriteLine("}");
        }

        private static void WriteSection(StreamWriter writer, GrammarType grammarType, string title, int subcount = 0)
        {
            WriteSection(writer, title, GetGrammarTypeRussianName(grammarType), subcount);
        }

        private static void WriteRegexSection(StreamWriter writer, string title, int subcount = 0)
        {
            WriteSection(writer, title, _RegularExpressionText, subcount);
        }

        private static void WriteConvertToExpression(StreamWriter writer, Expression expression)
        {
            writer.WriteLine(@"\section{Этап 2}");
            writer.WriteLine(@"\subsection{Этап 2.1}");
            writer.Write(@"Выражение ");
            WriteEquationRef(writer,_OriginalRegularSetLabel);
            writer.WriteLine(" уже представляет регулярное множество.");
            writer.WriteLine(@"\subsection{Этап 2.2}");
            writer.Write(@"Регулярное выражение для множества ");
            WriteEquationRef(writer, _OriginalRegularSetLabel);
            writer.WriteLine(@" примет вид");
            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _OriginalRegularExpressionLabel);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            WriteExpression(writer, expression, true);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.WriteLine(@"\end{equation}");
        }

        private static void WriteEquationLabel(StreamWriter writer, string uniqueId, GrammarType grammarType)
        {
            writer.Write(@"\label{eq:");
            writer.WriteLatex(string.Concat(uniqueId,grammarType.ToString()));
            writer.Write(@"}");
        }

        private static void WriteImageRef(StreamWriter writer, string uniqueId)
        {
            writer.Write(@"\ref{img:");
            writer.WriteLatex(uniqueId);
            writer.Write(@"}");
        }

        private static void WriteDiagramRef(StreamWriter writer, int number)
        {
            WriteImageRef(writer, string.Format(CultureInfo.InvariantCulture, _DiagramLabel, number));
        }

        private static void WriteEquationRef(StreamWriter writer, string uniqueId, GrammarType grammarType)
        {
            writer.Write(@"(\ref{eq:");
            writer.WriteLatex(string.Concat(uniqueId,grammarType.ToString()));
            writer.Write(@"})");
        }

        private static void WriteEquationLabel(StreamWriter writer, string uniqueId)
        {
            writer.Write(@"\label{eq:");
            writer.WriteLatex(uniqueId);
            writer.Write(@"}");
        }

        private static void WriteEquationRef(StreamWriter writer, string uniqueId)
        {
            writer.Write(@"(\ref{eq:");
            writer.WriteLatex(uniqueId);
            writer.Write(@"})");
        }

        private static Expression WriteCheckLanguageType(StreamWriter writer, Entity language)
        {
            Expression expression = language.ToRegExp();

            writer.WriteLine(@"\section{Этап 1}");
            writer.WriteLine(@"Докажем, что язык является регулярным, используя свойство замкнутости.");
            writer.WriteLine(@"Для этого представим язык в виде регулярного множества.");
            writer.Write(@"\begin{equation}");
            WriteEquationLabel(writer, _OriginalRegularSetLabel);
            writer.WriteLine();
            writer.WriteLine(@"\begin{split}");
            WriteExpressionEx(writer, expression, true);
            writer.WriteLine();
            writer.WriteLine(@"\end{split}");
            writer.WriteLine(@"\end{equation}");

            writer.WriteLine(@"\begin{enumerate}");

            int lastIteration = -1;

            expression.CheckRegular(l => lastIteration = OnLanguagePostReport(writer, l));

            writer.WriteLine(@"\end{enumerate}");

            writer.WriteLine();
            writer.Write(@"Так как языки ");
            writer.Write(@"\begin{math}");
            WriteSign(writer, 'L', lastIteration);
            writer.Write(@"\end{math}");
            writer.Write(@" и ");
            writer.Write(@"\begin{math}L\end{math}");
            writer.Write(@" описываются одним и тем же регулярным множеством, а язык ");
            writer.Write(@"\begin{math}");
            WriteSign(writer, 'L', lastIteration);
            writer.Write(@"\end{math}");
            writer.WriteLine(@" является регулярным, то и язык \begin{math}L\end{math} также является регулярным.");

            writer.WriteLine();

            return expression;
        }

        private static string GetGrammarTypeRussianName(GrammarType grammarType, RussianCaseType caseType = RussianCaseType.Nominative, bool singular = true)
        {
            switch (grammarType)
            {
                case GrammarType.Left:
                    return GetLeftGrammarTypeRussianName(caseType, singular);
                case GrammarType.Right:
                    return GetRightGrammarTypeRussianName(caseType, singular);
                default:
                    throw new InvalidOperationException(string.Format("Grammar type {0} is not supported.", grammarType));

            }
        }

        private static string GetRightGrammarTypeRussianName(RussianCaseType caseType, bool singular)
        {
            switch (caseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "праволинейная" : "праволинейные";
                case RussianCaseType.Genitive:
                    return singular ? "праволинейной" : "праволинейных";
                case RussianCaseType.Dative:
                    return singular ? "праволинейной" : "праволинейным";
                case RussianCaseType.Accusative:
                    return singular ? "праволинейную" : "праволинейные";
                case RussianCaseType.Ablative:
                    return singular ? "праволинейной" : "праволинейными";
                case RussianCaseType.Prepositional:
                    return singular ? "праволинейной" : "праволинейных";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, caseType));
            }
        }

        private static string GetLeftGrammarTypeRussianName(RussianCaseType caseType, bool singular)
        {
            switch (caseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "леволинейная" : "леволинейные";
                case RussianCaseType.Genitive:
                    return singular ? "леволинейной" : "леволинейных";
                case RussianCaseType.Dative:
                    return singular ? "леволинейной" : "леволинейным";
                case RussianCaseType.Accusative:
                    return singular ? "леволинейную" : "леволинейные";
                case RussianCaseType.Ablative:
                    return singular ? "леволинейной" : "леволинейными";
                case RussianCaseType.Prepositional:
                    return singular ? "леволинейной" : "леволинейных";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, caseType));
            }
        }

        private static string GetConcatRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "конкатенация" : "конкатенации";
                case RussianCaseType.Genitive:
                    return singular ? "конкатенации" : "конкатенаций";
                case RussianCaseType.Dative:
                    return singular ? "конкатенации" : "конкатенациям";
                case RussianCaseType.Accusative:
                    return singular ? "конкатенацию" : "конкатенации";
                case RussianCaseType.Ablative:
                    return singular ? "конкатенацией" : "конкатенациями";
                case RussianCaseType.Prepositional:
                    return singular ? "конкатенации" : "конкатенациях";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetUnionRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "объединение" : "объединения";
                case RussianCaseType.Genitive:
                    return singular ? "объединения" : "объединений";
                case RussianCaseType.Dative:
                    return singular ? "объединению" : "объединениям";
                case RussianCaseType.Accusative:
                    return singular ? "объединение" : "объединения";
                case RussianCaseType.Ablative:
                    return singular ? "объединением" : "объединениями";
                case RussianCaseType.Prepositional:
                    return singular ? "объединении" : "объединениях";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetIterationRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "итерация" : "итерации";
                case RussianCaseType.Genitive:
                    return singular ? "итерации" : "итераций";
                case RussianCaseType.Dative:
                    return singular ? "итерации" : "итерациям";
                case RussianCaseType.Accusative:
                    return singular ? "итерацию" : "итерации";
                case RussianCaseType.Ablative:
                    return singular ? "итерацией" : "итерацией";
                case RussianCaseType.Prepositional:
                    return singular ? "итерации" : "итерациях";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetConstIterationRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "повторение" : "повторения";
                case RussianCaseType.Genitive:
                    return singular ? "повторения" : "повторений";
                case RussianCaseType.Dative:
                    return singular ? "повторению" : "повторениям";
                case RussianCaseType.Accusative:
                    return singular ? "повторение" : "повторения";
                case RussianCaseType.Ablative:
                    return singular ? "повторением" : "повторениями";
                case RussianCaseType.Prepositional:
                    return singular ? "повторении" : "повторениях";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetSymbolRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "символ" : "символы";
                case RussianCaseType.Genitive:
                    return singular ? "символа" : "символов";
                case RussianCaseType.Dative:
                    return singular ? "символу" : "символам";
                case RussianCaseType.Accusative:
                    return singular ? "символ" : "символы";
                case RussianCaseType.Ablative:
                    return singular ? "символом" : "символами";
                case RussianCaseType.Prepositional:
                    return singular ? "символе" : "символах";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetEmptyRussianName(RussianCaseType russianCaseType, bool singular = true)
        {
            switch (russianCaseType)
            {
                case RussianCaseType.Nominative:
                    return singular ? "пустая цепочка" : "пустые цепочки";
                case RussianCaseType.Genitive:
                    return singular ? "пустой цепочки" : "пустых цепочек";
                case RussianCaseType.Dative:
                    return singular ? "пустой цепочке" : "пустым цепочкам";
                case RussianCaseType.Accusative:
                    return singular ? "пустую цепочку" : "пустые цепочки";
                case RussianCaseType.Ablative:
                    return singular ? "пустой цепочкой" : "пустыми цепочками";
                case RussianCaseType.Prepositional:
                    return singular ? "пустой цепочке" : "пустых цепочках";
                default:
                    throw new InvalidOperationException(string.Format(_RussianCaseIsNotSupportedMessage, russianCaseType));
            }
        }

        private static string GetExpressionTypeRussianName(ExpressionType expressionType, RussianCaseType russianCaseType, bool singular = true)
        {
            switch (expressionType)
            {
                case ExpressionType.BinaryConcat:
                case ExpressionType.Concat:
                    return GetConcatRussianName(russianCaseType, singular);
                case ExpressionType.BinaryUnion:
                case ExpressionType.Union:
                    return GetUnionRussianName(russianCaseType, singular);
                case ExpressionType.Iteration:
                    return GetIterationRussianName(russianCaseType, singular);
                case ExpressionType.ConstIteration:
                    return GetConstIterationRussianName(russianCaseType, singular);
                case ExpressionType.Symbol:
                    return GetSymbolRussianName(russianCaseType, singular);
                case ExpressionType.Empty:
                    return GetEmptyRussianName(russianCaseType, singular);
                default:
                    throw new InvalidOperationException(string.Format("Expression type {0} is not supported.", expressionType));
            }
        }

        private static void WriteSign(StreamWriter writer, char sign, int number)
        {
            writer.Write("{");
            writer.WriteLatex(sign.ToString(_RussianCulture));
            writer.Write("_{");
            writer.WriteLatex(number.ToString(_RussianCulture));
            writer.Write("}");
            writer.Write("}");
        }

        private static int OnLanguagePostReport(StreamWriter writer, LanguagePostReport languagePostReport)
        {            
            writer.Write(@"\item Регулярное множество вида ");
            writer.Write(@"\begin{math}");
            WriteExpression(writer, languagePostReport.New.Expression, true, true);
            writer.Write(@"\end{math}");
            writer.Write(@", которое является ");
            writer.WriteLatex(GetExpressionTypeRussianName(languagePostReport.New.Expression.ExpressionType, RussianCaseType.Ablative));

            if (languagePostReport.Dependencies.Count > 0)
            {
                bool plural = languagePostReport.Dependencies.Count > 1; 

                if (plural)
                {
                    writer.WriteLatex(" языков ");
                }
                else
                {
                    writer.WriteLatex(" языка ");
                }

                bool first = true;

                foreach (LanguageExpressionTuple tuple in languagePostReport.Dependencies)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.Write(", ");
                    }

                    writer.Write(@"\begin{math}");
                    WriteSign(writer, 'L', tuple.LanguageNumber);
                    writer.Write(@"\end{math}");
                }
            }

            writer.WriteLatex(" соответствует языку ");
            writer.Write(@"\begin{math}");
            WriteSign(writer, 'L', languagePostReport.New.LanguageNumber);
            writer.Write(@"\end{math}");
            writer.WriteLine(".");

            return languagePostReport.New.LanguageNumber;
        }

        private static void WriteExpressionEx(StreamWriter writer, FLaGLib.Data.RegExps.Expression expression, bool asRegularSet = false)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap = expression.DependencyMap;

            WriteExpressionEx(writer, dependencyMap, dependencyMap.Count - 1, asRegularSet);
        }

        private static void WriteExpressionEx(StreamWriter writer, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            writer.Write(@"{");
            writer.Write(@"\underbrace{");

            FLaGLib.Data.RegExps.Expression expression = dependencyMap[index].Expression;

            switch (expression.ExpressionType)
            {
                case FLaGLib.Data.RegExps.ExpressionType.Concat:
                    WriteConcatEx(writer, (FLaGLib.Data.RegExps.Concat)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.BinaryConcat:
                    WriteBinaryConcatEx(writer, (FLaGLib.Data.RegExps.BinaryConcat)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Union:
                    WriteUnionEx(writer, (FLaGLib.Data.RegExps.Union)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.BinaryUnion:
                    WriteBinaryUnionEx(writer, (FLaGLib.Data.RegExps.BinaryUnion)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Symbol:
                    WriteSymbolEx(writer, (FLaGLib.Data.RegExps.Symbol)expression, dependencyMap, index);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Iteration:
                    WriteIterationEx(writer, (FLaGLib.Data.RegExps.Iteration)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.ConstIteration:
                    WriteConstIterationEx(writer, (FLaGLib.Data.RegExps.ConstIteration)expression, dependencyMap, index, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Empty:
                    WriteEmptyEx(writer, (FLaGLib.Data.RegExps.Empty)expression, dependencyMap, index);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The expression type {0} is not supported.", expression.ExpressionType));
            }

            writer.Write(@"}_\text{");
            writer.WriteLatex((index + 1).ToString(_RussianCulture));
            writer.Write(@"}");
            writer.Write(@"}");
        }

        private static void WriteEmptyEx(StreamWriter writer, FLaGLib.Data.RegExps.Empty empty, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index)
        {
            writer.Write(@"{\varepsilon}");
        }

        private static void WriteConstIterationEx(StreamWriter writer, FLaGLib.Data.RegExps.ConstIteration constIteration, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            bool needBrackets = constIteration.Expression.Priority >= constIteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpressionEx(writer, dependencyMap, dependencyMap[index].Single(), asRegularSet);

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");
            writer.WriteLatex(constIteration.IterationCount.ToString(_RussianCulture));
            writer.Write("}");
        }

        private static void WriteIterationEx(StreamWriter writer, FLaGLib.Data.RegExps.Iteration iteration, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            bool needBrackets = iteration.Expression.Priority >= iteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpressionEx(writer, dependencyMap, dependencyMap[index].Single(), asRegularSet);

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");

            if (iteration.IsPositive)
            {
                writer.Write("+");
            }
            else
            {
                writer.Write("*");
            }

            writer.Write("}");
        }

        private static void WriteSymbolEx(StreamWriter writer, FLaGLib.Data.RegExps.Symbol symbol, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index)
        {
            writer.WriteLatex(symbol.Character.ToString(_RussianCulture));
        }

        private static void WriteBinaryUnionEx(StreamWriter writer, FLaGLib.Data.RegExps.BinaryUnion binaryUnion, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            WriteExpressionsEx(writer, dependencyMap, index, asRegularSet ? @" \cup " : " + ", asRegularSet);
        }

        private static void WriteBinaryConcatEx(StreamWriter writer, FLaGLib.Data.RegExps.BinaryConcat binaryConcat, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            WriteExpressionsEx(writer, dependencyMap, index, @" \cdot ", asRegularSet);
        }

        private static void WriteExpressionsEx(StreamWriter writer, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, string separator, bool asRegularSet)
        {
            FLaGLib.Data.RegExps.DependencyCollection dependencies = dependencyMap[index];

            bool first = true;

            foreach (int i in dependencies)
            {
                FLaGLib.Data.RegExps.DependencyCollection currentDependencies = dependencyMap[i];

                bool needBrackets;

                if (first)
                {
                    needBrackets = currentDependencies.Expression.Priority > dependencies.Expression.Priority;
                    first = false;
                }
                else
                {
                    needBrackets = currentDependencies.Expression.Priority >= dependencies.Expression.Priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write(@"\left(");
                }

                WriteExpressionEx(writer, dependencyMap, i, asRegularSet);

                if (needBrackets)
                {
                    writer.Write(@"\right)");
                }
            }
        }

        private static void WriteUnionEx(StreamWriter writer, FLaGLib.Data.RegExps.Union union, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            throw new NotSupportedException();
        }

        private static void WriteConcatEx(StreamWriter writer, FLaGLib.Data.RegExps.Concat concat, IReadOnlyList<FLaGLib.Data.RegExps.DependencyCollection> dependencyMap, int index, bool asRegularSet)
        {
            throw new NotSupportedException();
        }

        private static void WriteExpression(StreamWriter writer, FLaGLib.Data.RegExps.Expression expression, bool writeDots = false, bool asRegularSet = false)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            switch (expression.ExpressionType)
            {
                case FLaGLib.Data.RegExps.ExpressionType.Concat:
                    WriteConcat(writer, (FLaGLib.Data.RegExps.Concat)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.BinaryConcat:
                    WriteBinaryConcat(writer, (FLaGLib.Data.RegExps.BinaryConcat)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Union:
                    WriteUnion(writer, (FLaGLib.Data.RegExps.Union)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.BinaryUnion:
                    WriteBinaryUnion(writer, (FLaGLib.Data.RegExps.BinaryUnion)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Symbol:
                    WriteSymbol(writer, (FLaGLib.Data.RegExps.Symbol)expression);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Iteration:
                    WriteIteration(writer, (FLaGLib.Data.RegExps.Iteration)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.ConstIteration:
                    WriteConstIteration(writer, (FLaGLib.Data.RegExps.ConstIteration)expression, writeDots, asRegularSet);
                    break;
                case FLaGLib.Data.RegExps.ExpressionType.Empty:
                    WriteEmpty(writer, (FLaGLib.Data.RegExps.Empty)expression);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The expression type {0} is not supported.", expression.ExpressionType));
            }
        }

        private static void WriteSymbol(StreamWriter writer, FLaGLib.Data.RegExps.Symbol symbol)
        {
            writer.WriteLatex(symbol.Character.ToString(_RussianCulture));
        }

        private static void WriteConstIteration(StreamWriter writer, FLaGLib.Data.RegExps.ConstIteration constIteration, bool writeDots, bool asRegularSet)
        {
            bool needBrackets = constIteration.Expression.Priority >= constIteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpression(writer, constIteration.Expression, writeDots, asRegularSet);

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");
            writer.WriteLatex(constIteration.IterationCount.ToString(_RussianCulture));
            writer.Write("}");
        }

        private static void WriteIteration(StreamWriter writer, FLaGLib.Data.RegExps.Iteration iteration, bool writeDots, bool asRegularSet)
        {
            bool needBrackets = iteration.Expression.Priority >= iteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpression(writer, iteration.Expression, writeDots, asRegularSet);

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");

            if (iteration.IsPositive)
            {
                writer.Write("+");
            }
            else
            {
                writer.Write("*");
            }

            writer.Write("}");
        }

        private static void WriteEmpty(StreamWriter writer, FLaGLib.Data.RegExps.Empty empty)
        {
            writer.Write(@"{\varepsilon}");
        }

        private static void WriteBinaryUnion(StreamWriter writer, FLaGLib.Data.RegExps.BinaryUnion binaryUnion, bool writeDots, bool asRegularSet)
        {
            ISet<FLaGLib.Data.RegExps.Expression> visitedExpression = new HashSet<FLaGLib.Data.RegExps.Expression>();
            WriteExpressions(writer, FLaGLib.Data.RegExps.UnionHelper.Iterate(visitedExpression, binaryUnion), asRegularSet ? @" \cup " : " + ", binaryUnion.Priority, writeDots, asRegularSet);
        }

        private static void WriteUnion(StreamWriter writer, FLaGLib.Data.RegExps.Union union, bool writeDots, bool asRegularSet)
        {
            ISet<FLaGLib.Data.RegExps.Expression> visitedExpression = new HashSet<FLaGLib.Data.RegExps.Expression>();
            WriteExpressions(writer, FLaGLib.Data.RegExps.UnionHelper.Iterate(visitedExpression, union), asRegularSet ? @" \cup " : " + ", union.Priority, writeDots, asRegularSet);
        }

        private static void WriteBinaryConcat(StreamWriter writer, FLaGLib.Data.RegExps.BinaryConcat binaryConcat, bool writeDots, bool asRegularSet)
        {
            WriteExpressions(writer, FLaGLib.Data.RegExps.ConcatHelper.Iterate(binaryConcat), writeDots ? @" \cdot " : string.Empty, binaryConcat.Priority, writeDots, asRegularSet);
        }

        private static void WriteConcat(StreamWriter writer, FLaGLib.Data.RegExps.Concat concat, bool writeDots, bool asRegularSet)
        {
            WriteExpressions(writer, FLaGLib.Data.RegExps.ConcatHelper.Iterate(concat), writeDots ? @" \cdot " : string.Empty, concat.Priority, writeDots, asRegularSet);
        }

        private static void WriteExpressions(StreamWriter writer, IEnumerable<FLaGLib.Data.RegExps.Expression> expressions, string separator, int priority, bool writeDots, bool asRegularSet)
        {
            bool first = true;

            foreach (FLaGLib.Data.RegExps.Expression expression in expressions)
            {
                bool needBrackets;

                if (first)
                {
                    needBrackets = expression.Priority > priority;
                    first = false;
                }
                else
                {
                    needBrackets = expression.Priority >= priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write(@"\left(");
                }

                WriteExpression(writer, expression, writeDots, asRegularSet);

                if (needBrackets)
                {
                    writer.Write(@"\right)");
                }
            }
        }

        private static void WriteLanguage(StreamWriter writer, FLaGLib.Data.Languages.Entity entity)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            writer.Write(@"\left\{");

            WriteEntity(writer, entity);
            WriteVariables(writer, entity.Variables);

            writer.Write(@"\right\}");
        }

        private static void WriteEntity(StreamWriter writer, FLaGLib.Data.Languages.Entity entity)
        {
            switch (entity.EntityType)
            {
                case FLaGLib.Data.Languages.EntityType.Concat:
                    WriteConcat(writer, (FLaGLib.Data.Languages.Concat)entity);
                    break;
                case FLaGLib.Data.Languages.EntityType.Union:
                    WriteUnion(writer, (FLaGLib.Data.Languages.Union)entity);
                    break;
                case FLaGLib.Data.Languages.EntityType.Symbol:
                    WriteSymbol(writer, (FLaGLib.Data.Languages.Symbol)entity);
                    break;
                case FLaGLib.Data.Languages.EntityType.Degree:
                    WriteDegree(writer, (FLaGLib.Data.Languages.Degree)entity);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The entity type {0} is not supported.", entity.EntityType));
            }
        }

        private static void WriteSymbol(StreamWriter writer, FLaGLib.Data.Languages.Symbol symbol)
        {
            writer.WriteLatex(symbol.Character.ToString(_RussianCulture));
        }

        private static void WriteDegree(StreamWriter writer, FLaGLib.Data.Languages.Degree degree)
        {
            bool needBrackets = degree.Entity.Priority >= degree.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteEntity(writer, degree.Entity);

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");
            WriteExponent(writer, degree.Exponent);
            writer.Write("}");
        }

        private static void WriteUnion(StreamWriter writer, FLaGLib.Data.Languages.Union union)
        {
            WriteEntities(writer, union.EntityCollection, ",", union.Priority);
        }

        private static void WriteConcat(StreamWriter writer, FLaGLib.Data.Languages.Concat union)
        {
            WriteEntities(writer, union.EntityCollection, string.Empty, union.Priority);
        }

        private static void WriteEntities(StreamWriter writer, IEnumerable<FLaGLib.Data.Languages.Entity> entities, string separator, int priority)
        {
            bool first = true;

            foreach (FLaGLib.Data.Languages.Entity entity in entities)
            {
                bool needBrackets;

                if (first)
                {
                    needBrackets = entity.Priority > priority;
                    first = false;
                }
                else
                {
                    needBrackets = entity.Priority >= priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write(@"\left(");
                }

                WriteEntity(writer, entity);

                if (needBrackets)
                {
                    writer.Write(@"\right)");
                }
            }
        }

        private static void WriteExponent(StreamWriter writer, FLaGLib.Data.Languages.Exponent exponent)
        {
            switch (exponent.ExponentType)
            {
                case FLaGLib.Data.Languages.ExponentType.Quantity:
                    WriteQuantity(writer, (FLaGLib.Data.Languages.Quantity)exponent);
                    break;
                case FLaGLib.Data.Languages.ExponentType.Variable:
                    WriteVariable(writer, (FLaGLib.Data.Languages.Variable)exponent);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The exponent type {0} is not supported.", exponent.ExponentType));
            }
        }

        private static void WriteVariable(StreamWriter writer, FLaGLib.Data.Languages.Variable variable)
        {
            writer.WriteLatex(variable.Name.ToString(_RussianCulture));
        }

        private static void WriteQuantity(StreamWriter writer, FLaGLib.Data.Languages.Quantity quantity)
        {
            writer.WriteLatex(quantity.Count.ToString(_RussianCulture));
        }

        private static void WriteVariables(StreamWriter writer, IReadOnlySet<FLaGLib.Data.Languages.Variable> variables)
        {
            if (!variables.Any())
            {
                return;
            }

            writer.Write(@" \mid \forall ");

            foreach (FLaGLib.Data.Languages.Variable variable in variables)
            {
                writer.WriteLatex(variable.Name.ToString(_RussianCulture));
                switch (variable.Sign)
                {
                    case FLaGLib.Data.Languages.Sign.MoreThan:
                        writer.Write(@" > ");
                        break;
                    case FLaGLib.Data.Languages.Sign.MoreThanOrEqual:
                        writer.Write(@" \geq ");
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Sign {0} is not supported.", variable.Sign));
                }

                writer.WriteLatex(variable.Number.ToString(_RussianCulture));
                writer.Write(@", ");
            }

            writer.Write(@"\text{где } ");

            writer.WriteLatex(string.Join(", ", variables.Select(v => v.Name.ToString(_RussianCulture))));

            writer.Write(@" \text{ --- целые}");
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
            writer.WriteLine(@"\newcommand{\comma}{,\allowbreak}");
            writer.WriteLine(@"\newcommand{\semicolon}{;\allowbreak}");
            writer.WriteLine(@"\tolerance=10000");
            writer.WriteLine(@"\relpenalty=10000");
            writer.WriteLine(@"\binoppenalty=10000");
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
