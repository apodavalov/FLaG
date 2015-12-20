using FLaGLib.Data.Languages;
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
    }
}
