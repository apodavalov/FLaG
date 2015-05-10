using NUnit.Framework;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System.Collections.Generic;

namespace FLaGLib.Test.Extensions
{
    [TestFixture]
    public class IEnumerableExtensionTest
    {
        [Test]
        public void SequenceCompareTest()
        {
            string[] sequence1 = null;
            string[] sequence2 = new string[] { "3dd99jf", "939wefwf3", "2323fsd" };

            Assert.AreEqual(-1, sequence1.SequenceCompare(sequence2));
            Assert.AreEqual(1, sequence2.SequenceCompare(sequence1));

            sequence1 = null;
            sequence2 = null;

            Assert.AreEqual(0, sequence2.SequenceCompare(sequence1));

            sequence1 = new string[] { "3dd99jf", "939wefwf3", "2323fsd" };
            sequence2 = new string[] { "3dd99jf", "939wefwf3" };

            Assert.AreEqual(1, sequence1.SequenceCompare(sequence2));
            Assert.AreEqual(-1, sequence2.SequenceCompare(sequence1));

            sequence1 = new string[] { "3dd99jf", "fr4ff34fd", "7yg78" };
            sequence2 = new string[] { "3dd99jf", "939wefwf3", "7yg78" };

            Assert.AreEqual(1, sequence1.SequenceCompare(sequence2));
            Assert.AreEqual(-1, sequence2.SequenceCompare(sequence1));

            sequence1 = new string[] { "3dd99jf", "fr4ff34fd", "7yg78" };
            sequence2 = new string[] { "3dd99jf", "fr4ff34fd", "7yg78" };

            Assert.AreEqual(0, sequence1.SequenceCompare(sequence2));
        }

        [Test]
        public void GetSequenceHashCodeTest()
        {
            Assert.AreEqual(0, ((string[])null).GetSequenceHashCode());
            Assert.AreEqual(0, new string[0].GetSequenceHashCode());

            IEnumerable<string> sequence1 = EnumerateHelper.Sequence("4j9dneh3", "d9end", "djeheb,gf");
            IEnumerable<string> sequence2 = EnumerateHelper.Sequence("4j9dneh3", "d9end", "djeheb,gf");
            IEnumerable<string> sequence3 = EnumerateHelper.Sequence( "erg34g", "3ghg6", "7g33g");
            IEnumerable<string> sequence4 = EnumerateHelper.Sequence("erg34g", "3ghg6", "7g33g");

            int hash1 = sequence1.GetSequenceHashCode();
            int hash2 = sequence2.GetSequenceHashCode();
            int hash3 = sequence3.GetSequenceHashCode();
            int hash4 = sequence4.GetSequenceHashCode();

            Assert.AreEqual(hash1, hash2);
            Assert.AreNotEqual(hash1, hash3);
            Assert.AreNotEqual(hash1, hash4);
            Assert.AreNotEqual(hash2, hash3);
            Assert.AreNotEqual(hash2, hash4);
            Assert.AreEqual(hash3, hash4);
        }
    }
}
