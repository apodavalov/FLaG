using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class MetaFinalStateTest
    {
        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Label> expectedRequiredStates =
                EnumerateHelper.Sequence<Label>
                (
                    new Label(
                        new SingleLabel(
                            'a'
                        )
                    ),
                    new Label(
                        new SingleLabel(
                            'b'
                        )
                    )
                );

            IEnumerable<Label> expectedOptionalStates =
                EnumerateHelper.Sequence<Label>
                (
                    new Label(
                        new SingleLabel(
                            'c'
                        )
                    ),
                    new Label(
                        new SingleLabel(
                            'd'
                        )
                    )
                );

            MetaFinalState metaFinalState =
                new MetaFinalState(
                    expectedRequiredStates, 
                    expectedOptionalStates
                );

            CollectionAssert.AreEquivalent(expectedRequiredStates, metaFinalState.RequiredStates);
            CollectionAssert.AreEquivalent(expectedOptionalStates, metaFinalState.OptionalStates);
        }

        [Test]
        public void CctorTest_RequiredStatesNull_Fail()
        {
            IEnumerable<Label> expectedOptionalStates =
                EnumerateHelper.Sequence<Label>
                (
                    new Label(
                        new SingleLabel(
                            'a'
                        )
                    ),
                    new Label(
                        new SingleLabel(
                            'b'
                        )
                    )
                );

            Assert.Throws<ArgumentNullException>(() => new MetaFinalState(null, expectedOptionalStates));
        }

        [Test]
        public void CctorTest_OptionalStatesNull_Fail()
        {
            IEnumerable<Label> expectedRequiredStates =
                EnumerateHelper.Sequence<Label>
                (
                    new Label(
                        new SingleLabel(
                            'a'
                        )
                    ),
                    new Label(
                        new SingleLabel(
                            'b'
                        )
                    )
                );

            Assert.Throws<ArgumentNullException>(() => new MetaFinalState(expectedRequiredStates, null));
        }
    }
}
