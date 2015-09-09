using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.Data.Languages
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
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                null,
                1
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                0
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('c'),
                                                            new Symbol('d')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                -1
            ),

            new Tuple<TreeCollection, TreeCollection, int>(
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Concat
                                            ),
                new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('c'),
                                                            new Symbol('d')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            ),
                -1
            ),
        };

        [Test]
        public void CctorTest_Ok_SetTreeOperatorUnion()
        {
            TreeCollection treeCollection = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            );
        }

        [Test]
        public void CctorTest_Ok_SetTreeOperatorConcat()
        {
            TreeCollection treeColletion = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Concat
                                            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_Fil_SetSubtreesLessTwo()
        {
            TreeCollection treeColletion = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Concat
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
        public void GetEnumeratorTest()
        {
            TreeCollection actualTreeCollection = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Concat
                                            );            

            IEnumerable<Tree> expectedTreeEnumerable = EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                );

            CollectionAssert.AreEqual(expectedTreeEnumerable, actualTreeCollection);

            Assert.IsTrue(expectedTreeEnumerable.SequenceEqual(actualTreeCollection));
        }

        [Test]
        public void ConvertOperatorTest()
        {
            TreeCollection treeCollection = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Concat
                                            );
            treeCollection.ToRegExp();

            treeCollection = new TreeCollection(
                                                EnumerateHelper.Sequence<Tree>(
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('a'),
                                                            new Symbol('b')
                                                        )
                                                    )),
                                                    new Tree(new Union(
                                                        EnumerateHelper.Sequence<Entity>(
                                                            new Symbol('b'),
                                                            new Symbol('c')
                                                        )
                                                    ))
                                                ),
                                                TreeOperator.Union
                                            );
            treeCollection.ToRegExp();
        }
    }
}
