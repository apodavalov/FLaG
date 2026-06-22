namespace FLaG.CmpEq.UnitTests
{
    [ComparableEquatable]
    public sealed partial class TestSingle(int value)
    {
        private readonly int _Value = value;

        public int FetchHashCode() => _Value.GetHashCode();

        public int CompareToNonnull(TestSingle other) => _Value.CompareTo(other._Value);

        public bool EqualsNonnull(TestSingle other) => _Value.Equals(other._Value);
    }

    public sealed partial class TestSingleGeneric<T>(T value)
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly T _Value = value;

        public int FetchHashCode() => _Value.GetHashCode();

        public int CompareToNonnull(TestSingleGeneric<T> other) => _Value.CompareTo(other._Value);

        public bool EqualsNonnull(TestSingleGeneric<T> other) => _Value.Equals(other._Value);
    }

    [ComparableEquatable]
    public partial class TestParent(int value)
    {
        private readonly int _Value = value;

        public virtual int FetchHashCode() => _Value.GetHashCode();

        public virtual int CompareToNonnull(TestParent other) => _Value.CompareTo(other._Value);

        public virtual bool EqualsNonnull(TestParent other) => _Value.Equals(other._Value);
    }

    [ComparableEquatable]
    public sealed partial class TestChild(int intValue, string strValue) : TestParent(intValue)
    {
        private readonly string _StrValue = strValue;

        public override int FetchHashCode() => _StrValue.GetHashCode();

        public int CompareToNonnull(TestChild other) => _StrValue.CompareTo(other._StrValue);

        public bool EqualsNonnull(TestChild other) => _StrValue.Equals(other._StrValue);
    }

    [ComparableEquatable]
    public sealed partial class TestAnotherChild(int intValue, bool boolValue)
        : TestParent(intValue)
    {
        private readonly bool _BoolValue = boolValue;

        public override int FetchHashCode() => _BoolValue.GetHashCode();

        public int CompareToNonnull(TestAnotherChild other) =>
            _BoolValue.CompareTo(other._BoolValue);

        public bool EqualsNonnull(TestAnotherChild other) => _BoolValue.Equals(other._BoolValue);
    }

    [ComparableEquatable]
    public partial class TestParentGeneric<T>(T value)
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly T _Value = value;

        public virtual int FetchHashCode() => _Value.GetHashCode();

        public virtual int CompareToNonnull(TestParentGeneric<T> other) =>
            _Value.CompareTo(other._Value);

        public virtual bool EqualsNonnull(TestParentGeneric<T> other) =>
            _Value.Equals(other._Value);
    }

    [ComparableEquatable]
    public sealed partial class TestChildGeneric<T>(T value, T anotherValue)
        : TestParentGeneric<T>(value)
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly T _AnotherValue = anotherValue;

        public override int FetchHashCode() => _AnotherValue.GetHashCode();

        public int CompareToNonnull(TestChildGeneric<T> other)
        {
            return _AnotherValue.CompareTo(other._AnotherValue);
        }

        public bool EqualsNonnull(TestChildGeneric<T> other) =>
            _AnotherValue.Equals(other._AnotherValue);
    }

    [ComparableEquatable]
    public sealed partial class TestAnotherChildGeneric<T>(T value, T anotherValue)
        : TestParentGeneric<T>(value)
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly T _AnotherValue = anotherValue;

        public override int FetchHashCode() => _AnotherValue.GetHashCode();

        public int CompareToNonnull(TestAnotherChildGeneric<T> other)
        {
            return _AnotherValue.CompareTo(other._AnotherValue);
        }

        public bool EqualsNonnull(TestAnotherChildGeneric<T> other) =>
            _AnotherValue.Equals(other._AnotherValue);
    }
}
