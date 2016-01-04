using FLaG.IO.Input;
using FLaGLib.Collections;
using FLaGLib.Data;
using FLaGLib.Data.Grammars;
using FLaGLib.Data.Languages;
using FLaGLib.Data.RegExps;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FLaG.IO.Output
{
    public static class TaskDescriptionExtensions
    {
        private const string _OriginalLanguageLabel = "originalLanguage";
        private const string _OriginalRegularSetLabel = "originalRegularSet";
        private const string _OriginalRegularExpressionLabel = "originalRegularExpression";
        private const string _RussianCaseIsNotSupportedMessage = "Russian case type {0} is not supported.";
        private const string _OriginalRegularExpressionExpandedLabel = "originalRegularExpressionExpanded";

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

        private static void WriteBody(StreamWriter writer, Entity language, string baseFullFileName)
        {
            Counter diagramCounter = new Counter();

            WriteTask(writer, language);

            Expression expression = WriteCheckLanguageType(writer, language);

            WriteConvertToExpression(writer, expression);

            Tuple<StateMachine, int> leftGrammarStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName, GrammarType.Left);
            Tuple<StateMachine, int> rightGrammarStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName, GrammarType.Right);
            //Tuple<StateMachine, int> expressionStateMachine = ConvertToStateMachine(writer, diagramCounter, expression, baseFullFileName);

            //leftGrammarStateMachine = OptimizeStateMachine(writer, diagramCounter, leftGrammarStateMachine, baseFullFileName);
            //rightGrammarStateMachine = OptimizeStateMachine(writer, diagramCounter, rightGrammarStateMachine, baseFullFileName);
            //expressionStateMachine = OptimizeStateMachine(writer, diagramCounter, expressionStateMachine, baseFullFileName);

            //Expression leftGrammarExpression = ConvertToExpression(writer, leftGrammarStateMachine, expression, GrammarType.Left);
            //Expression rightGrammarExpression = ConvertToExpression(writer, rightGrammarStateMachine, expression, GrammarType.Right);
            //Expression leftStateMachineExpression = ConvertToExpression(writer, expressionStateMachine, expression, GrammarType.Left);
            //Expression rightStateMachineExpression = ConvertToExpression(writer, expressionStateMachine, expression, GrammarType.Right);

            //ConvertToEntity(writer, expression, language);
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
            throw new NotImplementedException();
        }

        private static Expression ConvertToExpression(StreamWriter writer, Tuple<StateMachine, int> stateMachine, Expression result, GrammarType grammarType)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> OptimizeStateMachine(StreamWriter writer, Counter diagramCounter, Tuple<StateMachine, int> leftGrammarStateMachine, string baseFullFileName)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter writer, Counter diagramCounter,
            Expression expression, string baseFullFileName)
        {
            throw new NotImplementedException();
        }

        private static Tuple<StateMachine, int> ConvertToStateMachine(StreamWriter writer, Counter diagramCounter,
            Expression expression, string baseFullFileName, GrammarType grammarType)
        {
            WriteSection(writer, grammarType, "Этап 2.3");

            Tuple<Grammar,int> grammar = ConvertToGrammar(writer, expression, grammarType);

            grammar = OptimizeGrammar(writer, grammar, grammarType);

            return null;
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

            return grammar;
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
            IReadOnlyDictionary<NonTerminalSymbol,
            IReadOnlySet<NonTerminalSymbol>> previousMap, 
            IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> consideredNextMap,
            IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> notConsideredNextMap,
            IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> newMap)
        {
            bool first = true;

            foreach (KeyValuePair<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> value in previousMap)
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
                WriteSymbolSet(writer, newMap[value.Key]);
                writer.Write(@" = ");
                WriteSymbolSet(writer, previousMap[value.Key]);
                writer.Write(@" \cup ");
                WriteSymbolSet(writer, newMap[value.Key]);
                writer.Write(@" = ");

                if (consideredNextMap.ContainsKey(value.Key))
                {
                    WriteSymbolSet(writer, consideredNextMap[value.Key]);
                }
                else
                {
                    WriteSymbolSet(writer, notConsideredNextMap[value.Key]);
                }

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
            WriteNonTerminalSymbolMap(writer, postReport.Iteration, 
                postReport.PreviousMap, postReport.ConsideredNextMap,
                postReport.NotConsideredNextMap,
                postReport.NewMap);
            writer.WriteLine(@".");
            writer.Write(@"Построение множеств ");
            writer.Write(@"\begin{math}");
            writer.Write(@"{N_i}^X");
            writer.Write(@"\end{math}");
            writer.Write(@" для нетерминалов ");
            writer.Write(@"\begin{math}");
            WriteSymbolSet(writer, postReport.NotConsideredNextMap.Select(m => m.Key));
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
                WriteSymbolSet(writer, postReport.ConsideredNextMap.Select(m => m.Key));
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
            
            foreach (KeyValuePair<NonTerminalSymbol, ChainRulesTuple> value in postReport.SymbolMap)
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
            WriteSection(writer, grammarType, string.Format("Этап 2.3.2.{0}", substep), 2);

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
            WriteSection(writer, grammarType, string.Format("Этап 2.3.2.{0}", substep), 2);

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
            writer.Write("Построим ");
            writer.WriteLatex(GetGrammarTypeRussianName(grammarType, RussianCaseType.Accusative));
            writer.Write(" грамматику для выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Воспользуемся рекурсивным определением регулярного выражения для построения последовательности ");
            writer.WriteLatex(GetGrammarTypeRussianName(grammarType, RussianCaseType.Genitive, false));
            writer.Write(" грамматик для каждого элементарного выражения, входящих в состав выражения ");
            WriteEquationRef(writer, _OriginalRegularExpressionLabel);
            writer.Write(". Собственно последняя грамматика и будет являться искомой. Определим совокупность выражений, ");
            writer.Write("входящих в состав ");
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

            writer.Write("Далее будем обозначать полученную грамматику ");
            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammarNumber);
            writer.Write(@"\end{math}");
            writer.Write(@" следующим образом: ");

            grammarNumber = 1;

            writer.Write(@"\begin{math}");
            WriteGrammarSign(writer, grammarNumber);
            writer.WriteLine(@"\end{math}. Другие грамматики, полученные на данном этапе использоваться не будут, ссылок на них далее по тексту тоже не будет.");
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
                        writer.Write(", ");
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
            writer.WriteLatex(symbol.Symbol.ToString());
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
                        writer.Write(", ");
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
            foreach (SingleLabel singleLabel in label.Sublabels)
            {
                WriteSingleLabel(writer, singleLabel);
            }
        }

        private static void WriteSingleLabel(StreamWriter writer, SingleLabel singleLabel)
        {
            WriteSymbol(writer, singleLabel.Sign.ToString(), singleLabel.SignIndex);
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
                writer.WriteLatex(value.Value.ToString());
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
            WriteSection(writer, title, "регулярное выражение", subcount);
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
            writer.WriteLine(@"\end{equation}.");
        }

        private static void WriteEquationLabel(StreamWriter writer, string uniqueId, GrammarType grammarType)
        {
            writer.Write(@"\label{eq:");
            writer.WriteLatex(string.Concat(uniqueId,grammarType.ToString()));
            writer.Write(@"}");
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
            writer.WriteLatex(sign.ToString());
            writer.Write("_{");
            writer.WriteLatex(number.ToString());
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
            writer.WriteLatex((index + 1).ToString());
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
            writer.WriteLatex(constIteration.IterationCount.ToString());
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
            writer.WriteLatex(symbol.Character.ToString());
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
            writer.WriteLatex(symbol.Character.ToString());
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
            writer.WriteLatex(constIteration.IterationCount.ToString());
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
            writer.WriteLatex(symbol.Character.ToString());
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
            writer.WriteLatex(variable.Name.ToString());
        }

        private static void WriteQuantity(StreamWriter writer, FLaGLib.Data.Languages.Quantity quantity)
        {
            writer.WriteLatex(quantity.Count.ToString());
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
                writer.WriteLatex(variable.Name.ToString());
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

                writer.WriteLatex(variable.Number.ToString());
                writer.Write(@", ");
            }

            writer.Write(@"\text{где } ");

            writer.WriteLatex(string.Join(", ", variables.Select(v => v.Name.ToString())));

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
