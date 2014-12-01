using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class RemovingUnreachableStatesPostReportTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentReachableStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(null, new Label[] { }, new Label[] { }, new Label[] { }, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_CurrentApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(new Label[] { }, new Label[] { }, null, new Label[] { }, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextReachableStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(new Label[] { }, null, new Label[] { }, new Label[] { }, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_NextApproachedStatesNull_Fail()
        {
            new RemovingUnreachableStatesPostReport(new Label[] { }, new Label[] { }, new Label[] { }, null, 0);
        }

        [Test]
        public void CctorTest_Ok()
        {
            Label[] expectedCurrentReachableStates = new Label[] { new Label(new SingleLabel('b')), new Label(new SingleLabel('c')) };
            Label[] expectedNextReachableStates = new Label[] { new Label(new SingleLabel('d')), new Label(new SingleLabel('e')) };
            Label[] expectedCurrentApproachedStates = new Label[] { new Label(new SingleLabel('f')), new Label(new SingleLabel('g')) };
            Label[] expectedNextApproachedStates = new Label[] { new Label(new SingleLabel('h')), new Label(new SingleLabel('i')) };
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
