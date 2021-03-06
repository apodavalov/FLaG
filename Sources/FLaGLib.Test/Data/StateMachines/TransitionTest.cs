﻿using FLaGLib.Data;
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
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                0),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                new Transition(new Label(new SingleLabel('S', 1)), 'e', new Label(new SingleLabel('S', 2))),
                -1),
            new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                new Transition(new Label(new SingleLabel('S', 1)), 'c', new Label(new SingleLabel('P', 2))),
                1),
           new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                new Transition(new Label(new SingleLabel('P', 1)), 'e', new Label(new SingleLabel('O', 2))),
                3),
           new Tuple<Transition, Transition, int>(
                new Transition(new Label(new SingleLabel('S', 1)), 'd', new Label(new SingleLabel('S', 2))),
                null,
                1),
           new Tuple<Transition, Transition, int>(
                null,
                null,
                0)
        };

        [Test]
        public void CctorTest_CurrentStateNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new Transition(null, 'c', new Label(new SingleLabel('S', 1))));
        }

        [Test]
        public void CctorTest_NextStateNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new Transition(new Label(new SingleLabel('S', 1)), 'c', null));
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
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("δ({S_null},c) -> {P_null}",
                new Transition(new Label(new SingleLabel('S')), 'c', new Label(new SingleLabel('P'))).ToString());
        }
    }
}
