using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                -1
                ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'b', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                -1
                ),
            
            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('a')),
                            new Label(new SingleLabel('b'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('c')),
                            new Label(new SingleLabel('d')),
                            new Label(new SingleLabel('e'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                 -2
                ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('a')),
                            new Label(new SingleLabel('b'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('c')),
                            new Label(new SingleLabel('d')),
                            new Label(new SingleLabel('e'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly()),
                 -2
                ),

            new Tuple<MetaTransition, MetaTransition, int>(
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('a')),
                            new Label(new SingleLabel('b'))
                        )
                        ).AsReadOnly()),
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                        ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('c')),
                            new Label(new SingleLabel('d')),
                            new Label(new SingleLabel('e'))
                        )
                        ).AsReadOnly()),
                -2
                )
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_MetaCurrentOptionalStatesIsNull_Fail()
        {
            new MetaTransition(
                new SortedSet<Label>().AsReadOnly(), 
                null, 
                'a', 
                new SortedSet<Label>().AsReadOnly());          
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_MetaCurrentRequiredStatesIsNull_Fail()
        {
            new MetaTransition(
                null,
                new SortedSet<Label>().AsReadOnly(),
                'a',
                new SortedSet<Label>().AsReadOnly());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_MetaNextStatesIsNull_Fail()
        {
            new MetaTransition(
                new SortedSet<Label>().AsReadOnly(),
                new SortedSet<Label>().AsReadOnly(),
                'a',
                null);
        }

        [Test]
        public void CctorTest_Ok()
        {
            SortedSet<Label> expectedMetaCurrentRequiredStates = 
                new SortedSet<Label>(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('a')),
                        new Label(new SingleLabel('b')),
                        new Label(new SingleLabel('c'))
                    )
                );

            SortedSet<Label> expectedMetaCurrentOptionalStates =
                new SortedSet<Label>(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('d')),
                        new Label(new SingleLabel('e')),
                        new Label(new SingleLabel('f')),
                        new Label(new SingleLabel('j'))
                    )
                );

            SortedSet<Label> expectedMetaNextStates =
                new SortedSet<Label>(
                    EnumerateHelper.Sequence(
                        new Label(new SingleLabel('f')),
                        new Label(new SingleLabel('j'))
                    )
                );

            char expectedSymbol = 's';

            MetaTransition metaTransition = new MetaTransition(
                expectedMetaCurrentRequiredStates.AsReadOnly(),
                expectedMetaCurrentOptionalStates.AsReadOnly(),
                expectedSymbol,
                expectedMetaNextStates.AsReadOnly());

            Assert.AreEqual(expectedSymbol, metaTransition.Symbol);
            CollectionAssert.AreEqual(expectedMetaCurrentRequiredStates, metaTransition.CurrentRequiredStates);
            CollectionAssert.AreEqual(expectedMetaCurrentOptionalStates, metaTransition.CurrentOptionalStates);
            CollectionAssert.AreEqual(expectedMetaNextStates, metaTransition.NextStates);
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
            MetaTransition metaTransition = 
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                    ).AsReadOnly(), 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                    ).AsReadOnly(), 
                    'a', 
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                   ).AsReadOnly());

            Assert.AreEqual(7733366,metaTransition.GetHashCode());
        }

        [Test]
        public void ToStringTest()
        {
            MetaTransition metaTransition =
                new MetaTransition(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                    ).AsReadOnly(),
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                    ).AsReadOnly(),
                    'a',
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            new Label(new SingleLabel('e')),
                            new Label(new SingleLabel('r'))
                        )
                   ).AsReadOnly());

            Assert.AreEqual("δ([{e_null_null_null} {r_null_null_null} q_1 ... q_2], a) -> [ {e_null_null_null} {r_null_null_null}], q_1 ... q_2 : {e_null_null_null} {r_null_null_null}", metaTransition.ToString());
        }
    }
}
