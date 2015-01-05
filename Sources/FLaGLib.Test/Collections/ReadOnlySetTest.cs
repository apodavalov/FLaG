using FLaGLib.Collections;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLaGLib.Test.Collections
{
    [TestFixture]
    public class ReadOnlySetTest
    {
        private SortedSet<int>[] _Sets = new SortedSet<int>[] 
        {
            new SortedSet<int>(EnumerateHelper.Sequence(5,7,3,2)),
            new SortedSet<int>(EnumerateHelper.Sequence(5,7,2)),
            new SortedSet<int>(EnumerateHelper.Sequence(5,7,2,8)),
            new SortedSet<int>(EnumerateHelper.Sequence(9,10,11,12))
        };

        [Test]
        public void CctorTest_Ok()
        {
            new ReadOnlySet<int>(new SortedSet<int>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SetIsNull_Fail()
        {
            new ReadOnlySet<int>(null);
        }

        [Test]
        public void IsProperSubsetOfTest()
        {
            TestMethod(_Sets, 
                (readOnlySet, set1, set2) => 
                    Assert.AreEqual(
                        set1.IsProperSubsetOf(set2),
                        readOnlySet.IsProperSubsetOf(set2)
                    )
            );
        }

        [Test]
        public void IsProperSupersetOfTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                    Assert.AreEqual(
                        set1.IsProperSupersetOf(set2),
                        readOnlySet.IsProperSupersetOf(set2)
                    )
            );
        }

        [Test]
        public void IsSubsetOfTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                    Assert.AreEqual(
                        set1.IsSubsetOf(set2),
                        readOnlySet.IsSubsetOf(set2)
                    )
            );
        }

        [Test]
        public void IsSupersetOfTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                    Assert.AreEqual(
                        set1.IsSupersetOf(set2),
                        readOnlySet.IsSupersetOf(set2)
                    )
            );
        }

        [Test]
        public void OverlapsTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                    Assert.AreEqual(
                        set1.Overlaps(set2),
                        readOnlySet.Overlaps(set2)
                    )
            );
        }

        [Test]
        public void SetEqualsTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                    Assert.AreEqual(
                        set1.SetEquals(set2),
                        readOnlySet.SetEquals(set2)
                    )
            );
        }

        [Test]
        public void CountTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set) =>
                    Assert.AreEqual(set.Count, readOnlySet.Count)
            );
        }

        [Test]
        public void IsReadOnlyTest()
        {
            Assert.AreEqual(true,new ReadOnlySet<int>(new SortedSet<int>(EnumerateHelper.Sequence(5, 7, 2, 8))).IsReadOnly);
        }

        [Test]
        public void ContainsTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set1, set2) =>
                {
                    foreach (int value in set2)
                    {
                        Assert.AreEqual(set1.Contains(value),readOnlySet.Contains(value));
                    }
                }
            );
        }

        [Test]
        public void CopyToTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set) =>
                {
                    int[] expectedArray = new int[set.Count];
                    int[] actualArray = new int[readOnlySet.Count];
                    set.CopyTo(expectedArray, 0);
                    readOnlySet.CopyTo(actualArray, 0);
                    CollectionAssert.AreEqual(expectedArray, actualArray);
                }
            );
        }

        [Test]
        public void GetEnumeratorTest()
        {
            TestMethod(_Sets,
                (readOnlySet, set) =>
                {
                    CollectionAssert.AreEqual(set, readOnlySet);
                    Assert.IsTrue(set.SequenceEqual(readOnlySet));
                }
            );
        }

        private void TestMethod(SortedSet<int>[] sets, Action<ReadOnlySet<int>, ISet<int>> testAction)
        {
            foreach (ISet<int> set in sets)
            {
                ReadOnlySet<int> readOnlySet = new ReadOnlySet<int>(set);

                testAction(readOnlySet, set);
            }            
        }

        private void TestMethod(SortedSet<int>[] sets, Action<ReadOnlySet<int>, ISet<int>,IEnumerable<int>> testAction)
        {
            foreach (ISet<int> set1 in sets)
            {
                foreach (ISet<int> set2 in sets)
                {
                    ReadOnlySet<int> readOnlySet = new ReadOnlySet<int>(set1);

                    testAction(readOnlySet, set1, set2);
                }
            }
        }
    }
}
