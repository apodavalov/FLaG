using FLaGLib.Data.RegExps;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Test.Data.RegExps
{
    [TestFixture]
    public class TreeCollectionTest
    {
        private Tuple<TreeCollection, TreeCollection, int>[] _Expectations = new Tuple<TreeCollection, TreeCollection, int>[]
        {
            new Tuple<TreeCollection, TreeCollection, int>(
                null,
                null,
                0
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                null,
                1
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                null,
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                -1
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                0
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('c'), null),
                        new Tree(new Symbol('d'), null)
                    ),
                    TreeOperator.Concat
                    ),
                -2
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('c'), null),
                        new Tree(new Symbol('d'), null)
                    ),
                    TreeOperator.Concat
                    ),
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                2
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Concat
                    ),
                new TreeCollection(
                    EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    ),
                    TreeOperator.Union
                    ),
                -1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Tree> subtrees = EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null),
                        new Tree(new Symbol('b'), null)
                    );
            TreeCollection treeCollection = new TreeCollection(subtrees, TreeOperator.Concat);
            treeCollection = new TreeCollection(subtrees, TreeOperator.Union);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SubtreesCountLessTwo_Fail()
        {
            IEnumerable<Tree> subtrees = EnumerateHelper.Sequence<Tree>(
                        new Tree(new Symbol('a'), null)
                    );
            TreeCollection treeCollection = new TreeCollection(subtrees, TreeOperator.Concat);
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
        public void GetEnumeratorTest()
        {
            TreeCollection actualTreeCollection = new TreeCollection(
                        EnumerateHelper.Sequence<Tree>(
                            new Tree(new Symbol('a'), null),
                            new Tree(new Symbol('b'), null)
                        ),
                        TreeOperator.Concat
                    );

            IEnumerable<Tree> expectedTreeEnumerable =
                        EnumerateHelper.Sequence<Tree>(
                            new Tree(new Symbol('a'), null),
                            new Tree(new Symbol('b'), null)
                        );

            CollectionAssert.AreEqual(expectedTreeEnumerable, actualTreeCollection);

            Assert.IsTrue(expectedTreeEnumerable.SequenceEqual(actualTreeCollection));
        }
    }
}
