using FLaGLib.Helpers;
using NUnit.Framework;

namespace FLaGLib.Test.Helpers
{
    [TestFixture]
    public class EnumerateHelperTest
    {
        [Test]
        public void SequenceTest()
        {
            int[] expected = new int[] { 3, 6, 2 };

            int actualCount = 0;

            foreach (int v in EnumerateHelper.Sequence(3,6,2))
            {
                Assert.IsTrue(actualCount < expected.Length);
                Assert.AreEqual(expected[actualCount],v);
                actualCount++;
            }

            Assert.AreEqual(expected.Length, actualCount);
        }
    }
}
