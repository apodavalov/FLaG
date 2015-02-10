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
                null,
                null,
                0
            ),

            new Tuple<Degree, Degree, int>(
                new Degree(),
                null,
                1
            ),

            new Tuple<Degree, Degree, int>(
                null,
                new Degree(),
                -1
            ),

            new Tuple<Degree, Degree, int>(
                new Degree(),
                new Degree(),
                0
            )
        };

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }
    }
}
