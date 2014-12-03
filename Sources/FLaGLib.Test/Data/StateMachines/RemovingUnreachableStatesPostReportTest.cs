using FLaGLib.Data;
using FLaGLib.Extensions;
using FLaGLib.Collections;
using FLaGLib.Data.StateMachines;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                new HashSet<Label>(new Label[] { }).AsReadOnly(),
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 
                null,
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextReachableStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 
                null,
                new HashSet<Label>(new Label[] { }).AsReadOnly(),
                new HashSet<Label>(new Label[] { }).AsReadOnly(), 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(
                new HashSet<Label>(new Label[] { }).AsReadOnly(),
                new HashSet<Label>(new Label[] { }).AsReadOnly(),
                new HashSet<Label>(new Label[] { }).AsReadOnly(), null, 0);
        }

        [Test]
        public void CctorTest_Ok()
        {
            IReadOnlySet<Label> expectedCurrentReachableStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')), new Label(new SingleLabel('c')) }).AsReadOnly();
            IReadOnlySet<Label> expectedNextReachableStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('d')), new Label(new SingleLabel('e')) }).AsReadOnly();
            IReadOnlySet<Label> expectedCurrentApproachedStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('f')), new Label(new SingleLabel('g')) }).AsReadOnly();
            IReadOnlySet<Label> expectedNextApproachedStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('h')), new Label(new SingleLabel('i')) }).AsReadOnly();
            int expectedIteration = 45;

            RemovingUnreachableStatesPostReport actual =
                new RemovingUnreachableStatesPostReport(
                    expectedCurrentReachableStates,
                    expectedNextReachableStates,
                    expectedCurrentApproachedStates,
                    expectedNextApproachedStates,expectedIteration);

            CollectionAssert.AreEqual(expectedCurrentReachableStates,actual.CurrentReachableStates);
            CollectionAssert.AreEqual(expectedNextReachableStates, actual.NextReachableStates);
            CollectionAssert.AreEqual(expectedCurrentApproachedStates, actual.CurrentApproachedStates);
            CollectionAssert.AreEqual(expectedNextApproachedStates, actual.NextApproachedStates);
            Assert.AreEqual(expectedIteration, actual.Iteration);
        }
    }
}
