using NUnit.Framework;
using FLaGLib.Extensions;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class IComparableExtensionsTest
    {
        [Test]
        public void CompareToTest()
        {
            Assert.AreEqual(0, "string".CompareToNullable("string"));
            Assert.AreEqual(1, "string".CompareToNullable("notnullstring"));
            Assert.AreEqual(-1, "notnullstring".CompareToNullable("string"));
            Assert.AreEqual(1, "notnullstring".CompareToNullable(null));
            Assert.AreEqual(-1, ((string)null).CompareToNullable("notnullstring"));
            Assert.AreEqual(0, ((string)null).CompareToNullable(null));
        }
    }
}
