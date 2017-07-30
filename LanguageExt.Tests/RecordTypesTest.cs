using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using Xunit;

namespace LanguageExt.Tests
{
    public class TestClass : Record<TestClass>
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;

        public TestClass(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class RecordTypeTests
    {
        [Fact]
        public void EqualityOperatorTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", Guid.Empty);

            Assert.True(x == y);
        }

        [Fact]
        public void EqualityMethodTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", Guid.Empty);

            Assert.True(x.Equals(y));
        }

        [Fact]
        public void NullEqualityOperatorTest()
        {
            TestClass x = new TestClass(1, "Hello", Guid.Empty);
            TestClass y = null;
            TestClass z = null;

            Assert.True(x != y);
            Assert.True(y != x);
            Assert.True(y == z);
            Assert.True(z == y);
        }

        [Fact]
        public void NullEqualityMethodTest()
        {
            TestClass x = new TestClass(1, "Hello", Guid.Empty);
            TestClass y = null;

            Assert.False(x.Equals(y));
        }

        [Fact]
        public void InEqualityOperatorTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(2, "Hello", Guid.Empty);
            var z = new TestClass(1, "Hello!", Guid.Empty);
            var w = new TestClass(1, "Hello", Guid.NewGuid());

            Assert.True(x != y);
            Assert.True(x != z);
            Assert.True(x != w);
        }

        [Fact]
        public void InEqualityMethodTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(2, "Hello", Guid.Empty);
            var z = new TestClass(1, "Hello!", Guid.Empty);
            var w = new TestClass(1, "Hello", Guid.NewGuid());

            Assert.False(x.Equals(y));
            Assert.False(x.Equals(z));
            Assert.False(x.Equals(w));
        }

        [Fact]
        public void HashingTest()
        {
            var a = new TestClass(1, "Hello", Guid.Empty);
            var b = new TestClass(1, "Hello", Guid.Empty);
            var c = new TestClass(2, "Hello", Guid.Empty);
            var d = new TestClass(1, "Hello!", Guid.Empty);
            var e = new TestClass(1, "Hello", Guid.NewGuid());

            Assert.True(a.GetHashCode() == b.GetHashCode());
            Assert.True(a.GetHashCode() != c.GetHashCode());
            Assert.True(a.GetHashCode() != d.GetHashCode());
            Assert.True(a.GetHashCode() != e.GetHashCode());
        }


        [Fact]
        public void OrderingTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", Guid.Empty);

            Assert.True(x.CompareTo(y) == 0);
        }

        [Fact]
        public void NullOrderingOperatorTest()
        {
            TestClass x = new TestClass(1, "Hello", Guid.Empty);
            TestClass y = null;
            TestClass z = null;

            Assert.True(x > y);
            Assert.True(x >= y);
            Assert.True(y < x);
            Assert.True(y <= x);
            Assert.True(y == z);
            Assert.True(z == y);
        }

        [Fact]
        public void NullOrderingMethodTest()
        {
            TestClass x = new TestClass(1, "Hello", Guid.Empty);
            TestClass y = null;

            Assert.True(x.CompareTo(y) > 0);
        }

        [Fact]
        public void OrderingOperatorTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(2, "Hello", Guid.Empty);
            var z = new TestClass(1, "Jello", Guid.Empty);

            Assert.True(x < y);
            Assert.True(x <= y);
            Assert.True(y > x);
            Assert.True(y >= x);

            Assert.True(x < z);
            Assert.True(x <= z);
            Assert.True(z > x);
            Assert.True(z >= x);
        }

        [Fact]
        public void OrderingMethodTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(2, "Hello", Guid.Empty);
            var z = new TestClass(1, "Jello", Guid.Empty);

            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(x, y) < 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(x, y) <= 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(y, x) > 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(y, x) >= 0);

            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(x, z) < 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(x, z) <= 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(z, x) > 0);
            Assert.True(GenericCompare<OrdRecord<TestClass>, TestClass>(z, x) >= 0);
        }

        [Fact]
        public void EqClassInstanceTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", Guid.Empty);
            var z = new TestClass(2, "Hello", Guid.Empty);

            var resA = GenericEquals<EqRecord<TestClass>,TestClass>(x, y);
            var resB = GenericEquals<EqRecord<TestClass>,TestClass>(x, z);

            Assert.True(resA);
            Assert.False(resB);
        }


        [Fact]
        public void OrdClassInstanceTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(2, "Hello", Guid.Empty);
            var z = new TestClass(1, "Jello", Guid.Empty);

            Assert.True(x.CompareTo(y) < 0);
            Assert.True(x.CompareTo(y) <= 0);
            Assert.True(y.CompareTo(x) > 0);
            Assert.True(y.CompareTo(x) >= 0);

            Assert.True(x.CompareTo(z) < 0);
            Assert.True(x.CompareTo(z) <= 0);
            Assert.True(z.CompareTo(x) > 0);
            Assert.True(z.CompareTo(x) >= 0);
        }

        public bool GenericEquals<EqA, A>(A x, A y) where EqA : struct, Eq<A> =>
            default(EqA).Equals(x, y);

        public int GenericCompare<OrdA, A>(A x, A y) where OrdA : struct, Ord<A> =>
            default(OrdA).Compare(x, y);

    }
}
