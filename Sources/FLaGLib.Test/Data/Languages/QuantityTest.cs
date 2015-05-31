using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class QuantityTest
    {
        private Tuple<Quantity, Quantity, int>[] _Expectations = new Tuple<Quantity, Quantity, int>[]
        {
            new Tuple<Quantity, Quantity, int>(
                new Quantity(1),
                null,
                1
            ),

            new Tuple<Quantity, Quantity, int>(
                null,
                new Quantity(1),
                -1
            ),

            new Tuple<Quantity, Quantity, int>(
                new Quantity(1),
                new Quantity(1),
                0
            ),

            new Tuple<Quantity, Quantity, int>(
                null,
                null,
                0
            ),

            new Tuple<Quantity, Quantity, int>(
                new Quantity(1),
                new Quantity(2),
                -1
            ),

            new Tuple<Quantity, Quantity, int>(
                new Quantity(2),
                new Quantity(1),
                1
            ),

            new Tuple<Quantity, Quantity, int>(
                new Quantity(2),
                new Quantity(1),
                1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            Quantity quantity = new Quantity(1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SetVariableLessZero_Fail()
        {
            Quantity quantity = new Quantity(-1);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegExp_SetNull_Fail()
        {
            Quantity quantity = new Quantity(1);
            quantity.ToRegExp(null);
        }

        [Test]
        public void ToRegExp_Ok()
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

            Quantity quantity = new Quantity(1);
            
            quantity.ToRegExp(entity);
        }
    }
}
