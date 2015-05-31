using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class VariableTest
    {
        private Tuple<Variable, Variable, int>[] _Expectations = new Tuple<Variable, Variable, int>[]
        {
            new Tuple<Variable, Variable, int>(
                null,
                null,
                0
            ),

            new Tuple<Variable, Variable, int>(
                new Variable('k', Sign.MoreThanOrEqual, 0),
                null,
                1
            ),

            new Tuple<Variable, Variable, int>(
                null,
                new Variable('k', Sign.MoreThanOrEqual, 0),
                -1
            ),

            new Tuple<Variable, Variable, int>(
                new Variable('k', Sign.MoreThanOrEqual, 0),
                new Variable('k', Sign.MoreThanOrEqual, 0),
                0
            ),

            new Tuple<Variable, Variable, int>(
                new Variable('a', Sign.MoreThanOrEqual, 0),
                new Variable('b', Sign.MoreThanOrEqual, 0),
                -1
            ),

            new Tuple<Variable, Variable, int>(
                new Variable('b', Sign.MoreThanOrEqual, 0),
                new Variable('a', Sign.MoreThanOrEqual, 0),
                1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            for (int i = 32; i <= 255; i++)
            {
                if ((char)i > 'a' & (char)i < 'z')
                {
                    Variable variable = new Variable((char)i, Sign.MoreThanOrEqual, 0);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SetVariableLessLatinLetter_Fail()
        {
            for (int i = 32; i <= 255; i++)
            {
                if ((char)i < 'a' || (char)i > 'z')
                {
                    Variable variable = new Variable((char)i, Sign.MoreThanOrEqual, 0);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_SetNameLessZero_Fail()
        {
            Variable variable = new Variable('k', Sign.MoreThanOrEqual, -1);
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

            Variable variable = new Variable('k', Sign.MoreThanOrEqual, 0);
            variable.ToRegExp(entity);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegExp_SetNull_Fail()
        {
            Variable variable = new Variable('k', Sign.MoreThanOrEqual, 0);
            variable.ToRegExp(null);
        }

        [Test]
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Variable variable = new Variable('k', Sign.MoreThanOrEqual, 0);
            string s = variable.ToString();
            Assert.AreEqual("k",
                variable.ToString());
        }
    }
}
