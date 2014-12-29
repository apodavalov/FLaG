using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class TransitionTest
    {
        private Tuple<Transition, Transition, int>[] _Expectations = new Tuple<Transition, Transition, int>[]
        {
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                0),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'e', new Label(new SingleLabel('S', subIndex: 2))),
                -1),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'c', new Label(new SingleLabel('P', subIndex: 2))),
                1),
           new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                new Transition(new Label(new SingleLabel('P', subIndex: 1)), 'e', new Label(new SingleLabel('O', subIndex: 2))),
                3),
           new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2))),
                null,
                1),
           new Tuple<Transition, Transition, int>(
                null,
                null,
                0)
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_CurrentStateNull_Fail()
        {
            new Transition(null, 'c', new Label(new SingleLabel('S', subIndex: 1)));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_NextStateNull_Fail()
        {
            new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'c', null);
        }

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
        public void GetHashCodeTest()
        {
            Transition transition = new Transition(new Label(new SingleLabel('S', subIndex: 1)), 'd', new Label(new SingleLabel('S', subIndex: 2)));

            Assert.AreEqual(6553703,transition.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("δ({S_null_null_null},c) -> {P_null_null_null}",
                new Transition(new Label(new SingleLabel('S')), 'c', new Label(new SingleLabel('P'))).ToString());
        }
    }
}
