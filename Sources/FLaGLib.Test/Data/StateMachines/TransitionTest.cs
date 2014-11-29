using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Test.Helpers;
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
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'d'),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'d'),
                0),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'d'),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'e'),
                -1),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'d'),
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('P', subIndex: 2)),'c'),
                3),
           new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', subIndex: 1)), new Label(new SingleLabel('S', subIndex: 2)),'d'),
                new Transition(new Label(new SingleLabel('P', subIndex: 1)), new Label(new SingleLabel('O', subIndex: 2)),'e'),
                3)
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentStateNull_Fail()
        {
            new Transition(null, new Label(new SingleLabel('S', subIndex: 1)), 'c');
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextStateNull_Fail()
        {
            new Transition(new Label(new SingleLabel('S', subIndex: 1)), null, 'c');
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
        public void ToStringTest()
        {
            Assert.AreEqual("δ({S_null_null_null},c) -> {P_null_null_null}", 
                new Transition(new Label(new SingleLabel('S')), new Label(new SingleLabel('P')),'c').ToString());
        }
    }
}
