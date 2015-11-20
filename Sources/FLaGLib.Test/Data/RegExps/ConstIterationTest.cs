using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class ConstIterationTest
    {
        private Tuple<ConstIteration, ConstIteration, int>[] _Expectations = new Tuple<ConstIteration, ConstIteration, int>[]
        {
            new Tuple<ConstIteration, ConstIteration, int>(
                null,
                null,
                0
            ),

            new Tuple<ConstIteration, ConstIteration, int>(
                new ConstIteration(new Symbol('a'), 1),
                null,
                1
            ),

            new Tuple<ConstIteration, ConstIteration, int>(
                null,
                new ConstIteration(new Symbol('a'), 1),
                -1
            ),

            new Tuple<ConstIteration, ConstIteration, int>(
                new ConstIteration(new Symbol('a'), 1),
                new ConstIteration(new Symbol('a'), 1),
                0
            ),

            new Tuple<ConstIteration, ConstIteration, int>(
                new ConstIteration(new Symbol('b'), 1),
                new ConstIteration(new Symbol('a'), 1),
                1
            ),

            new Tuple<ConstIteration, ConstIteration, int>(
                new ConstIteration(new Symbol('a'), 1),
                new ConstIteration(new Symbol('b'), 1),
                -1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            ConstIteration constIteration = new ConstIteration(new Symbol('a'), 0);
        }

        [Test]
        public void CctorTest_SetNullExpression_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new ConstIteration(null, 0));
        }

        [Test]
        public void CctorTest_SetIterationLessZero_Fail()
        {
            Assert.Throws<ArgumentException>(() => new ConstIteration(new Symbol('a'), -1));
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
            ConstIteration constIteration = new ConstIteration(new Symbol('a'), 1);
            Assert.AreEqual(constIteration.ToString(), "a^(1)");
        }
    }
}
