using FLaGLib.Collections;
using FLaGLib.Data;
using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.Data.Grammars
{
    [TestFixture]
    public class GrammarTest
    {
        [Test]
        public void MakeStateMachineGrammar_Left_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));
            NonTerminalSymbol s6 = new NonTerminalSymbol(new Label(new SingleLabel('S', 6)));
            NonTerminalSymbol s7 = new NonTerminalSymbol(new Label(new SingleLabel('S', 7)));

            TerminalSymbol a = new TerminalSymbol('a');
            TerminalSymbol b = new TerminalSymbol('b');
            TerminalSymbol c = new TerminalSymbol('c');
            TerminalSymbol d = new TerminalSymbol('d');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2,
                                    a,
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3,
                                    a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a,
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b,
                                    c,
                                    d
                                )
                            )
                        ), s3
                    )
                ), s1
            );

            Grammar expectedGrammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4,
                                    b
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3,
                                    a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s5,
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s7,
                                    d
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2,
                                    a
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            )
                        ), s5
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            )
                        ), s6
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6,
                                    c
                                )
                            )
                        ), s7
                    )
                ), s1
            );

            Grammar actualGrammar = grammar.MakeStateMachineGrammar(GrammarType.Left);

            Assert.AreEqual(expectedGrammar, actualGrammar);

            Assert.Fail("Check also onIterate");
        }

        [Test]
        public void MakeStateMachineGrammar_Right_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));
            NonTerminalSymbol s6 = new NonTerminalSymbol(new Label(new SingleLabel('S', 6)));
            NonTerminalSymbol s7 = new NonTerminalSymbol(new Label(new SingleLabel('S', 7)));

            TerminalSymbol a = new TerminalSymbol('a');
            TerminalSymbol b = new TerminalSymbol('b');
            TerminalSymbol c = new TerminalSymbol('c');
            TerminalSymbol d = new TerminalSymbol('d');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b,
                                    a,
                                    s2
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a,
                                    s3
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b,
                                    a
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    d,
                                    c,
                                    b
                                )
                            )
                        ), s3
                    )
                ), s1
            );

            Grammar expectedGrammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b,
                                    s4
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a,
                                    s3
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b,
                                    s5
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    d,
                                    s7
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a,
                                    s2
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            )
                        ), s5
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            )
                        ), s6
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c,
                                    s6
                                )
                            )
                        ), s7
                    )
                ), s1
            );

            Grammar actualGrammar = grammar.MakeStateMachineGrammar(GrammarType.Right);

            Assert.AreEqual(expectedGrammar, actualGrammar);

            Assert.Fail("Check also onIterate");
        }

        [Test]
        public void RemoveChainRulesTest_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));

            TerminalSymbol c = new TerminalSymbol('c');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s5
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s5
                    )
                ), s1
            );

            Grammar expectedGrammar = new Grammar(
               EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s5
                    )
                ), s1
            );

            Grammar actualGrammar;

            ChainRulesBeginPostReport expectedBegin = new ChainRulesBeginPostReport(0, 
                new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                {
                    { s1, EnumerateHelper.Sequence(s1).ToHashSet() },
                    { s2, EnumerateHelper.Sequence(s2).ToHashSet() },
                    { s3, EnumerateHelper.Sequence(s3).ToHashSet() },
                    { s4, EnumerateHelper.Sequence(s4).ToHashSet() },
                    { s5, EnumerateHelper.Sequence(s5).ToHashSet() }
                });

            IReadOnlyList<ChainRulesIterationPostReport> expectedIterationPostReports = EnumerateHelper.Sequence(
                new ChainRulesIterationPostReport(
                    1,
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s3).ToHashSet() },
                        { s4, EnumerateHelper.Sequence(s4).ToHashSet() },
                        { s5, EnumerateHelper.Sequence(s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s2).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s3).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s4).ToHashSet() },
                        { s4, EnumerateHelper.Sequence(s5).ToHashSet() },
                        { s5, EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s3,s4).ToHashSet() },
                        { s4, EnumerateHelper.Sequence(s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s5, EnumerateHelper.Sequence(s5).ToHashSet() }
                    },
                    false
                ),
                new ChainRulesIterationPostReport(
                    2,
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s3,s4).ToHashSet() },
                        { s4, EnumerateHelper.Sequence(s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s3).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s4).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s5).ToHashSet() },
                        { s4, EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3,s4).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s4, EnumerateHelper.Sequence(s4,s5).ToHashSet() }
                    },
                    false
                ),
                new ChainRulesIterationPostReport(
                    3,
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3,s4).ToHashSet() },
                        { s3, EnumerateHelper.Sequence(s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s4).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s5).ToHashSet() },
                        { s3, EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3,s4).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s3, EnumerateHelper.Sequence(s3,s4,s5).ToHashSet() }
                    },
                    false
                ),
                new ChainRulesIterationPostReport(
                    4,
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3,s4).ToHashSet() },
                        { s2, EnumerateHelper.Sequence(s2,s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s5).ToHashSet() },
                        { s2, EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s2, EnumerateHelper.Sequence(s2,s3,s4,s5).ToHashSet() }
                    },
                    false
                ),
                new ChainRulesIterationPostReport(
                    5,
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3,s4,s5).ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet() }
                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {

                    },
                    new Dictionary<NonTerminalSymbol, ISet<NonTerminalSymbol>>
                    {
                        { s1, EnumerateHelper.Sequence(s1,s2,s3,s4,s5).ToHashSet() }
                    },
                    true
                )
            ).ToList().AsReadOnly();

            ChainRulesEndPostReport expectedEnd = new ChainRulesEndPostReport(
                new Dictionary<NonTerminalSymbol, ChainRulesTuple>
                {
                    { s1, new ChainRulesTuple(EnumerateHelper.Sequence(s1,s2,s3,s4,s5).ToHashSet(), 5) },
                    { s2, new ChainRulesTuple(EnumerateHelper.Sequence(s2,s3,s4,s5).ToHashSet(), 4) },
                    { s3, new ChainRulesTuple(EnumerateHelper.Sequence(s3,s4,s5).ToHashSet(), 3) },
                    { s4, new ChainRulesTuple(EnumerateHelper.Sequence(s4,s5).ToHashSet(), 2) },
                    { s5, new ChainRulesTuple(EnumerateHelper.Sequence(s5).ToHashSet(), 1) }
                },
                new Dictionary<NonTerminalSymbol, ChainRulesTuple>
                {
                    { s1, new ChainRulesTuple(EnumerateHelper.Sequence(s2,s3,s4,s5).ToHashSet(), 5) },
                    { s2, new ChainRulesTuple(EnumerateHelper.Sequence(s3,s4,s5).ToHashSet(), 4) },
                    { s3, new ChainRulesTuple(EnumerateHelper.Sequence(s4,s5).ToHashSet(), 3) },
                    { s4, new ChainRulesTuple(EnumerateHelper.Sequence(s5).ToHashSet(), 2) },
                    { s5, new ChainRulesTuple(EnumerateHelper.Sequence<NonTerminalSymbol>().ToHashSet(), 1) }
                }
            );

            int actualPostReportCount = 0;
            bool beginInvoked = false;
            bool endInvoked = false;

            Assert.IsTrue(grammar.RemoveChainRules(out actualGrammar,
                tuple =>
                {
                    Assert.AreEqual(0, actualPostReportCount);

                    beginInvoked = OnTuple(tuple, expectedBegin, beginInvoked, endInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedIterationPostReports, beginInvoked, endInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    endInvoked = OnTuple(tuple, expectedEnd, beginInvoked, endInvoked, actualPostReportCount, expectedIterationPostReports.Count);
                }));

            Assert.AreEqual(expectedIterationPostReports.Count, actualPostReportCount);
            Assert.IsTrue(beginInvoked);
            Assert.IsTrue(endInvoked);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        private bool OnTuple(ChainRulesBeginPostReport actualBegin, ChainRulesBeginPostReport expectedBegin, bool beginInvoked, bool endInvoked, int actualPostReportProcessedCount)
        {
            Assert.IsFalse(beginInvoked);
            Assert.IsFalse(endInvoked);
            Assert.AreEqual(0, actualPostReportProcessedCount);

            Assert.AreEqual(expectedBegin.Iteration, actualBegin.Iteration);
            AssertSymbolMap(expectedBegin.SymbolMap, actualBegin.SymbolMap);

            return true;
        }

        private void AssertSymbolMap(IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> expected, IReadOnlyDictionary<NonTerminalSymbol, IReadOnlySet<NonTerminalSymbol>> actual)
        {
            CollectionAssert.AreEquivalent(expected.Keys, actual.Keys);

            foreach (NonTerminalSymbol symbol in expected.Keys)
            {
                CollectionAssert.AreEquivalent(expected[symbol], actual[symbol]);
            }
        }

        private void AssertSymbolMap(IReadOnlyDictionary<NonTerminalSymbol, ChainRulesTuple> expected, IReadOnlyDictionary<NonTerminalSymbol, ChainRulesTuple> actual)
        {
            CollectionAssert.AreEquivalent(expected.Keys, actual.Keys);

            foreach (NonTerminalSymbol symbol in expected.Keys)
            {
                ChainRulesTuple actualChainRulesTuple = actual[symbol];
                ChainRulesTuple expectedChainRulesTuple = expected[symbol];

                CollectionAssert.AreEquivalent(expectedChainRulesTuple.NonTerminals, actualChainRulesTuple.NonTerminals);
                Assert.AreEqual(expectedChainRulesTuple.Iteration, actualChainRulesTuple.Iteration);
            }
        }

        private int OnTuple(ChainRulesIterationPostReport actualCurrentIteration, IReadOnlyList<ChainRulesIterationPostReport> expectedIterations, bool beginInvoked, bool endInvoked, int actualPostReportProcessedCount)
        {
            Assert.IsTrue(beginInvoked);
            Assert.IsFalse(endInvoked);

            Assert.IsTrue(actualPostReportProcessedCount < expectedIterations.Count);

            ChainRulesIterationPostReport expectedCurrentIteration = expectedIterations[actualPostReportProcessedCount];

            Assert.AreEqual(expectedCurrentIteration.Iteration, actualCurrentIteration.Iteration);
            AssertSymbolMap(expectedCurrentIteration.PreviousMap, actualCurrentIteration.PreviousMap);
            AssertSymbolMap(expectedCurrentIteration.NewMap, actualCurrentIteration.NewMap);
            AssertSymbolMap(expectedCurrentIteration.ConsideredNextMap, actualCurrentIteration.ConsideredNextMap);
            AssertSymbolMap(expectedCurrentIteration.NotConsideredNextMap, actualCurrentIteration.NotConsideredNextMap);
            Assert.AreEqual(expectedCurrentIteration.IsLastIteration, actualCurrentIteration.IsLastIteration);

            return actualPostReportProcessedCount + 1;
        }

        private bool OnTuple(ChainRulesEndPostReport actualEnd, ChainRulesEndPostReport expectedEnd, bool beginInvoked, bool endInvoked, int actualPostReportProcessedCount, int expectedPostReportProcessedCount)
        {
            Assert.IsTrue(beginInvoked);
            Assert.IsFalse(endInvoked);
            Assert.AreEqual(expectedPostReportProcessedCount, actualPostReportProcessedCount);

            AssertSymbolMap(expectedEnd.SymbolMap, actualEnd.SymbolMap);
            AssertSymbolMap(expectedEnd.SymbolMapFinal, actualEnd.SymbolMapFinal);

            return true;
        }

        [Test]
        public void RemoveUnreachableSymbolsTest_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));
            NonTerminalSymbol s6 = new NonTerminalSymbol(new Label(new SingleLabel('S', 6)));
            NonTerminalSymbol s7 = new NonTerminalSymbol(new Label(new SingleLabel('S', 7)));
            NonTerminalSymbol s8 = new NonTerminalSymbol(new Label(new SingleLabel('S', 8)));

            TerminalSymbol c = new TerminalSymbol('c');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2, s3
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4, s5
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6, s7
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2, s5
                                )
                            )
                        ), s8
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s4
                    )
                ), s1
            );

            Grammar expectedGrammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2, s3
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4, s5
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6, s7
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            )
                        ), s4
                    )
                ), s1
            );

            Grammar actualGrammar;

            BeginPostReport<Symbol> expectedBegin = new BeginPostReport<Symbol>(0, EnumerateHelper.Sequence(s1));
            IReadOnlyList<IterationPostReport<Symbol>> expectedPostReports = EnumerateHelper.Sequence(
                new IterationPostReport<Symbol>(
                    1,
                    EnumerateHelper.Sequence(s1),
                    EnumerateHelper.Sequence(s2,s3,s4,s5),
                    EnumerateHelper.Sequence(s1,s2,s3,s4,s5),
                    false
                ),
                new IterationPostReport<Symbol>(
                    2,
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5),
                    EnumerateHelper.Sequence<Symbol>(s6,s7,c),
                    EnumerateHelper.Sequence<Symbol>(s1, s2, s3, s4, s5, s6, s7, c),
                    false
                ),
                new IterationPostReport<Symbol>(
                    3,
                    EnumerateHelper.Sequence<Symbol>(s1, s2, s3, s4, s5, s6, s7, c),
                    EnumerateHelper.Sequence<Symbol>(),
                    EnumerateHelper.Sequence<Symbol>(s1, s2, s3, s4, s5, s6, s7, c),
                    true
                )
            ).ToList().AsReadOnly();

            int actualPostReportCount = 0;
            bool beginInvoked = false;

            Assert.IsTrue(grammar.RemoveUnreachableSymbols(out actualGrammar,
                tuple =>
                {
                    Assert.AreEqual(0, actualPostReportCount);

                    beginInvoked = OnTuple(tuple, expectedBegin, beginInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedPostReports, beginInvoked, actualPostReportCount);
                }));

            Assert.AreEqual(expectedPostReports.Count, actualPostReportCount);
            Assert.IsTrue(beginInvoked);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        [Test]
        public void RemoveUselessSymbolsTest_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));

            TerminalSymbol a = new TerminalSymbol('a');
            TerminalSymbol c = new TerminalSymbol('c');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2, c
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s1, s3
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, s3
                                )
                            )
                        ), s3
                    )
                ), s2
            );

            Grammar expectedGrammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2, c
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            )
                        ), s2
                    )
                ), s2
            );

            Grammar actualGrammar;

            BeginPostReport<NonTerminalSymbol> expectedBegin = new BeginPostReport<NonTerminalSymbol>(0, EnumerateHelper.Sequence<NonTerminalSymbol>());
            IReadOnlyList<IterationPostReport<NonTerminalSymbol>> expectedPostReports = EnumerateHelper.Sequence(
                new IterationPostReport<NonTerminalSymbol>(
                    1,
                    EnumerateHelper.Sequence<NonTerminalSymbol>(),
                    EnumerateHelper.Sequence(s2),
                    EnumerateHelper.Sequence(s2),
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    2,
                    EnumerateHelper.Sequence(s2),
                    EnumerateHelper.Sequence(s1),
                    EnumerateHelper.Sequence(s1,s2),
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    3,
                    EnumerateHelper.Sequence(s1, s2),
                    EnumerateHelper.Sequence<NonTerminalSymbol>(),
                    EnumerateHelper.Sequence(s1, s2),
                    true
                )
            ).ToList().AsReadOnly();

            int actualPostReportCount = 0;
            bool beginInvoked = false;

            Assert.IsTrue(grammar.RemoveUselessSymbols(out actualGrammar,
                tuple =>
                {
                    Assert.AreEqual(0, actualPostReportCount);

                    beginInvoked = OnTuple(tuple, expectedBegin, beginInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedPostReports, beginInvoked, actualPostReportCount);
                }));

            Assert.AreEqual(expectedPostReports.Count, actualPostReportCount);
            Assert.IsTrue(beginInvoked);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        [Test]
        public void RemoveEmptyRulesTest_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));
            NonTerminalSymbol s6 = new NonTerminalSymbol(new Label(new SingleLabel('S', 6)));
            NonTerminalSymbol s10 = new NonTerminalSymbol(new Label(new SingleLabel('S', 10)));
            NonTerminalSymbol s11 = new NonTerminalSymbol(new Label(new SingleLabel('S', 11)));
            NonTerminalSymbol s12 = new NonTerminalSymbol(new Label(new SingleLabel('S', 12)));
            NonTerminalSymbol s14 = new NonTerminalSymbol(new Label(new SingleLabel('S', 14)));

            TerminalSymbol a = new TerminalSymbol('a');
            TerminalSymbol b = new TerminalSymbol('b');
            TerminalSymbol c = new TerminalSymbol('c');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10, a
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s1, a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, b
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3, b
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, s10, s11, c
                                )
                            )
                        ), s5
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s5, c
                                )
                            )
                        ), s6
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2
                                )
                            )
                        ), s10
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4
                                )
                            )
                        ), s11
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11
                                )
                            )
                        ), s12
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12
                                )
                            )
                        ), s14
                    )
                ), s14
            );

            Grammar expectedGrammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10,a
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s1, a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, b
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3, b
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, s10, s11, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, s10, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, s11, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a, c
                                )
                            )
                        ), s5
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s5, c
                                )
                            )
                        ), s6
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2
                                )
                            )
                        ), s10
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4
                                )
                            )
                        ), s11
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11
                                )
                            )
                        ), s12
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12
                                )
                            )
                        ), s14
                    )
                ), s14
            );
            
            Grammar actualGrammar;

            BeginPostReport<NonTerminalSymbol> expectedBegin = new BeginPostReport<NonTerminalSymbol>(0, EnumerateHelper.Sequence(s10, s11));
            IReadOnlyList<IterationPostReport<NonTerminalSymbol>> expectedPostReports = EnumerateHelper.Sequence(
                new IterationPostReport<NonTerminalSymbol>(
                    1, 
                    EnumerateHelper.Sequence(s10, s11), 
                    EnumerateHelper.Sequence(s12, s14),
                    EnumerateHelper.Sequence(s10, s11, s12, s14), 
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    2,
                    EnumerateHelper.Sequence(s10, s11, s12, s14),
                    EnumerateHelper.Sequence<NonTerminalSymbol>(),
                    EnumerateHelper.Sequence(s10, s11, s12, s14),
                    true
                )
            ).ToList().AsReadOnly();

            int actualPostReportCount = 0;
            bool beginInvoked = false;

            Assert.IsTrue(grammar.RemoveEmptyRules(out actualGrammar,
                tuple =>
                { 
                    Assert.AreEqual(0, actualPostReportCount);

                    beginInvoked = OnTuple(tuple, expectedBegin, beginInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedPostReports, beginInvoked, actualPostReportCount);
                }));

            Assert.AreEqual(expectedPostReports.Count, actualPostReportCount);
            Assert.IsTrue(beginInvoked);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        [Test]
        public void IsLangEmptyTest_Ok()
        {
            NonTerminalSymbol s1 = new NonTerminalSymbol(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol s2 = new NonTerminalSymbol(new Label(new SingleLabel('S', 2)));
            NonTerminalSymbol s3 = new NonTerminalSymbol(new Label(new SingleLabel('S', 3)));
            NonTerminalSymbol s4 = new NonTerminalSymbol(new Label(new SingleLabel('S', 4)));
            NonTerminalSymbol s5 = new NonTerminalSymbol(new Label(new SingleLabel('S', 5)));
            NonTerminalSymbol s6 = new NonTerminalSymbol(new Label(new SingleLabel('S', 6)));
            NonTerminalSymbol s10 = new NonTerminalSymbol(new Label(new SingleLabel('S', 10)));
            NonTerminalSymbol s11 = new NonTerminalSymbol(new Label(new SingleLabel('S', 11)));
            NonTerminalSymbol s12 = new NonTerminalSymbol(new Label(new SingleLabel('S', 12)));
            NonTerminalSymbol s14 = new NonTerminalSymbol(new Label(new SingleLabel('S', 14)));

            TerminalSymbol a = new TerminalSymbol('a');
            TerminalSymbol b = new TerminalSymbol('b');
            TerminalSymbol c = new TerminalSymbol('c');

            Grammar grammar = new Grammar(
                EnumerateHelper.Sequence(
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    a
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10, a
                                )
                            )
                        ), s1
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s1, a
                                )
                            )
                        ), s2
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    b
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, b
                                )
                            )
                        ), s3
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s3, b
                                )
                            )
                        ), s4
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11, c
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12, c
                                )
                            )
                        ), s5
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s5, c
                                )
                            )
                        ), s6
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s2
                                )
                            )
                        ), s10
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            Chain.Empty,
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s4
                                )
                            )
                        ), s11
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s6
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s11
                                )
                            )
                        ), s12
                    ),
                    new Rule(
                        EnumerateHelper.Sequence(
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s10
                                )
                            ),
                            new Chain(
                                EnumerateHelper.Sequence<Symbol>(
                                    s12
                                )
                            )
                        ), s14
                    )
                ), s14
            );
            
            BeginPostReport<NonTerminalSymbol> expectedBegin = new BeginPostReport<NonTerminalSymbol>(0, Enumerable.Empty<NonTerminalSymbol>());
            IReadOnlyList<IterationPostReport<NonTerminalSymbol>> expectedPostReports = EnumerateHelper.Sequence(
                new IterationPostReport<NonTerminalSymbol>(
                    1,
                    EnumerateHelper.Sequence<NonTerminalSymbol>(),
                    EnumerateHelper.Sequence(s1, s3, s10, s11),
                    EnumerateHelper.Sequence(s1, s3, s10, s11),
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    2,
                    EnumerateHelper.Sequence(s1, s3, s10, s11),
                    EnumerateHelper.Sequence(s2, s4, s5, s12, s14),
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5, s10, s11, s12, s14),
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    3,
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5, s10, s11, s12, s14),
                    EnumerateHelper.Sequence(s6),
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5, s6, s10, s11, s12, s14),
                    false
                ),
                new IterationPostReport<NonTerminalSymbol>(
                    4,
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5, s6, s10, s11, s12, s14),
                    EnumerateHelper.Sequence<NonTerminalSymbol>(),
                    EnumerateHelper.Sequence(s1, s2, s3, s4, s5, s6, s10, s11, s12, s14),
                    true
                )
            ).ToList().AsReadOnly();

            int actualPostReportCount = 0;
            bool beginInvoked = false;

            Assert.IsFalse(grammar.IsLangEmpty(
                tuple =>
                {
                    Assert.AreEqual(0, actualPostReportCount);

                    beginInvoked = OnTuple(tuple, expectedBegin, beginInvoked, actualPostReportCount);
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedPostReports, beginInvoked, actualPostReportCount);
                }));

            Assert.AreEqual(expectedPostReports.Count, actualPostReportCount);
            Assert.IsTrue(beginInvoked);
        }

        private bool OnTuple<T>(BeginPostReport<T> actualCurrentIteration, BeginPostReport<T> expectedCurrentIteration, bool beginInvoked, int actualPostReportProcessedCount) where T : Symbol
        {
            Assert.IsFalse(beginInvoked);
            Assert.AreEqual(0, actualPostReportProcessedCount);

            Assert.AreEqual(expectedCurrentIteration.Iteration, actualCurrentIteration.Iteration);

            CollectionAssert.AreEquivalent(expectedCurrentIteration.SymbolSet, actualCurrentIteration.SymbolSet);

            return true;
        }

        private int OnTuple<T>(IterationPostReport<T> actualCurrentIteration, IReadOnlyList<IterationPostReport<T>> expectedIterations, bool beginInvoked, int actualPostReportProcessedCount) where T : Symbol
        {
            Assert.IsTrue(beginInvoked);

            Assert.IsTrue(actualPostReportProcessedCount < expectedIterations.Count);

            IterationPostReport<T> expectedCurrentIteration = expectedIterations[actualPostReportProcessedCount];

            Assert.AreEqual(expectedCurrentIteration.Iteration, actualCurrentIteration.Iteration);
            Assert.AreEqual(expectedCurrentIteration.IsLastIteration, actualCurrentIteration.IsLastIteration);

            CollectionAssert.AreEquivalent(expectedCurrentIteration.NewSymbolSet, actualCurrentIteration.NewSymbolSet);
            CollectionAssert.AreEquivalent(expectedCurrentIteration.NextSymbolSet, actualCurrentIteration.NextSymbolSet);
            CollectionAssert.AreEquivalent(expectedCurrentIteration.PreviousSymbolSet, actualCurrentIteration.PreviousSymbolSet);

            return actualPostReportProcessedCount + 1;
        }
    }
}
