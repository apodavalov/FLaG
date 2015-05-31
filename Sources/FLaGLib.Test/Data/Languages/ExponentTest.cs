using FLaGLib.Data.Languages;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class ExponentTest
    {
        private Tuple<Exponent, Exponent, int>[] _Expectations = new Tuple<Exponent, Exponent, int>[]
        {
            new Tuple<Exponent, Exponent, int>(
                new Variable('k', Sign.MoreThanOrEqual, 0),
                null,
                1
            ),

            new Tuple<Exponent, Exponent, int>(
                null,
                new Quantity(1),
                -1
            ),

            new Tuple<Exponent, Exponent, int>(
                new Variable('k', Sign.MoreThanOrEqual, 1),
                new Quantity(1),
                1
            ),

            new Tuple<Exponent, Exponent, int>(
                new Quantity(1),
                new Variable('k', Sign.MoreThanOrEqual, 1),
                -1
            ),

            new Tuple<Exponent, Exponent, int>(
                null,
                null,
                0
            ),
        };

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
    }
}
