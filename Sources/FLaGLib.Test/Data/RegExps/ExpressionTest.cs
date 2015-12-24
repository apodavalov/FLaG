using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class ExpressionTest
    {
        private Tuple<Expression, Expression, int>[] _Expectations = new Tuple<Expression, Expression, int>[]
        {
            new Tuple<Expression, Expression, int>(
                null,
                null,
                0
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('a'),
                null,
                1
            ),

            new Tuple<Expression, Expression, int>(
                null,
                new Symbol('a'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('a'),
                new Symbol('a'),
                0
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('a'),
                new Symbol('b'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('b'),
                new Symbol('a'),
                1
            ),

            new Tuple<Expression, Expression, int>(
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                new Symbol('c'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                new Symbol('c'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new ConstIteration(new Symbol('a'), 1),
                new Symbol('c'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new ConstIteration(new Symbol('a'), 1),
                null,
                1
            ),

            new Tuple<Expression, Expression, int>(
                Empty.Instance,
                new Symbol('a'),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('a'),
                Empty.Instance,
                1
            ),

            new Tuple<Expression, Expression, int>(
                Empty.Instance,
                null,
                1
            ),

            new Tuple<Expression, Expression, int>(
                null,
                Empty.Instance,
                -1
            ),

            new Tuple<Expression, Expression, int>(
                null,
                new Iteration(new Symbol('a'), true),
                -1
            ),

            new Tuple<Expression, Expression, int>(
                new Symbol('a'),
                new Iteration(new Symbol('a'), true),
                1
            ),
        };

        [Test]
        public void MakeStateMachineTest()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);
            ConstIteration constIteration1 = new ConstIteration(iteration1, 0);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);
            ConstIteration constIteration2 = new ConstIteration(iteration2, 1);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);
            ConstIteration constIteration3 = new ConstIteration(iteration3, 3);

            BinaryUnion union1 = new BinaryUnion(constIteration1, constIteration2);
            BinaryUnion union2 = new BinaryUnion(union1, constIteration3);

            IList<StateMachinePostReport> reports = new List<StateMachinePostReport>();

            FLaGLib.Data.StateMachines.StateMachine stateMachine = union2.MakeStateMachine(d => reports.Add(d));

            Assert.Fail("Not Implemented");
        }

        [Test]
        public void MakeGrammarTest_Left()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);
            ConstIteration constIteration1 = new ConstIteration(iteration1, 0);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);
            ConstIteration constIteration2 = new ConstIteration(iteration2, 1);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);
            ConstIteration constIteration3 = new ConstIteration(iteration3, 3);

            BinaryUnion union1 = new BinaryUnion(constIteration1, constIteration2);
            BinaryUnion union2 = new BinaryUnion(union1, constIteration3);

            IList<GrammarPostReport> reports = new List<GrammarPostReport>();

            FLaGLib.Data.Grammars.Grammar grammar = union2.MakeGrammar(FLaGLib.Data.Grammars.GrammarType.Left,
                d => reports.Add(d));

            Assert.Fail("Not Implemented");
        }

        [Test]
        public void MakeGrammarTest_Right()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);
            ConstIteration constIteration1 = new ConstIteration(iteration1, 0);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);
            ConstIteration constIteration2 = new ConstIteration(iteration2, 1);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);
            ConstIteration constIteration3 = new ConstIteration(iteration3, 3);

            BinaryUnion union1 = new BinaryUnion(constIteration1, constIteration2);
            BinaryUnion union2 = new BinaryUnion(union1, constIteration3);

            IList<GrammarPostReport> reports = new List<GrammarPostReport>();

            FLaGLib.Data.Grammars.Grammar grammar = union2.MakeGrammar(FLaGLib.Data.Grammars.GrammarType.Right,
                d => reports.Add(d));

            Assert.Fail("Not Implemented");
        }

        [Test]
        public void IsRegularSetTest_False()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');
            Empty empty = Empty.Instance;

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, true);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);
                        
            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);            
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);
            BinaryUnion union3 = new BinaryUnion(union2, empty);

            ConstIteration iteration4 = new ConstIteration(symbolC, 4);

            BinaryUnion union4 = new BinaryUnion(union3, iteration4);

            Assert.AreEqual(false, union4.IsRegularSet);
        }

        [Test]
        public void IsRegularSetTest_True()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');
            Empty empty = Empty.Instance;

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, true);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);

            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);
            BinaryUnion union3 = new BinaryUnion(union2, empty);

            Assert.AreEqual(true, union3.IsRegularSet);
        }

        [Test]
        public void ToRegularSetTest()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');
            Empty empty = Empty.Instance;

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, true);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);

            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);
            BinaryUnion union3 = new BinaryUnion(union2, empty);

            ConstIteration iteration4 = new ConstIteration(symbolC, 4);

            BinaryUnion union4 = new BinaryUnion(union3, iteration4);

            Expression actual = union4.ToRegularSet();

            BinaryConcat concat5 = new BinaryConcat(symbolC, symbolC);
            BinaryConcat concat6 = new BinaryConcat(concat5, symbolC);
            BinaryConcat concat7 = new BinaryConcat(concat6, symbolC);

            Expression expected = new BinaryUnion(union3, concat7);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Expression expression = new Symbol('a');
            Assert.AreEqual(expression.ToString(), "a");
        }
    }
}
