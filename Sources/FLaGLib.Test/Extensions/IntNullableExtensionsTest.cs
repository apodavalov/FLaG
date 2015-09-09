using FLaGLib.Extensions;
using NUnit.Framework;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    class IntNullableExtensionsTest
    {
        [Test]
        public void CompareToTest()
        {
            Assert.AreEqual(0, new int?(5).CompareTo(5));
            Assert.AreEqual(1, new int?(5).CompareTo(4));
            Assert.AreEqual(-1, new int?(4).CompareTo(5));
            Assert.AreEqual(1, new int?(4).CompareTo(null));
            Assert.AreEqual(-1, new int?().CompareTo(5));
            Assert.AreEqual(0, new int?().CompareTo(new int?()));
        }
    }
}
