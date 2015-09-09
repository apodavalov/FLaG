using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using NUnit.Framework;

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

        [Test]
        public void IsTest()
        {
            int expectedInt = 1;
            Assert.AreEqual(ObjectExtensions.Is<int>(expectedInt), true);
            Assert.AreEqual(ObjectExtensions.Is<string>(expectedInt), false);
        }

        [Test]
        public void AsTest()
        {
            Symbol symbol = new Symbol('a');
            Expression expression = symbol;
            Assert.AreEqual(null, symbol.As<string>());
            Assert.AreEqual(expression, symbol.As<Expression>());
        }

        [Test]
        public void OfTest()
        {
            Symbol symbol = new Symbol('a');
            Expression expression = symbol;
            Assert.AreEqual(expression, symbol.Of<Expression>());
        }
    }
}
