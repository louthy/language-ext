using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.TypeClass;

namespace LanguageExtTests
{
    public class Metres : NewType<Metres, TInt, int>
    {
        public Metres(int value) : base(value) { }
    }

    public class Hours : NewType<Hours, int>
    {
        public Hours(int value) : base(value) { }
    }

    public class Seconds : NewType<Seconds, int>
    {
        public Seconds(int value) : base(value) { }
    }

    public class NewTypeTests
    {
        [Fact]
        public void BrokenTest()
        {
            var x = new Seconds(10);
        }

        public Metres New(int x) => new Metres(x);

        [Fact]
        public void EqTest1()
        {
            var m1 = new Metres(1);
            var m2 = new Metres(1);
            var m3 = new Metres(2);

            var h1 = new Hours(1);
            var h2 = new Hours(1);
            var h3 = new Hours(2);

            Assert.True(m1 == m2);
            Assert.False(m1 == m3);

            Assert.True(m1.Equals(m2));
            Assert.False(m1.Equals(m3));

            //var r3 = m1 == h1;    // won't compile
        }

        [Fact]
        public void OrdTest1()
        {
            var m1 = new Metres(1);
            var m2 = new Metres(2);

            var h1 = new Hours(1);
            var h2 = new Hours(1);

            Assert.True(m1 < m2);
            Assert.True(m2 > m1);
            Assert.True(m1 <= m2);
            Assert.True(m2 >= m1);

            Assert.True(m1.CompareTo(m2) < 0);
            Assert.True(m2.CompareTo(m1) > 0);
            Assert.True(m1.CompareTo(m2) <= 0);
            Assert.True(m2.CompareTo(m1) >= 0);

            // Mixing types - won't compile! success!
            // Assert.Throws<Exception>(() => h1 > m2);
            // Assert.Throws<Exception>(() => h1 < m2);
            // Assert.Throws<Exception>(() => h1 >= m2);
            // Assert.Throws<Exception>(() => h1 <= m2);
        }

#if !COREFX
        [Fact]
        public void LinqTest()
        {
            var m1 = new Metres(1);
            var m2 = new Metres(2);

            var ctor = Metres.New;

            var test = ctor(1);

            Metres res = from x in m1
                         from y in m2
                         select x + y;

            Assert.True(res.Value == 3);

            var h1 = new Hours(1);

            // Mixing types - won't now compile!  success!
            // Assert.Throws<Exception>(() =>
            //    from x in m1
            //    from y in h1
            //    select x + y
            //      );
        }

        [Fact]
        public void ArithmeticTest()
        {
            var m1 = new Metres(8);
            var m2 = new Metres(2);

            var r1 = m1 + m2;
            var r2 = m1 - m2;
            var r3 = m1 / m2;
            var r4 = m1 * m2;

            Assert.True(r1.Value == 10);
            Assert.True(r2.Value == 6);
            Assert.True(r3.Value == 4);
            Assert.True(r4.Value == 16);

            var h1 = new Hours(1);

            // Mixing types - won't now compile!  success!
            // Assert.Throws<Exception>(() => m1 + h1);
            // Assert.Throws<Exception>(() => m1 - h1);
            // Assert.Throws<Exception>(() => m1 / h1);
            // Assert.Throws<Exception>(() => m1 * h1);
        }

        public class Arr<A> : NewType<Arr<A>, TArray<A>, OrdArray<OrdDefault<A>, A>, A[]>
        {
            public Arr(A[] x) : base(x) { }
        }

        [Fact]
        public void AppendTest()
        {
            var a1 = new Arr<int>(new[] { 1, 2, 3, 4, 5 });
            var a2 = new Arr<int>(new[] { 10, 2 });

            var a3 = append(a1, a2);

            var a4 = a1 + a2;

            Assert.True(a3.Value.Length == 7);
            Assert.True(a3.Value[0] == 1);
            Assert.True(a3.Value[1] == 2);
            Assert.True(a3.Value[2] == 3);
            Assert.True(a3.Value[3] == 4);
            Assert.True(a3.Value[4] == 5);
            Assert.True(a3.Value[5] == 10);
            Assert.True(a3.Value[6] == 2);

            Assert.True(a4.Value.Length == 7);
            Assert.True(a4.Value[0] == 1);
            Assert.True(a4.Value[1] == 2);
            Assert.True(a4.Value[2] == 3);
            Assert.True(a4.Value[3] == 4);
            Assert.True(a4.Value[4] == 5);
            Assert.True(a4.Value[5] == 10);
            Assert.True(a4.Value[6] == 2);

        }
#endif
    }
}
