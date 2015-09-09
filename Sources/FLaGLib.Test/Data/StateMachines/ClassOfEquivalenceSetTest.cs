using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

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
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
                        )
                    )
                ),
                -1
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
                        )
                    )                    
                ),
                null,
                1
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
                        )
                    )                    
                ),
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('c', 'd')
                        )
                    )                    
                ),
                -2
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('c', 'd')
                         )
                    )                    
                ),
                new ClassOfEquivalenceSet(
                     EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
                        )
                    )                    
                ),
                2
            ),
            new Tuple<ClassOfEquivalenceSet, ClassOfEquivalenceSet, int>(
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
                        )
                    )                    
                ),
                new ClassOfEquivalenceSet(
                    EnumerateHelper.Sequence(
                        new ClassOfEquivalence(
                            1,
                            EnumerateHelper.Sequence('a', 'b')
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
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }
    }
}
