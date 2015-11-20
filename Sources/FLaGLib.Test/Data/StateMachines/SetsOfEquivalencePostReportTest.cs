using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetsOfEquivalencePostReportTest
    {
        [Test]
        public void CctorTest_SetsOfEquivalenceNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new SetsOfEquivalencePostReport(null, 56));
        }

        [Test]
        public void CctorTest_Ok()
        {
            SetsOfEquivalence expectedSetsOfEquivalence = new SetsOfEquivalence(
                EnumerateHelper.Sequence(
                new SetOfEquivalence(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('P')),
                            null,
                            new Label(new SingleLabel('D'))
                        )                                
                    ),
                    new SetOfEquivalence(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('S')),
                            null,
                            new Label(new SingleLabel('M'))
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
