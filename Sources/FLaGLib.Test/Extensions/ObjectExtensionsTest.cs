using NUnit.Framework;
using FLaGLib.Extensions;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        [Test]
        public void GetHashCodeNullableTest()
        {
            Assert.AreEqual(0, ((object)null).GetHashCodeNullable());
            Assert.AreEqual("sdsdsf".GetHashCode(), "sdsdsf".GetHashCodeNullable());
        }
    }
}
