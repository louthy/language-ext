using System;
using Xunit;

namespace LanguageExt.Tests
{
    public class RecordIgnoreBaseTests
    {
        public class BaseClass
        {
            public readonly int X;

            public BaseClass(int x) =>
                X = x;
        }

        public class SubClass1 : BaseClass, IEquatable<SubClass1>, IComparable<SubClass1>
        {
            public readonly int Y;

            public SubClass1(int x, int y) : base(x) =>
                Y = y;

            public int CompareTo(SubClass1 other) =>
                RecordTypeIgnoreBase<SubClass1>.Compare(this, other);

            public override bool Equals(object obj) =>
                RecordTypeIgnoreBase<SubClass1>.Equality(this, obj);

            public bool Equals(SubClass1 other) =>
                RecordTypeIgnoreBase<SubClass1>.EqualityTyped(this, other);

            public override int GetHashCode() =>
                RecordTypeIgnoreBase<SubClass1>.Hash(this);
        }

        [IgnoreBase]
        public class SubClass2 : BaseClass, IEquatable<SubClass2>, IComparable<SubClass2>
        {
            public readonly int Y;

            public SubClass2(int x, int y) : base(x) =>
                Y = y;

            public int CompareTo(SubClass2 other) =>
                RecordType<SubClass2>.Compare(this, other);

            public override bool Equals(object obj) =>
                RecordType<SubClass2>.Equality(this, obj);

            public bool Equals(SubClass2 other) =>
                RecordType<SubClass2>.EqualityTyped(this, other);

            public override int GetHashCode() =>
                RecordType<SubClass2>.Hash(this);
        }

        public class SubClass3 : BaseClass, IEquatable<SubClass3>, IComparable<SubClass3>
        {
            public readonly int Y;

            public SubClass3(int x, int y) : base(x) =>
                Y = y;

            public int CompareTo(SubClass3 other) =>
                RecordType<SubClass3>.Compare(this, other);

            public override bool Equals(object obj) =>
                RecordType<SubClass3>.Equality(this, obj);

            public bool Equals(SubClass3 other) =>
                RecordType<SubClass3>.EqualityTyped(this, other);

            public override int GetHashCode() =>
                RecordType<SubClass3>.Hash(this);
        }

        public class FirstAttributeAttribute : Attribute { }

        [FirstAttribute]
        [IgnoreBase]
        public class SubClass4 : BaseClass, IEquatable<SubClass4>, IComparable<SubClass4>
        {
            public readonly int Y;

            public SubClass4(int x, int y) : base(x) =>
                Y = y;

            public int CompareTo(SubClass4 other) =>
                RecordType<SubClass4>.Compare(this, other);

            public override bool Equals(object obj) =>
                RecordType<SubClass4>.Equality(this, obj);

            public bool Equals(SubClass4 other) =>
                RecordType<SubClass4>.EqualityTyped(this, other);

            public override int GetHashCode() =>
                RecordType<SubClass4>.Hash(this);
        }

        [Fact]
        public void TestSubClass1Eq()
        {
            var a = new SubClass1(0, 1);
            var b = new SubClass1(1, 1);

            Assert.True(a.Equals(b));
            Assert.True(a.GetHashCode() == b.GetHashCode());
            Assert.True(a.CompareTo(b) == 0);
        }

        [Fact]
        public void TestSubClass2Eq()
        {
            var a = new SubClass2(0, 1);
            var b = new SubClass2(1, 1);

            Assert.True(a.Equals(b));
            Assert.True(a.GetHashCode() == b.GetHashCode());
            Assert.True(a.CompareTo(b) == 0);
        }

        [Fact]
        public void TestSubClass3NotEq()
        {
            var a = new SubClass3(0, 1);
            var b = new SubClass3(1, 1);

            Assert.False(a.Equals(b));
            Assert.False(a.GetHashCode() == b.GetHashCode());
            Assert.False(a.CompareTo(b) == 0);
        }

        [Fact]
        public void TestSubClass4Eq()
        {
            var a = new SubClass4(0, 1);
            var b = new SubClass4(1, 1);

            Assert.True(a.Equals(b));
            Assert.True(a.GetHashCode() == b.GetHashCode());
            Assert.True(a.CompareTo(b) == 0);
        }

    }
}
