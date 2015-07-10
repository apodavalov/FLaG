using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class EntityTest
    {
        private Tuple<Entity, Entity, int>[] _Expectations = new Tuple<Entity, Entity, int>[]
        {
            new Tuple<Entity, Entity, int>(
                null,
                null,
                0
            ),

            new Tuple<Entity, Entity, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                null,
                1
            ),

            new Tuple<Entity, Entity, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                0
            ),

            new Tuple<Entity, Entity, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                new Degree(
                        new Symbol('b'),
                        new Quantity(2)
                    ),
                -1
            ),

            new Tuple<Entity, Entity, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                0
            ),
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
        public void SplitTest()
        {
            Variable kVariable = new Variable('k', Sign.MoreThanOrEqual, 0);
            Variable mVariable = new Variable('m', Sign.MoreThan, 0);

            Degree degree1 =
                new Degree(
                    new Degree(
                        new Union(
                            EnumerateHelper.Sequence(
                                new Symbol('a'),
                                new Symbol('b')
                            )
                        ),
                        kVariable
                    ),
                    new Quantity(2)
                );

            Degree degree2 =
                new Degree(
                    new Degree(
                        new Concat(
                            EnumerateHelper.Sequence(
                                new Symbol('b'),
                                new Symbol('a')
                            )
                        ),
                        new Quantity(2)
                    ),
                    mVariable
                );

            Concat concat1 =
                new Concat(
                    EnumerateHelper.Sequence<Entity>(
                        new Degree(
                            new Symbol('c'),
                            kVariable
                        ),
                        new Symbol('d')
                    )
                );

            Union union1 =
                new Union(
                    EnumerateHelper.Sequence<Entity>(
                        new Degree(
                            new Symbol('e'),
                            mVariable
                        ),
                        new Symbol('f')
                    )
                );

            Degree degree3 = 
                new Degree(
                    new Symbol('m'),
                    mVariable
                );

            Degree degree4 =
                new Degree(
                    new Symbol('z'),
                    kVariable
                );

            Concat concat2 = new Concat(
                    EnumerateHelper.Sequence<Entity>(
                        degree3,
                        degree4
                    )
                );

            Symbol symbol1 = new Symbol('y');

            Entity entity = 
                new Union(
                    EnumerateHelper.Sequence<Entity>(
                        degree1,
                        degree2,
                        concat1,
                        union1,
                        concat2,
                        symbol1
                    )
                );

            Tree actualTree = entity.Split();

            Tree expectedTree = new Tree(entity,
                new TreeCollection(
                    EnumerateHelper.Sequence(
                        new Tree(degree1),
                        new Tree(degree2),
                        new Tree(concat1),
                        new Tree(union1),
                        new Tree(concat2, 
                            new TreeCollection(
                                EnumerateHelper.Sequence(
                                    new Tree(degree3),
                                    new Tree(degree4)
                                ),
                                TreeOperator.Concat
                            )
                        ),
                        new Tree(symbol1)
                    ),
                    TreeOperator.Union
                )
            );

            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
