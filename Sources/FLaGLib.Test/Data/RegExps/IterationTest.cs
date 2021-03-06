﻿using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class IterationTest
    {
        private Tuple<Iteration, Iteration, int>[] _Expectations = new Tuple<Iteration, Iteration, int>[]
        {
            new Tuple<Iteration, Iteration, int>(
                null,
                null,
                0
            ),

            new Tuple<Iteration, Iteration, int>(
                new Iteration(new Symbol('a'), true),
                null,
                1
            ),

            new Tuple<Iteration, Iteration, int>(
                null,
                new Iteration(new Symbol('a'), true),
                -1
            ),

            new Tuple<Iteration, Iteration, int>(
                new Iteration(new Symbol('a'), true),
                new Iteration(new Symbol('a'), true),
                0
            ),

            new Tuple<Iteration, Iteration, int>(
                new Iteration(new Symbol('b'), true),
                new Iteration(new Symbol('a'), true),
                1
            ),

            new Tuple<Iteration, Iteration, int>(
                new Iteration(new Symbol('a'), true),
                new Iteration(new Symbol('b'), true),
                -1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            Iteration iteration = new Iteration(new Symbol('a'), true);
            iteration = new Iteration(new Symbol('a'), false);
        }

        [Test]
        public void CctorTest_SetNullExpression_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new Iteration(null, true));
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
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Iteration iteration = new Iteration(new Symbol('a'), true);
            Assert.AreEqual(iteration.ToString(), "a^(+)");
            iteration = new Iteration(new Symbol('a'), false);
            Assert.AreEqual(iteration.ToString(), "a^(*)");
        }

        [Test]
        public void PriorityTest()
        {
            Iteration iteration = new Iteration(new Symbol('a'), true);
            Assert.AreEqual(iteration.Priority, 1);
        }
    }
}
