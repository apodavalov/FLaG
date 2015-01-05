using NUnit.Framework;
using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class MetaFinalStateTest
    {
        [Test]
        public void CctorTest_Ok()
        {
            IReadOnlySet<Label> expectedRequiredStates = 
                new SortedSet<Label>(
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
                    )
                ).AsReadOnly();

            IReadOnlySet<Label> expectedOptionalStates = 
                new SortedSet<Label>(
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
                    )
                ).AsReadOnly();

            MetaFinalState metaFinalState =
                new MetaFinalState(
                    expectedRequiredStates, 
                    expectedOptionalStates
                );

            CollectionAssert.AreEqual(expectedRequiredStates, metaFinalState.RequiredStates);
            CollectionAssert.AreEqual(expectedOptionalStates, metaFinalState.OptionalStates);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_RequiredStatesNull_Fail()
        {
            IReadOnlySet<Label> expectedOptionalStates =
                new SortedSet<Label>(
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
                    )
                ).AsReadOnly();

            new MetaFinalState(null, expectedOptionalStates);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_OptionalStatesNull_Fail()
        {
            IReadOnlySet<Label> expectedRequiredStates = 
                new SortedSet<Label>(
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
                    )
                ).AsReadOnly();

            new MetaFinalState(expectedRequiredStates, null);
        }
    }
}
