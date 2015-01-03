using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using FLaGLib.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class ClassOfEquivalenceSetTest
    {
        private Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>[] _Expectations = new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>[]
        {
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                null,
                null,
                0
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                null,
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                -1
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                null,
                1
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('c', 'd')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                -2
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('c', 'd')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                2
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                ),
                0
            )
        };

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
        public void GetHashCode()
        {
            ClassOfEquivalenceSet classOfEquivalenceSet = new ClassOfEquivalenceSet(
                    new SortedSet<ClassOfEquivalence>(
                        EnumerateHelper.Sequence(
                            new ClassOfEquivalence(
                                1,
                                new SortedSet<char>(
                                    EnumerateHelper.Sequence('a', 'b')
                                ).AsReadOnly()
                            )
                        )
                    )
                );

            Assert.AreEqual(196610, classOfEquivalenceSet.GetHashCode());
        }
    }
}
