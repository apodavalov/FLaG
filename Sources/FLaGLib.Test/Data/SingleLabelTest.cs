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
            new Tuple<SingleLabel, SingleLabel, int>(null, new SingleLabel('b',5,6), -1),
            new Tuple<SingleLabel, SingleLabel, int>(null, new SingleLabel('b',5,6,7), -1),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b'), new SingleLabel('b'), 0),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5), new SingleLabel('b',5), 0),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5,6), new SingleLabel('b',5,6), 0),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5,6,7), new SingleLabel('b',5,6,7), 0),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',4,3,2), new SingleLabel('c',4,3,2), -1),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5,3,2), new SingleLabel('b',6,3,2), -1),
            new Tuple<SingleLabel, SingleLabel, int>(new SingleLabel('b',5,6,7), new SingleLabel('b',5,6,2), 1)
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
            Assert.AreEqual("b_null_null_null", new SingleLabel('b').ToString());
            Assert.AreEqual("b_5_null_null", new SingleLabel('b',5).ToString());
            Assert.AreEqual("b_5_6_null", new SingleLabel('b', 5, 6).ToString());
            Assert.AreEqual("b_5_6_7", new SingleLabel('b', 5, 6, 7).ToString());
        }

        [Test]
        public void NextTest_Ok()
        {
            SingleLabel expectedLabel = new SingleLabel('b', 0, 1, 1);
            SingleLabel actualLabel = new SingleLabel('b', 0, 1,0).Next();
            Assert.AreEqual(expectedLabel, actualLabel);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTest_SubIndexHasNoValue_Fail()
        {
            new SingleLabel('b', 0, 1).Next();
        }
    }
}
