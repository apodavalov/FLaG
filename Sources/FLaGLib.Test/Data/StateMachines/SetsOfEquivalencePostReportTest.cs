using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetsOfEquivalencePostReportTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetsOfEquivalenceNull_Fail()
        {
            new SetsOfEquivalencePostReport(null, 56);
        }

        [Test]
        public void CctorTest_Ok()
        {
            SetsOfEquivalence expectedSetsOfEquivalence = new SetsOfEquivalence(
                    new SortedSet<SetOfEquivalence>
                    (
                        EnumerateHelper.Sequence(
                            new SetOfEquivalence(
                                new SortedSet<Label>(
                                    EnumerateHelper.Sequence(
                                        new Label(new SingleLabel('P')),
                                        null,
                                        new Label(new SingleLabel('D'))
                                    )
                                )
                            ),
                            new SetOfEquivalence(
                                new SortedSet<Label>(
                                    EnumerateHelper.Sequence(
                                        new Label(new SingleLabel('S')),
                                        null,
                                        new Label(new SingleLabel('M'))
                                    )
                                )
                            )
                        )
                    )
                );

            int expectedIteration = 65;
            SetsOfEquivalencePostReport actual = new SetsOfEquivalencePostReport(expectedSetsOfEquivalence, expectedIteration);

            Assert.AreEqual(expectedSetsOfEquivalence, actual.SetsOfEquivalence);
            Assert.AreEqual(expectedIteration, actual.Iteration);
        }
    }
}
