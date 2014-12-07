using FLaGLib.Data.StateMachines;
using NUnit.Framework;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetsOfEquivalenceResultTest
    {
        [Test]
        public void CctorTest()
        {
            SetsOfEquivalenceResult result = new SetsOfEquivalenceResult(true, 56);

            Assert.IsTrue(result.IsStatesCombined);
            Assert.AreEqual(56,result.LastIteration);
        }
    }
}
