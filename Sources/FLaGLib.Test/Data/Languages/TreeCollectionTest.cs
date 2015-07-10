using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        };

        [Test]
        public void CctorTest_Ok_SetTreeOperatorUnion()
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
    }
}
