using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using SimpleInjector.Advanced;
using Xunit;
using static LanguageExt.Prelude;

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

        TestClass(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }

    public class DerivedTestClass : TestClass
    {
        public readonly int Extra;

        public DerivedTestClass(int x, string y, Guid z, int extra) : base(x, y, z)
        {
            Extra = extra;
        }
    }

    public class TestClass2 : Record<TestClass2>
    {
        [OptOutOfEq]
        public readonly int X;

        [OptOutOfHashCode]
        public readonly string Y;

        [OptOutOfToString]
        public readonly Guid Z;

        public TestClass2(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class TestClass3 : Record<TestClass3>
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;
        public TestClass W { get; }

        public TestClass3(int x, string y, Guid z, TestClass w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        TestClass3(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public class RecordTypeTests
    {
        static readonly Guid guid = Guid.Parse("{2ba1ec03-8309-46f6-a93e-5d6aada3a43c}");

        [Fact]
        public void EqualityOfOriginTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new DerivedTestClass(1, "Hello", Guid.Empty, 1000);

            Assert.False(x.Equals(y));  // Different types must not be equal
            Assert.False(y.Equals(x));  // Different types must not be equal
        }

        [Fact]
        public void DeepEqualityTestFieldsAndProperties()
        {
            var x1 = new TestClass(1, "Hello", Guid.Empty);
            var x2 = new TestClass(1, "Hello", Guid.Empty);
            var y1 = new TestClass3(1, "Hello", Guid.Empty, x1);
            var y2 = new TestClass3(1, "Hello", Guid.Empty, x2);

            Assert.True(y1 == y2);
        }

        [Fact]
        public void DeepInEqualityTestFieldsAndProperties()
        {
            var x1 = new TestClass(1, "Hello", Guid.Empty);
            var x2 = new TestClass(1, "Hello", guid);
            var y1 = new TestClass3(1, "Hello", Guid.Empty, x1);
            var y2 = new TestClass3(1, "Hello", Guid.Empty, x2);

            Assert.True(y1 != y2);
        }

        [Fact]
        public void SerialisationTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", guid);

            var x1 = JsonConvert.SerializeObject(x);
            var y1 = JsonConvert.SerializeObject(y);

            var x2 = JsonConvert.DeserializeObject<TestClass>(x1);
            var y2 = JsonConvert.DeserializeObject<TestClass>(y1);

            Assert.True(x == x2);
            Assert.True(y == y2);
        }

        [Fact]
        public void ToStringTest()
        {
            var x = new TestClass(1, "Hello", Guid.Empty);
            var y = new TestClass(1, "Hello", guid);

            Assert.True(x.ToString() == "TestClass(1, Hello, 00000000-0000-0000-0000-000000000000)");
            Assert.True(y.ToString() == "TestClass(1, Hello, 2ba1ec03-8309-46f6-a93e-5d6aada3a43c)");
        }

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
            var w = new TestClass(1, "Hello", guid);

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
            var w = new TestClass(1, "Hello", guid);

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
            var e = new TestClass(1, "Hello", guid);

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

        [Fact]
        public void OptOutOfEqTest()
        {
            var x = new TestClass2(1, "Hello", Guid.Empty);
            var y = new TestClass2(1, "Hello", Guid.Empty);
            var z = new TestClass2(2, "Hello", Guid.Empty);

            Assert.True(x == y);
            Assert.True(x == z);
        }

        [Fact]
        public void OptOutOfHashCodeTest()
        {
            var x = new TestClass2(1, "Hello", Guid.Empty);
            var y = new TestClass2(1, "Hello32543534", Guid.Empty);
            var z = new TestClass2(1, "Hello", Guid.Empty);

            Assert.True(x.GetHashCode() == y.GetHashCode());
            Assert.True(x.GetHashCode() == z.GetHashCode());
        }

        [Fact]
        public void OptOutOfToString()
        {
            var x = new TestClass2(1, "Hello", Guid.Empty);
            var y = new TestClass2(1, "Hello", Guid.NewGuid());

            Assert.True(x.ToString() == y.ToString());
        }
    }

    /*
    public struct OrdSet<T> : Ord<Set<T>>
    {
        public bool Equals(Set<T> x, Set<T> y)
        {
            return default(OrdSet<OrdInt, int>).Compare( x,y )==0;
        }

        public int GetHashCode(Set<T> x)
        {
            throw new NotImplementedException();
        }

        public int Compare(Set<T> x, Set<T> y)
        {
            throw new NotImplementedException();
        }
    }
    */

    /// <summary>
    /// These test will all fail because there is no <![CDATA[ OrdSet for Set<ORD,T> ]]>
    /// </summary>
    public class ContainerWithCustomOrdOrderingTests
    {
        public class Data : Record<Data>
        {
            public Data(Set<OrdInt, int> so)
            {
                SO = so;
            }

            public Set<OrdInt, int> SO { get; }
        }

        [Fact]
        public void GetOrderableContainerOrd2Works()
        {
            var type = Class<Ord<Set<OrdInt, int>>>.GetOrderableContainerOrd2();

            // Type should be OrdSet<OrdInt,int>
            Assert.True(type.IsSome);
        }

        [Fact]
        public void SetOrderingWorks()
        {
            {
                var a = new Data(Set<OrdInt, int>(0, 1, 2));
                var b = new Data(Set<OrdInt, int>(0, 1, 2));
                Assert.Equal(0, a.CompareTo(b));
            }
        }
    }

    public class ContainerWithOrderingTests
    {
        public class Data : Record<Data>
        {
            public Data(int i, Set<int> s, Arr<int> a, Lst<int> l)
            {
                I = i;
                S = s;
                A = a;
                L = l;
            }

            public int I { get; }
            public Set<int> S { get; }
            public Lst<int> L { get; }
            public Arr<int> A { get; }

        }

        [Fact]
        public void GetOrderableContainerOrdWorks()
        {
            var type = Class<Ord<Set<int>>>.GetOrderableContainerOrd();
            Assert.True( type.IsSome );

        }

        [Fact]
        public void SetOrderingWorks()
        {
            {
                var a = new Data( 0, Set( 0, 1, 2 ), Array( 0, 1, 2 ), List( 0, 1, 2 ));
                var b = new Data( 0, Set( 0, 1, 2 ), Array( 0, 1, 2 ), List( 0, 1, 2 ));
                Assert.Equal( 0, a.CompareTo( b ) );
            }
            {
                var a = new Data( 0, Set( 0, 1, 2 ), Array( 1, 1, 2 ), List( 0, 1, 2 ));
                var b = new Data( 0, Set( 0, 1, 2 ), Array( 0, 1, 2 ), List( 0, 1, 2 ));
                Assert.Equal( 1, a.CompareTo( b ) );
            }
            {
                var a = new Data( 0, Set( 0, 1, 2 ), Array( -1, 1, 2 ), List( 0, 1, 2 ) );
                var b = new Data( 0, Set( 0, 1, 2 ), Array( 0, 1, 2 ), List( 0, 1, 2 )  );
                Assert.Equal( -1, a.CompareTo( b ) );
            }
        }
    }
}
