using NUnit.Framework;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System.Collections.Generic;
using System;

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

        [Test]
        public void ToHashSetTest_Ok()
        {
            int[] variable = new int[2];
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                variable[i] = i;
            }

            HashSet<int> expectedHashSet = new HashSet<int>(variable);
            HashSet<int> actualHashSet = variable.ToHashSet();
            Assert.AreEqual(expectedHashSet, actualHashSet);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToHashSetTest_Fail()
        {
            IEnumerable<int> iEnumerable = null;
            IEnumerableExtension.ToHashSet(iEnumerable);
        }

        [Test]
        public void ToSortedSetTest_Ok()
        {
            int[] variable = new int[2];
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                variable[i] = i;
            }

            SortedSet<int> expectedSortedSet = new SortedSet<int>(variable);
            SortedSet<int> actualSortedSet = IEnumerableExtension.ToSortedSet(variable);
            Assert.AreEqual(expectedSortedSet, actualSortedSet);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToSortedSetTest_Fail()
        {
            IEnumerable<int> iEnumerable = null;
            IEnumerableExtension.ToSortedSet(iEnumerable);
        }

        [Test]
        public void ToSortedSetNullableTest()
        {
            int[] variable = new int[2];
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                variable[i] = i;
            }

            SortedSet<int> expectedSortedSet = new SortedSet<int>(variable);
            SortedSet<int> actualSortedSet = IEnumerableExtension.ToSortedSetNullable(variable);
            Assert.AreEqual(expectedSortedSet, actualSortedSet);
            IEnumerable<int> iEnumerable = null;
            actualSortedSet = IEnumerableExtension.ToSortedSetNullable(iEnumerable);
            Assert.AreEqual(null, actualSortedSet);
        }

        [Test]
        public void ToHashSetNullableTest()
        {
            int[] variable = new int[2];
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                variable[i] = i;
            }

            HashSet<int> expectedHashSet = new HashSet<int>(variable);
            HashSet<int> actualHashSet = IEnumerableExtension.ToHashSetNullable(variable);
            Assert.AreEqual(expectedHashSet, actualHashSet);
            IEnumerable<int> iEnumerable = null;
            actualHashSet = IEnumerableExtension.ToHashSetNullable(iEnumerable);
            Assert.AreEqual(null, actualHashSet);
        }

        [Test]
        public void ToListNullableTest()
        {
            int[] variable = new int[2];
            for (int i = 0; i < variable.GetLength(0); i++)
            {
                variable[i] = i;
            }

            List<int> expectedList = new List<int>(variable);
            List<int> actualList = IEnumerableExtension.ToListNullable(variable);
            Assert.AreEqual(expectedList, actualList);
            IEnumerable<int> iEnumerable = null;
            actualList = IEnumerableExtension.ToListNullable(iEnumerable);
            Assert.AreEqual(null, actualList);
        }
    }
}
