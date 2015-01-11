using NUnit.Framework;
using FLaGLib.Extensions;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class IEquatableExtensionsTest
    {
        [Test]
        public void EqualsNullableTest()
        {
            Assert.AreEqual(true, "notnullstring".EqualsNullable("notnullstring"));
            Assert.AreEqual(false, "anothernotnullstring".EqualsNullable("notnullstring"));
            Assert.AreEqual(false, "notnullstring".EqualsNullable("anothernotnullstring"));
            Assert.AreEqual(false, "notnullstring".EqualsNullable(null));
            Assert.AreEqual(false, ((string)null).EqualsNullable("notnullstring"));
            Assert.AreEqual(true, ((string)null).EqualsNullable(null));
        }
    }
}
