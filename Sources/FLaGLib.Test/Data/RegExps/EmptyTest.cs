using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class EmptyTest
    {
        private Tuple<Empty, Empty, int>[] _Expectations = new Tuple<Empty, Empty , int>[]
        {
            new Tuple<Empty, Empty, int>(
                null,
                null,
                0
            ),

            new Tuple<Empty, Empty, int>(
                Empty.Instance,
                null,
                1
            ),

            new Tuple<Empty, Empty, int>(
                null,
                Empty.Instance,
                -1
            ),

            new Tuple<Empty, Empty, int>(
                Empty.Instance,
                Empty.Instance,
                0
            ),
        };

        [Test]
        public void InstanceTest_Ok()
        {
            Empty empty = Empty.Instance;
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
            Empty empty = Empty.Instance;
            Assert.AreEqual(empty.ToString(), "ε");
        }

        [Test]
        public void PriorityTest()
        {
            Empty empty = Empty.Instance;
            Assert.AreEqual(empty.Priority, 0);
        }
    }
}
