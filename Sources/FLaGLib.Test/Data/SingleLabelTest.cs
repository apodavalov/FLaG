using FLaGLib.Data;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data
{
    [TestFixture]
    public class SingleLabelTest
    {
        private Tuple<SingleLabel, SingleLabel, int>[] _Expectations = new Tuple<SingleLabel, SingleLabel, int>[]
        {
            new Tuple<SingleLabel, SingleLabel, int>(null, null, 0),
            new Tuple<SingleLabel, SingleLabel, int>(null, new SingleLabel('b'), -1),
            new Tuple<SingleLabel, SingleLabel, int>(null, new SingleLabel('b',5), -1),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b'), new SingleLabel('b'), 0),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5), new SingleLabel('b',5), 0)
        };

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("b_null", new SingleLabel('b').ToString());
            Assert.AreEqual("b_5", new SingleLabel('b',5).ToString());
        }
    }
}
