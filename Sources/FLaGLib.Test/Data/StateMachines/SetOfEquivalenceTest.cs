using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetOfEquivalenceTest
    {
        private Tuple<SetOfEquivalence, SetOfEquivalence, int>[] _Expectations = new Tuple<SetOfEquivalence, SetOfEquivalence, int>[]
        {
            new Tuple<SetOfEquivalence, SetOfEquivalence, int>(null,null,0),
            new Tuple<SetOfEquivalence, SetOfEquivalence, int>(
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('A')),
                        new Label(new SingleLabel('B'))
                    )
                ),null,1),
            new Tuple<SetOfEquivalence, SetOfEquivalence, int>(
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('A')),
                        new Label(new SingleLabel('B'))
                    )
                ),
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('A')),
                        new Label(new SingleLabel('B'))
                    )
                ),0),
            new Tuple<SetOfEquivalence, SetOfEquivalence, int>(
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('C')),
                        new Label(new SingleLabel('B'))
                    )
                ),
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('A')),
                        new Label(new SingleLabel('B'))
                    )
                ),1),
            new Tuple<SetOfEquivalence, SetOfEquivalence, int>(
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('C')),
                        null,
                        new Label(new SingleLabel('B'))
                    )
                ),
                new SetOfEquivalence(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('A')),
                        new Label(new SingleLabel('B'))
                    )
                ),-1)
        };

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Label> expected =
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('A')),
                    new Label(new SingleLabel('C'))
                );
            

            SetOfEquivalence actual = new SetOfEquivalence(expected);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetNull_Fail()
        {
            new SetOfEquivalence(null);
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
    }
}
