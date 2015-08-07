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
    public class BinaryConcatTest
    {
        private Tuple<BinaryConcat, BinaryConcat, int>[] _Expectations = new Tuple<BinaryConcat, BinaryConcat, int>[]
        {
            new Tuple<BinaryConcat, BinaryConcat, int>(
                null,
                null,
                0
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                null,
                1
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                null,
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                -1
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                0
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('c'), new Symbol('d')),
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                2
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                new BinaryConcat(new Symbol('c'), new Symbol('d')),
                -2
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('c'), new Symbol('d')),
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                2
            ),

            new Tuple<BinaryConcat, BinaryConcat, int>(
                new BinaryConcat(new Symbol('a'), new Symbol('b')),
                new BinaryConcat(new Symbol('c'), new Symbol('d')),
                -2
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            BinaryConcat binaryConcat = new BinaryConcat(new Symbol('a'), new Symbol('b'));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_LeftExpressionIsNull_Fail()
        {
            BinaryConcat binaryConcat = new BinaryConcat(null, new Symbol('b'));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_RightExpressionIsNull_Fail()
        {
            BinaryConcat binaryConcat = new BinaryConcat(new Symbol('a'), null);
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
            BinaryConcat binaryConcat = new BinaryConcat(new Symbol('a'), new Symbol('b'));
            Assert.AreEqual("ab", binaryConcat.ToString());
        }

        [Test]
        public void ToRegularSetTest()
        {
            BinaryConcat expectedBinaryConcat = new BinaryConcat(new Symbol('a'), new Symbol('b'));
            
            Assert.AreEqual(expectedBinaryConcat.ToRegularSet(), expectedBinaryConcat);

            expectedBinaryConcat = new BinaryConcat(new ConstIteration(new Symbol('a'), 0), new ConstIteration(new Symbol('b'), 1));

            BinaryConcat binaryConcat = new BinaryConcat(new ConstIteration(new Symbol('a'), 0).ToRegularSet(), new ConstIteration(new Symbol('b'), 1).ToRegularSet());

            Assert.AreEqual(expectedBinaryConcat.ToRegularSet(), binaryConcat);
        }
    }
}
