using FLaGLib.Collections;
using FLaGLib.Extensions;
using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('P')),
                            null,
                            new Label(new SingleLabel('D'))
                        )
                    )
                );

            SetOfEquivalence setOfEquavalence2 =
              new SetOfEquivalence(
                   new SortedSet<Label>(
                       EnumerateHelper.Sequence(
                           new Label(new SingleLabel('M'))
                       )
                   )
               );

            IReadOnlySet<char> symbols = new SortedSet<char>(EnumerateHelper.Sequence('b','c')).AsReadOnly();;

            IReadOnlyList<SetOfEquivalenceTransition> expectedTransitions = new List<SetOfEquivalenceTransition>(EnumerateHelper.Sequence(
                new SetOfEquivalenceTransition(setOfEquavalence1, symbols, setOfEquavalence2, 6),
                new SetOfEquivalenceTransition(setOfEquavalence2, symbols, setOfEquavalence1, 3)
            )).AsReadOnly();

            int expectedIteration = 9;

            SetOfEquivalenceTransitionsPostReport actualPostReport = new SetOfEquivalenceTransitionsPostReport(expectedTransitions,expectedIteration);

            Assert.AreEqual(expectedIteration, actualPostReport.Iteration);
            Assert.AreEqual(expectedTransitions.Count, actualPostReport.SetOfEquivalenceTransitions.Count);

            IEnumerator<SetOfEquivalenceTransition> expectedTransitionsEnumerator = expectedTransitions.GetEnumerator();
            IEnumerator<SetOfEquivalenceTransition> actualTransitionsEnumerator = actualPostReport.SetOfEquivalenceTransitions.GetEnumerator();

            while (expectedTransitionsEnumerator.MoveNext() && actualTransitionsEnumerator.MoveNext())
            {
                Assert.AreEqual(expectedTransitionsEnumerator.Current.CurrentSetOfEquivalence, actualTransitionsEnumerator.Current.CurrentSetOfEquivalence);
                Assert.AreEqual(expectedTransitionsEnumerator.Current.NextSetOfEquivalence, actualTransitionsEnumerator.Current.NextSetOfEquivalence);
                CollectionAssert.AreEqual(expectedTransitionsEnumerator.Current.Symbols, actualTransitionsEnumerator.Current.Symbols);
                Assert.AreEqual(expectedTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence, actualTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence);
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
