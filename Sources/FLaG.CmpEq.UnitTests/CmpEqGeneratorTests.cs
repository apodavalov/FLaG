using NUnit.Framework.Interfaces;

namespace FLaG.CmpEq.UnitTests
{
    [TestFixture]
    public sealed class CmpEqGeneratorTests
    {
        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        [TestCase(5, 5, 0)]
        public void Single(int value1, int value2, int comparisonResult)
        {
            TestSingle instance1 = new(value1);
            TestSingle instance2 = new(value2);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
        }

        [Test]
        public void SingleEqualityToObject()
        {
            TestSingle instance = new(5);
            object sameValue = new TestSingle(5);
            object otherValue = new TestSingle(6);
            object? nullValue = null;

            Assert.That(instance.Equals(sameValue), Is.True);
            Assert.That(instance.Equals(otherValue), Is.False);
            Assert.That(instance.Equals("other-object"), Is.False);
            Assert.That(instance.Equals(nullValue), Is.False);
        }

        [Test]
        public void SingleNonnullAndNull()
        {
            TestSingle instance = new(5);

            Assert.That(instance.CompareTo(null), Is.EqualTo(1));
            Assert.That(instance.Equals(null), Is.False);
            Assert.That(instance == null, Is.False);
            Assert.That(instance != null, Is.True);
            Assert.That(instance > null, Is.True);
            Assert.That(instance >= null, Is.True);
            Assert.That(instance < null, Is.False);
            Assert.That(instance <= null, Is.False);
            Assert.That(null == instance, Is.False);
            Assert.That(null != instance, Is.True);
            Assert.That(null > instance, Is.False);
            Assert.That(null >= instance, Is.False);
            Assert.That(null < instance, Is.True);
            Assert.That(null <= instance, Is.True);
        }

        [Test]
        public void SingleNulls()
        {
            TestSingle? instance1 = null;
            TestSingle? instance2 = null;

            Assert.That(instance1 == instance2, Is.True);
            Assert.That(instance1 != instance2, Is.False);
            Assert.That(instance1 > instance2, Is.False);
            Assert.That(instance1 >= instance2, Is.True);
            Assert.That(instance1 < instance2, Is.False);
            Assert.That(instance1 <= instance2, Is.True);
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        [TestCase(5, 5, 0)]
        public void Parent(int value1, int value2, int comparisonResult)
        {
            TestParent instance1 = new(value1);
            TestParent instance2 = new(value2);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
        }

        [Test]
        public void ParentEqualityToObject()
        {
            TestParent instance = new(5);
            object sameValue = new TestParent(5);
            object otherValue = new TestParent(6);
            object? nullValue = null;

            Assert.That(instance.Equals(sameValue), Is.True);
            Assert.That(instance.Equals(otherValue), Is.False);
            Assert.That(instance.Equals("other-object"), Is.False);
            Assert.That(instance.Equals(nullValue), Is.False);
        }

        [Test]
        public void ParentNonnullAndNull()
        {
            TestParent instance = new(5);

            Assert.That(instance.CompareTo(null), Is.EqualTo(1));
            Assert.That(instance.Equals(null), Is.False);
            Assert.That(instance == null, Is.False);
            Assert.That(instance != null, Is.True);
            Assert.That(instance > null, Is.True);
            Assert.That(instance >= null, Is.True);
            Assert.That(instance < null, Is.False);
            Assert.That(instance <= null, Is.False);
            Assert.That(null == instance, Is.False);
            Assert.That(null != instance, Is.True);
            Assert.That(null > instance, Is.False);
            Assert.That(null >= instance, Is.False);
            Assert.That(null < instance, Is.True);
            Assert.That(null <= instance, Is.True);
        }

        [Test]
        public void ParentNulls()
        {
            TestParent? instance1 = null;
            TestParent? instance2 = null;

            Assert.That(instance1 == instance2, Is.True);
            Assert.That(instance1 != instance2, Is.False);
            Assert.That(instance1 > instance2, Is.False);
            Assert.That(instance1 >= instance2, Is.True);
            Assert.That(instance1 < instance2, Is.False);
            Assert.That(instance1 <= instance2, Is.True);
        }

        [TestCase(5, "a", 5, "b", -1)]
        [TestCase(5, "b", 5, "a", 1)]
        [TestCase(5, "a", 5, "a", 0)]
        [TestCase(5, "a", 6, "a", -1)]
        [TestCase(6, "a", 5, "a", 1)]
        public void Child(
            int value1,
            string anotherValue1,
            int value2,
            string anotherValue2,
            int comparisonResult
        )
        {
            TestChild instance1 = new(value1, anotherValue1);
            TestChild instance2 = new(value2, anotherValue2);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        public void ParentChild(int value1, int value2, int comparisonResult)
        {
            TestParent instance1 = new(value1);
            TestChild instance2 = new(value2, "another-value");

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2 == instance1, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance2 != instance1, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void ParentChildCommon()
        {
            TestParent instance1 = new(6);
            TestChild instance2 = new(6, "another-value");

            int comparisonResult = instance1.CompareTo(instance2);
            Assert.That(comparisonResult, Is.Not.EqualTo(0));
            Assert.That(instance1.Equals(instance2), Is.False);
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.False);
            Assert.That(instance1 == instance2, Is.False);
            Assert.That(instance2 == instance1, Is.False);
            Assert.That(instance1 != instance2, Is.True);
            Assert.That(instance2 != instance1, Is.True);
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        public void ChildAnotherChild(int value1, int value2, int comparisonResult)
        {
            TestChild instance1 = new(value1, "false");
            TestAnotherChild instance2 = new(value2, false);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2 == instance1, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance2 != instance1, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void ChildAnotherChildCommon()
        {
            TestChild instance1 = new(6, "another-value");
            TestAnotherChild instance2 = new(6, false);

            int comparisonResult = instance1.CompareTo(instance2);
            Assert.That(comparisonResult, Is.Not.EqualTo(0));
            Assert.That(instance1.Equals(instance2), Is.False);
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.False);
            Assert.That(instance1 == instance2, Is.False);
            Assert.That(instance2 == instance1, Is.False);
            Assert.That(instance1 != instance2, Is.True);
            Assert.That(instance2 != instance1, Is.True);
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void ChildEqualityToObject()
        {
            TestChild instance = new(5, "same-value");
            object sameValue = new TestChild(5, "same-value");
            object otherValue = new TestChild(6, "same-value");
            object? nullValue = null;

            Assert.That(instance.Equals(sameValue), Is.True);
            Assert.That(instance.Equals(otherValue), Is.False);
            Assert.That(instance.Equals("other-object"), Is.False);
            Assert.That(instance.Equals(nullValue), Is.False);
        }

        [Test]
        public void ChildNonnullAndNull()
        {
            TestChild instance = new(5, "a");

            Assert.That(instance.CompareTo(null), Is.EqualTo(1));
            Assert.That(instance.Equals(null), Is.False);
            Assert.That(instance == null, Is.False);
            Assert.That(instance != null, Is.True);
            Assert.That(instance > null, Is.True);
            Assert.That(instance >= null, Is.True);
            Assert.That(instance < null, Is.False);
            Assert.That(instance <= null, Is.False);
            Assert.That(null == instance, Is.False);
            Assert.That(null != instance, Is.True);
            Assert.That(null > instance, Is.False);
            Assert.That(null >= instance, Is.False);
            Assert.That(null < instance, Is.True);
            Assert.That(null <= instance, Is.True);
        }

        [Test]
        public void ChildNulls()
        {
            TestChild? instance1 = null;
            TestChild? instance2 = null;

            Assert.That(instance1 == instance2, Is.True);
            Assert.That(instance1 != instance2, Is.False);
            Assert.That(instance1 > instance2, Is.False);
            Assert.That(instance1 >= instance2, Is.True);
            Assert.That(instance1 < instance2, Is.False);
            Assert.That(instance1 <= instance2, Is.True);
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        [TestCase(5, 5, 0)]
        public void GenericParent(int value1, int value2, int comparisonResult)
        {
            TestParentGeneric<int> instance1 = new(value1);
            TestParentGeneric<int> instance2 = new(value2);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
        }

        [Test]
        public void GenericParentEqualityToObject()
        {
            TestParentGeneric<int> instance = new(5);
            object sameValue = new TestParentGeneric<int>(5);
            object otherValue = new TestParentGeneric<int>(6);
            object? nullValue = null;

            Assert.That(instance.Equals(sameValue), Is.True);
            Assert.That(instance.Equals(otherValue), Is.False);
            Assert.That(instance.Equals("other-object"), Is.False);
            Assert.That(instance.Equals(nullValue), Is.False);
        }

        [Test]
        public void GenericParentNonnullAndNull()
        {
            TestParentGeneric<int> instance = new(5);

            Assert.That(instance.CompareTo(null), Is.EqualTo(1));
            Assert.That(instance.Equals(null), Is.False);
            Assert.That(instance == null, Is.False);
            Assert.That(instance != null, Is.True);
            Assert.That(instance > null, Is.True);
            Assert.That(instance >= null, Is.True);
            Assert.That(instance < null, Is.False);
            Assert.That(instance <= null, Is.False);
            Assert.That(null == instance, Is.False);
            Assert.That(null != instance, Is.True);
            Assert.That(null > instance, Is.False);
            Assert.That(null >= instance, Is.False);
            Assert.That(null < instance, Is.True);
            Assert.That(null <= instance, Is.True);
        }

        [Test]
        public void GenericParentNulls()
        {
            TestParentGeneric<int>? instance1 = null;
            TestParentGeneric<int>? instance2 = null;

            Assert.That(instance1 == instance2, Is.True);
            Assert.That(instance1 != instance2, Is.False);
            Assert.That(instance1 > instance2, Is.False);
            Assert.That(instance1 >= instance2, Is.True);
            Assert.That(instance1 < instance2, Is.False);
            Assert.That(instance1 <= instance2, Is.True);
        }

        [TestCase(5, 7, 5, 8, -1)]
        [TestCase(5, 8, 5, 7, 1)]
        [TestCase(5, 7, 5, 7, 0)]
        [TestCase(5, 7, 6, 7, -1)]
        public void GenericChild(
            int value1,
            int anotherValue1,
            int value2,
            int anotherValue2,
            int comparisonResult
        )
        {
            TestChildGeneric<int> instance1 = new(value1, anotherValue1);
            TestChildGeneric<int> instance2 = new(value2, anotherValue2);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        public void GenericParentChild(int value1, int value2, int comparisonResult)
        {
            TestParentGeneric<int> instance1 = new(value1);
            TestChildGeneric<int> instance2 = new(value2, 8);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2 == instance1, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance2 != instance1, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void GenericParentChildCommon()
        {
            TestParentGeneric<int> instance1 = new(6);
            TestChildGeneric<int> instance2 = new(6, 8);

            int comparisonResult = instance1.CompareTo(instance2);
            Assert.That(comparisonResult, Is.Not.EqualTo(0));
            Assert.That(instance1.Equals(instance2), Is.False);
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.False);
            Assert.That(instance1 == instance2, Is.False);
            Assert.That(instance2 == instance1, Is.False);
            Assert.That(instance1 != instance2, Is.True);
            Assert.That(instance2 != instance1, Is.True);
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [TestCase(5, 6, -1)]
        [TestCase(6, 5, 1)]
        public void GenericChildAnotherChild(int value1, int value2, int comparisonResult)
        {
            TestChildGeneric<int> instance1 = new(value1, 8);
            TestAnotherChildGeneric<int> instance2 = new(value2, 8);

            Assert.That(instance1.CompareTo(instance2), Is.EqualTo(comparisonResult));
            Assert.That(instance1.Equals(instance2), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 == instance2, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance2 == instance1, Is.EqualTo(comparisonResult == 0));
            Assert.That(instance1 != instance2, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance2 != instance1, Is.EqualTo(comparisonResult != 0));
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult <= 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult >= 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void GenericChildAnotherChildCommon()
        {
            TestChildGeneric<int> instance1 = new(6, 9);
            TestAnotherChildGeneric<int> instance2 = new(6, 9);

            int comparisonResult = instance1.CompareTo(instance2);
            Assert.That(comparisonResult, Is.Not.EqualTo(0));
            Assert.That(instance1.Equals(instance2), Is.False);
            Assert.That(instance2.CompareTo(instance1), Is.EqualTo(-comparisonResult));
            Assert.That(instance2.Equals(instance1), Is.False);
            Assert.That(instance1 == instance2, Is.False);
            Assert.That(instance2 == instance1, Is.False);
            Assert.That(instance1 != instance2, Is.True);
            Assert.That(instance2 != instance1, Is.True);
            Assert.That(instance1 >= instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 >= instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 > instance2, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance2 > instance1, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance1 <= instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 <= instance1, Is.EqualTo(comparisonResult > 0));
            Assert.That(instance1 < instance2, Is.EqualTo(comparisonResult < 0));
            Assert.That(instance2 < instance1, Is.EqualTo(comparisonResult > 0));
        }

        [Test]
        public void GenericChildEqualityToObject()
        {
            TestChildGeneric<int> instance = new(5, 8);
            object sameValue = new TestChildGeneric<int>(5, 8);
            object otherValue = new TestChildGeneric<int>(6, 8);
            object? nullValue = null;

            Assert.That(instance.Equals(sameValue), Is.True);
            Assert.That(instance.Equals(otherValue), Is.False);
            Assert.That(instance.Equals("other-object"), Is.False);
            Assert.That(instance.Equals(nullValue), Is.False);
        }

        [Test]
        public void GenericChildNonnullAndNull()
        {
            TestChildGeneric<int> instance = new(5, 8);

            Assert.That(instance.CompareTo(null), Is.EqualTo(1));
            Assert.That(instance.Equals(null), Is.False);
            Assert.That(instance == null, Is.False);
            Assert.That(instance != null, Is.True);
            Assert.That(instance > null, Is.True);
            Assert.That(instance >= null, Is.True);
            Assert.That(instance < null, Is.False);
            Assert.That(instance <= null, Is.False);
            Assert.That(null == instance, Is.False);
            Assert.That(null != instance, Is.True);
            Assert.That(null > instance, Is.False);
            Assert.That(null >= instance, Is.False);
            Assert.That(null < instance, Is.True);
            Assert.That(null <= instance, Is.True);
        }

        [Test]
        public void GenericChildNulls()
        {
            TestChildGeneric<int>? instance1 = null;
            TestChildGeneric<int>? instance2 = null;

            Assert.That(instance1 == instance2, Is.True);
            Assert.That(instance1 != instance2, Is.False);
            Assert.That(instance1 > instance2, Is.False);
            Assert.That(instance1 >= instance2, Is.True);
            Assert.That(instance1 < instance2, Is.False);
            Assert.That(instance1 <= instance2, Is.True);
        }
    }
}
