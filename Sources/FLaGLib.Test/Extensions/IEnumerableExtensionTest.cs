using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLaGLib.Extensions;

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

            string[] sequence = new string[] { "4j9dneh3", "d9end", "djeheb,gf" };

            int expectedHash = -1864936153;

            Assert.AreEqual(expectedHash, sequence.GetSequenceHashCode());
        }
    }
}
