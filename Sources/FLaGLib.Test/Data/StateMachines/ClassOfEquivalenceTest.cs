using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                -1
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                null,
                1
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('c', 'd')
                    ),
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                2
            ),

            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                0
            ),
            
            new Tuple<ClassOfEquivalence, ClassOfEquivalence, int>(
                new ClassOfEquivalence(
                        1,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                new ClassOfEquivalence(
                        0,
                        EnumerateHelper.Sequence('a', 'b')
                    ),
                1
            )
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SymbolsNull_Fail()
        {
            new ClassOfEquivalence(2, null);
        }

        [Test]
        public void CctorTest_Ok()
        {
            int expectedSetNumber = 2;

            IEnumerable<char> expectedSymbols = EnumerateHelper.Sequence('a', 'b');           

            ClassOfEquivalence classOfEquivalence = new ClassOfEquivalence(expectedSetNumber, expectedSymbols);

            Assert.AreEqual(expectedSetNumber, classOfEquivalence.SetNum);
            CollectionAssert.AreEquivalent(expectedSymbols, classOfEquivalence.Symbols);
        }

        [Test]
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
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
