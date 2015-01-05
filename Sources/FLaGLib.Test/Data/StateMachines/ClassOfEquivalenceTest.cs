using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using FLaGLib.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FLaGLib.Collections;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class ClassOfEquivalenceTest
    {
        private Tuple<ClassOfEquivalence, ClassOfEquivalence, int>[] _Expectations = new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>[]
        {
            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                null,
                null,
                0
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                null,
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                -1
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                null,
                1
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('c', 'd')
                        ).AsReadOnly()
                    ),
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                2
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                0
            ),
            
            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                new ClassOfEquivalence(
                        0,
                        new SortedSet<char>(
                            EnumerateHelper.Sequence('a', 'b')
                        ).AsReadOnly()
                    ),
                1
            )
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SymbolsIsNull_Failed()
        {
            new ClassOfEquivalence(2, null);
        }

        [Test]
        public void CctorTest_Ok()
        {
            int expectedSetNumber = 2;

            IReadOnlySet<char> expectedSymbols = new SortedSet<char>(
                EnumerateHelper.Sequence('a', 'b')
            ).AsReadOnly();

            ClassOfEquivalence classOfEquivalence = new ClassOfEquivalence(expectedSetNumber, expectedSymbols);

            Assert.AreEqual(expectedSetNumber, classOfEquivalence.SetNum);
            CollectionAssert.AreEqual(expectedSymbols, classOfEquivalence.Symbols);
        }

        [Test]
        public void GetHashCodeTest()
        {
            ClassOfEquivalence classOfEquivalence = new ClassOfEquivalence(
                2, 
                new SortedSet<char>(
                    EnumerateHelper.Sequence('a', 'b')
                ).AsReadOnly());

            Assert.AreEqual(196609,classOfEquivalence.GetHashCode());
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
    }
}
