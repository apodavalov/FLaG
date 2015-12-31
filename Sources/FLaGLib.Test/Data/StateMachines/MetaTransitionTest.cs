using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class MetaTransitionTest
    {
        private Tuple<MetaTransition, MetaTransition, int>[] _Expectations = new Tuple<MetaTransition, MetaTransition, int>[]
        {
            new Tuple<MetaTransition, MetaTransition, int>(
                null,
                null,
                0
            ),
            
            new Tuple<MetaTransition, MetaTransition, int>(
                null,
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a',
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                -1
            ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'b', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                -1
            ),
            
            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('a')),
                        new Label(new SingleLabel('b'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('c')),
                        new Label(new SingleLabel('d')),
                        new Label(new SingleLabel('e'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                -2
            ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('a')),
                        new Label(new SingleLabel('b'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),

                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('c')),
                        new Label(new SingleLabel('d')),
                        new Label(new SingleLabel('e'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                ),
                -2
            ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('a')),
                        new Label(new SingleLabel('b'))
                    )
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a', 
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('c')),
                        new Label(new SingleLabel('d')),
                        new Label(new SingleLabel('e'))
                    )
                ),
                -2
            )
        };

        [Test]
        public void CctorTest_MetaCurrentOptionalStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MetaTransition(
                    Enumerable.Empty<Label>(),
                    null,
                    'a',
                    Enumerable.Empty<Label>()
                )
            );
        }

        [Test]
        public void CctorTest_MetaCurrentRequiredStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MetaTransition(
                    null,
                    Enumerable.Empty<Label>(),
                    'a',
                    Enumerable.Empty<Label>()
                )
            );
        }

        [Test]
        public void CctorTest_MetaNextStatesNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MetaTransition(
                    Enumerable.Empty<Label>(),
                    Enumerable.Empty<Label>(),
                    'a',
                    null
                )
            );
        }

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Label> expectedMetaCurrentRequiredStates =
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('a')),
                    new Label(new SingleLabel('b')),
                    new Label(new SingleLabel('c'))
                );


            IEnumerable<Label> expectedMetaCurrentOptionalStates =
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('d')),
                    new Label(new SingleLabel('e')),
                    new Label(new SingleLabel('f')),
                    new Label(new SingleLabel('j'))
                );

            IEnumerable<Label> expectedMetaNextStates =
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('f')),
                    new Label(new SingleLabel('j'))
                );

            char expectedSymbol = 's';

            MetaTransition metaTransition = new MetaTransition(
                expectedMetaCurrentRequiredStates,
                expectedMetaCurrentOptionalStates,
                expectedSymbol,
                expectedMetaNextStates);

            Assert.AreEqual(expectedSymbol, metaTransition.Symbol);
            CollectionAssert.AreEquivalent(expectedMetaCurrentRequiredStates, metaTransition.CurrentRequiredStates);
            CollectionAssert.AreEquivalent(expectedMetaCurrentOptionalStates, metaTransition.CurrentOptionalStates);
            CollectionAssert.AreEquivalent(expectedMetaNextStates, metaTransition.NextStates);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);           
        }

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            MetaTransition metaTransition =
                new MetaTransition(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    ),
                    'a',
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('r'))
                    )
                );

            Assert.AreEqual("δ([{e_null} {r_null} q_1 ... q_2], a) -> [ {e_null} {r_null}], q_1 ... q_2 : {e_null} {r_null}", metaTransition.ToString());
        }
    }
}
