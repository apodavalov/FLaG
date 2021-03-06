﻿using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class BinaryUnionTest
    {
        private Tuple<BinaryUnion, BinaryUnion, int>[] _Expectations = new Tuple<BinaryUnion, BinaryUnion, int>[]
        {
            new Tuple<BinaryUnion, BinaryUnion, int>(
                null,
                null,
                0
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                null,
                1
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                null,
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                -1
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                new BinaryUnion(new Symbol('b'), new Symbol('a')),
                0
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('c'), new Symbol('d')),
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                2
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                new BinaryUnion(new Symbol('c'), new Symbol('d')),
                -2
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('c'), new Symbol('d')),
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                2
            ),

            new Tuple<BinaryUnion, BinaryUnion, int>(
                new BinaryUnion(new Symbol('a'), new Symbol('b')),
                new BinaryUnion(new Symbol('c'), new Symbol('d')),
                -2
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            BinaryUnion binatyUnion = new BinaryUnion(new Symbol('a'), new Symbol('b'));
        }

        [Test]
        public void CctorTest_LeftExpressionIsNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new BinaryUnion(null, new Symbol('b')));
        }

        [Test]
        public void CctorTest_RightExpressionIsNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new BinaryUnion(new Symbol('a'), null));
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void EqualTest()
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
            BinaryUnion binaryUnion = new BinaryUnion(new Symbol('a'), new Symbol('b'));
            Assert.AreEqual("a + b", binaryUnion.ToString());
        }
    }
}
