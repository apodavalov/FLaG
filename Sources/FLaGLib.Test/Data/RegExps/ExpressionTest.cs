using FLaGLib.Data.RegExps;
using FLaGLib.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using FLaGLib.Extensions;
using System.Linq;
using FLaGLib.Test.TestHelpers;
using System;

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
            )
        };

        [Test]
        public void LeftGrammarTest()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);

            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);

            FLaGLib.Data.Grammars.Grammar grammar = union2.LeftGrammar;

            Assert.Fail("Not Implemented");
        }

        [Test]
        public void RightGrammarTest()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);

            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);

            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);

            FLaGLib.Data.Grammars.Grammar grammar = union2.RightGrammar;

            Assert.Fail("Not Implemented");
        }

        [Test]
        public void WalkDataTest()
        {
            Symbol symbolA = new Symbol('a');
            Symbol symbolB = new Symbol('b');
            Symbol symbolC = new Symbol('c');

            BinaryConcat concat1 = new BinaryConcat(symbolA, symbolB);
            Iteration iteration1 = new Iteration(concat1, false);

            BinaryConcat concat2 = new BinaryConcat(symbolA, symbolC);
            Iteration iteration2 = new Iteration(concat2, false);
            
            BinaryConcat concat3 = new BinaryConcat(symbolB, symbolC);
            Iteration iteration3 = new Iteration(concat3, false);

            BinaryUnion union1 = new BinaryUnion(iteration1, iteration2);
            BinaryUnion union2 = new BinaryUnion(union1, iteration3);

            IReadOnlyList<WalkData<Expression>> actual = union2.WalkData;
            IReadOnlyList<WalkData<Expression>> expected = EnumerateHelper.Sequence<WalkData<Expression>>(
                new WalkData<Expression>(WalkStatus.Begin, 14, union2),
                new WalkData<Expression>(WalkStatus.Begin, 13, union1),
                new WalkData<Expression>(WalkStatus.Begin, 10, iteration1),
                new WalkData<Expression>(WalkStatus.Begin, 7, concat1),
                new WalkData<Expression>(WalkStatus.Begin, 1, symbolA),
                new WalkData<Expression>(WalkStatus.End, 1, symbolA),
                new WalkData<Expression>(WalkStatus.Begin, 2, symbolB),
                new WalkData<Expression>(WalkStatus.End, 2, symbolB),
                new WalkData<Expression>(WalkStatus.End, 7, concat1),
                new WalkData<Expression>(WalkStatus.End, 10, iteration1),
                new WalkData<Expression>(WalkStatus.Begin, 11, iteration2),
                new WalkData<Expression>(WalkStatus.Begin, 8, concat2),
                new WalkData<Expression>(WalkStatus.Begin, 3, symbolA),
                new WalkData<Expression>(WalkStatus.End, 3, symbolA),
                new WalkData<Expression>(WalkStatus.Begin, 4, symbolC),
                new WalkData<Expression>(WalkStatus.End, 4, symbolC),
                new WalkData<Expression>(WalkStatus.End, 8, concat2),
                new WalkData<Expression>(WalkStatus.End, 11, iteration2),
                new WalkData<Expression>(WalkStatus.End, 13, union1),
                new WalkData<Expression>(WalkStatus.Begin, 12, iteration3),
                new WalkData<Expression>(WalkStatus.Begin, 9, concat3),
                new WalkData<Expression>(WalkStatus.Begin, 5, symbolB),
                new WalkData<Expression>(WalkStatus.End, 5, symbolB),
                new WalkData<Expression>(WalkStatus.Begin, 6, symbolC),
                new WalkData<Expression>(WalkStatus.End, 6, symbolC),
                new WalkData<Expression>(WalkStatus.End, 9, concat3),
                new WalkData<Expression>(WalkStatus.End, 12, iteration3),
                new WalkData<Expression>(WalkStatus.End, 14, union2)
                ).ToList().AsReadOnly();

            CollectionAssert.AreEqual(expected, actual);
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
