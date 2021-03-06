﻿using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class RemovingUnreachableStatesPostReportTest
    {
        [Test]
        public void Cctor_CurrentReachableStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RemovingUnreachableStatesIterationPostReport(
                    null, 
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(),
                    0,
                    false
                )
            );
        }

        [Test]
        public void Cctor_CurrentApproachedStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RemovingUnreachableStatesIterationPostReport(
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(), 
                    null,
                    Enumerable.Empty<Label>(),
                    0,
                    true
                )
            );
        }

        [Test]
        public void Cctor_NextReachableStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RemovingUnreachableStatesIterationPostReport(
                    Enumerable.Empty<Label>(), 
                    null,
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(), 
                    0,
                    false
                )
            );
        }

        [Test]
        public void Cctor_NextApproachedStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RemovingUnreachableStatesIterationPostReport(
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(),
                    null,
                    0,
                    true
                )
            );
        }

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Label> expectedCurrentReachableStates = EnumerateHelper.Sequence(new Label(new SingleLabel('b')), new Label(new SingleLabel('c')));
            IEnumerable<Label> expectedNextReachableStates = EnumerateHelper.Sequence(new Label(new SingleLabel('d')), new Label(new SingleLabel('e')));
            IEnumerable<Label> expectedCurrentApproachedStates = EnumerateHelper.Sequence(new Label(new SingleLabel('f')), new Label(new SingleLabel('g')));
            IEnumerable<Label> expectedNextApproachedStates = EnumerateHelper.Sequence(new Label(new SingleLabel('h')), new Label(new SingleLabel('i')));
            int expectedIteration = 45;
            bool expectedIsLastIteration = true;

            RemovingUnreachableStatesIterationPostReport actual =
                new RemovingUnreachableStatesIterationPostReport(
                    expectedCurrentReachableStates,
                    expectedNextReachableStates,
                    expectedCurrentApproachedStates,
                    expectedNextApproachedStates,
                    expectedIteration, 
                    expectedIsLastIteration);

            CollectionAssert.AreEquivalent(expectedCurrentReachableStates,actual.CurrentReachableStates);
            CollectionAssert.AreEquivalent(expectedNextReachableStates, actual.NextReachableStates);
            CollectionAssert.AreEquivalent(expectedCurrentApproachedStates, actual.CurrentApproachedStates);
            CollectionAssert.AreEquivalent(expectedNextApproachedStates, actual.CurrentApproachedMinusCurrentReachableStates);
            Assert.AreEqual(expectedIteration, actual.Iteration);
            Assert.AreEqual(expectedIsLastIteration, actual.IsLastIteration);
        }
    }
}
