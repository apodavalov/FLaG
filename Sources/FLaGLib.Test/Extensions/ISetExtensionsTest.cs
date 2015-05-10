using NUnit.Framework;
using System;
using System.Collections.Generic;
using FLaGLib.Extensions;
using FLaGLib.Helpers;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class ISetExtensionsTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToReadOnlyListAndSortTest_ThisNull_Fail()
        {
            ((ISet<int>)null).ConvertToReadOnlyListAndSort();
        }

        [Test]
        public void ConvertToReadOnlyListAndSortTest_Ok()
        {
            IEnumerable<int> expectedList = EnumerateHelper.Sequence(3, 4, 5, 8, 9, 10);
            IReadOnlyList<int> actualList = EnumerateHelper.Sequence(5, 8, 3, 4, 9, 10).ToHashSet().ConvertToReadOnlyListAndSort();

            CollectionAssert.AreEqual(expectedList, actualList);
        }
    }
}
