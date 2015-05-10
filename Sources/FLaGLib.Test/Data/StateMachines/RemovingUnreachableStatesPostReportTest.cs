using FLaGLib.Data;
using FLaGLib.Extensions;
using FLaGLib.Collections;
using FLaGLib.Data.StateMachines;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Helpers;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class RemovingUnreachableStatesPostReportTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentReachableStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                null, 
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(),
                0
            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(), 
                null,
                Enumerable.Empty<Label>(),
                0
             );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextReachableStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                Enumerable.Empty<Label>(), 
                null,
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(), 
                0
            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(),
                Enumerable.Empty<Label>(), 
                null, 
                0
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

            RemovingUnreachableStatesPostReport actual =
                new RemovingUnreachableStatesPostReport(
                    expectedCurrentReachableStates,
                    expectedNextReachableStates,
                    expectedCurrentApproachedStates,
                    expectedNextApproachedStates,expectedIteration);

            CollectionAssert.AreEquivalent(expectedCurrentReachableStates,actual.CurrentReachableStates);
            CollectionAssert.AreEquivalent(expectedNextReachableStates, actual.NextReachableStates);
            CollectionAssert.AreEquivalent(expectedCurrentApproachedStates, actual.CurrentApproachedStates);
            CollectionAssert.AreEquivalent(expectedNextApproachedStates, actual.NextApproachedStates);
            Assert.AreEqual(expectedIteration, actual.Iteration);
        }
    }
}
