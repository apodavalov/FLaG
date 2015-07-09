using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class UnionTest
    {
        private Tuple<Union, Union, int>[] _Expectations = new Tuple<Union, Union, int>[]
        {
            new Tuple<Union, Union, int>(
                null,
                null,
                0
            ),

            new Tuple<Union, Union, int>(
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                null,
                1
            ),

            new Tuple<Union, Union, int>(
                null,
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                -1
            ),

            new Tuple<Union, Union, int>(
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                0
            ),

            new Tuple<Union, Union, int>(
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('c'),
                                    new Symbol('d')
                                )
                            ),
                -2
            ),

            new Tuple<Union, Union, int>(
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('c'),
                                    new Symbol('d')
                                )
                            ),
                new Union (
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            ),
                2
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            Union union = new Union(
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    new Symbol('b')
                                )
                            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SetLessTwoItems_Fail()
        {
            Union union = new Union(
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a')
                                )
                            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetNull_Fail()
        {
            Union union = new Union(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SetNullItems_Fail()
        {
            Union union = new Union(
                                EnumerateHelper.Sequence<Entity>(
                                    new Symbol('a'),
                                    null
                                )
                            );
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void EqualTest()
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
            Union union = new Union(
                                 EnumerateHelper.Sequence<Entity>(
                                     new Symbol('a'),
                                     new Symbol('b')
                                 )
                             );
            Assert.AreEqual("(a,b)",
                union.ToString());
        }
    }
}
