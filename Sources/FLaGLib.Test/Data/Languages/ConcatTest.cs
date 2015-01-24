using FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class ConcatTest
    {
        private Tuple<Concat, Concat, int>[] _Expectations = new Tuple<Concat, Concat, int>[]
        {
            new Tuple<Concat, Concat, int>(
                null,
                null,
                0
            ),

            new Tuple<Concat, Concat, int>(
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                null,
                1
            ),

            new Tuple<Concat, Concat, int>(
                null,
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                -1
            ),

            new Tuple<Concat, Concat, int>(
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                0
            ),
            
            new Tuple<Concat, Concat, int>(
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('b'))
                    ),
                -1
            ),

            new Tuple<Concat, Concat, int>(
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('b'))
                    ),
                new Concat(
                    EnumerateHelper.Sequence<Entity>(new Symbol('a'))
                    ),
                1
            )
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_Fail()
        {
            Concat concat = new Concat(null);
        }

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<Entity> sequence = EnumerateHelper.Sequence<Entity>(new Symbol('a'));

            IReadOnlyList<Entity> expectedEntityCollection = new List<Entity>(sequence).AsReadOnly();

            Concat concat = new Concat(sequence);

            Assert.AreEqual(expectedEntityCollection, concat.EntityCollection);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }
    }
}
