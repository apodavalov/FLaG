using NUnit.Framework;
using System;
using System.Collections.Generic;
using FLaGLib.Extensions;

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
            int[] expectedList = new int[] { 3, 4, 5, 8, 9, 10 };
            IReadOnlyList<int> actualList = new HashSet<int>(new int[] {5,8,3,4,9,10}).ConvertToReadOnlyListAndSort();

            CollectionAssert.AreEqual(expectedList, actualList);
        }
    }
}
