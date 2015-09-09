using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetOfEquivalenceTransitionsPostReportTest
    {
        [Test]
        public void CctorTest_Ok()
        {
            SetOfEquivalence setOfEquavalence1 =
               new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('P')),
                        null,
                        new Label(new SingleLabel('D'))
                    )                    
                );

            SetOfEquivalence setOfEquavalence2 =
              new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('M'))
                    )                   
               );

            IEnumerable<char> symbols = EnumerateHelper.Sequence('b', 'c');

            IEnumerable<SetOfEquivalenceTransition> expectedTransitions = EnumerateHelper.Sequence(
                new SetOfEquivalenceTransition(setOfEquavalence1, symbols, setOfEquavalence2, 6),
                new SetOfEquivalenceTransition(setOfEquavalence2, symbols, setOfEquavalence1, 3)
            );

            int expectedIteration = 9;

            SetOfEquivalenceTransitionsPostReport actualPostReport = new SetOfEquivalenceTransitionsPostReport(expectedTransitions,expectedIteration);

            Assert.AreEqual(expectedIteration, actualPostReport.Iteration);
            Assert.AreEqual(expectedTransitions.Count(), actualPostReport.SetOfEquivalenceTransitions.Count);

            using (IEnumerator<SetOfEquivalenceTransition> expectedTransitionsEnumerator = expectedTransitions.GetEnumerator())
            {
                using (IEnumerator<SetOfEquivalenceTransition> actualTransitionsEnumerator = actualPostReport.SetOfEquivalenceTransitions.GetEnumerator())
                {
                    while (expectedTransitionsEnumerator.MoveNext() && actualTransitionsEnumerator.MoveNext())
                    {
                        Assert.AreEqual(expectedTransitionsEnumerator.Current.CurrentSetOfEquivalence, actualTransitionsEnumerator.Current.CurrentSetOfEquivalence);
                        Assert.AreEqual(expectedTransitionsEnumerator.Current.NextSetOfEquivalence, actualTransitionsEnumerator.Current.NextSetOfEquivalence);
                        CollectionAssert.AreEqual(expectedTransitionsEnumerator.Current.Symbols, actualTransitionsEnumerator.Current.Symbols);
                        Assert.AreEqual(expectedTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence, actualTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence);
                    }
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetOfEquivalenceTransitionsNull_Fail()
        {
            new SetOfEquivalenceTransitionsPostReport(null, 4);
        }
    }
}
