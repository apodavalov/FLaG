using FLaGLib.Data.Languages;
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
    public class DegreeTest
    {
        private Tuple<Degree, Degree, int>[] _Expectations = new Tuple<Degree, Degree, int>[]
        {
            new Tuple<Degree, Degree, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                null,
                1
            ),

            new Tuple<Degree, Degree, int>(
                null,
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                -1
            ),

            new Tuple<Degree, Degree, int>(
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

            new Tuple<Degree, Degree, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                new Degree(
                        new Symbol('b'),
                        new Quantity(1)
                    ),
                -1
            ),

            new Tuple<Degree, Degree, int>(
                new Degree(
                        new Symbol('b'),
                        new Quantity(1)
                    ),
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                1
            ),

            new Tuple<Degree, Degree, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                new Degree(
                        new Symbol('a'),
                        new Quantity(2)
                    ),
                -1
            ),

            new Tuple<Degree, Degree, int>(
                new Degree(
                        new Symbol('a'),
                        new Quantity(2)
                    ),
                new Degree(
                        new Symbol('a'),
                        new Quantity(1)
                    ),
                1
            ),
            
            new Tuple<Degree, Degree, int>(
                null,
                null,
                0
            ),
        };        

        [Test]
        public void CctorTest_Ok()
        {
            Degree degree = new Degree(
                                new Symbol('a'),
                                new Quantity(1)
                            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_EntitiesNull_Fail()
        {
            Degree degree = new Degree(
                                null,
                                new Quantity(1)
                            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_ExponentNull_Fail()
        {
            Degree degree = new Degree(
                                new Symbol('a'),
                                null
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
        public void ToStringTest()
        {
            Degree degree = new Degree(
                                new Symbol('a'),
                                new Quantity(1)
                            );

            Assert.AreEqual("a^(1)",
                degree.ToString());
        }        
    }
}
