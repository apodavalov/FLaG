using FLaGLib.Extensions;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class NullableExtensionsTest
    {
        [Test]
        public void CompareToNullableTest()
        {
            Assert.AreEqual(0, new DateTime?(new DateTime(232323232335)).CompareToNullable(new DateTime(232323232335)));
            Assert.AreEqual(1, new DateTime?(new DateTime(232323232335)).CompareToNullable(new DateTime(232323232334)));
            Assert.AreEqual(-1, new DateTime?(new DateTime(232323232334)).CompareToNullable(new DateTime(232323232335)));
            Assert.AreEqual(1, new DateTime?(new DateTime(232323232334)).CompareToNullable(null));
            Assert.AreEqual(-1, ((DateTime?)null).CompareToNullable(new DateTime(232323232334)));
            Assert.AreEqual(0, ((DateTime?)null).CompareToNullable(null));
        }
    }
}
