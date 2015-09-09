using FLaGLib.Data.RegExps;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class TreeTest
    {
        private Tuple<Tree, Tree, int>[] _Expectations = new Tuple<Tree, Tree, int>[]
        {
            new Tuple<Tree, Tree, int>(
                null,
                null,
                0
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Symbol('a'), null),
                null,
                1
            ),
            
            new Tuple<Tree, Tree, int>(
                null,
                new Tree(new Symbol('a'), null),
                -1
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Symbol('a'), null),
                new Tree(new Symbol('a'), null),
                0
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Symbol('b'), null),
                new Tree(new Symbol('a'), null),
                1
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Symbol('a'), null),
                new Tree(new Symbol('b'), null),
                -1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            Tree tree = new Tree(new Symbol('a'), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetNull_Fail()
        {
            Tree tree = new Tree(null, null);
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
    }
}
