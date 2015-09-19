﻿using FLaGLib.Data;
using FLaGLib.Data.Grammars;
using FLaGLib.Helpers;
using NUnit.Framework;

namespace FLaGLib.Test.Data.Grammars
{
    [TestFixture]
    public class GrammarTest
    {
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

            Assert.IsTrue(grammar.RemoveEmptyRules(out actualGrammar));
            Assert.AreEqual(expectedGrammar, actualGrammar);
            Assert.Fail("Not fully implemented.");
        }
    }
}
