using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.TypeClass;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.ClassInstances.Const;
using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;

namespace LanguageExt.Tests
{
    public class Metres : NumType<Metres, TInt, int>
    {
        public Metres(int value) : base(value) { }
    }

    public class Hours : NumType<Hours, TInt, int, ForAll<int, GreaterOrEq<TInt, int, I0>, LessThan<TInt, int, I24>>>
    {
        public Hours(int value) : base(value) { }
    }

    public class Seconds : NumType<Seconds,  TInt, int, Range<TInt, int, I0, I59>>
    {
        public Seconds(int value) : base(value) { }
    }

    public class Email : NewType<Email, string, True<string>, OrdStringOrdinalIgnoreCase>
    {
        public Email(string value) : base(value) { }
    }

    public class NewTypeTests
    {
        [Fact]
        public void OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Hours.New(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Hours.New(24));

            Assert.Throws<ArgumentOutOfRangeException>(() => Seconds.New(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Seconds.New(60));
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

        #pragma warning disable CS1718
        [Fact]
        public void OrdTest2()
        {
            var a1 = new Email("a@b.com");
            var a2 = new Email("A@b.com");
            var b = new Email("b@b.com");

            Assert.Equal(0, a1.CompareTo(a1));
            Assert.Equal(0, a1.CompareTo(a2));
            Assert.Equal(0, a2.CompareTo(a1));

            Assert.True(a1.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a1) > 0);


            Assert.True(a1 == a1);
            Assert.True(a1 == a2);
            Assert.True(a2 == a1);
            Assert.False(a1 == b);

            Assert.True(a1 <= a1);
            Assert.True(a1 <= a2);
            Assert.True(a2 <= a1);
            Assert.True(a1 <= b);

            Assert.True(a1 >= a1);
            Assert.True(a1 >= a2);
            Assert.True(a2 >= a1);
            Assert.False(a1 >= b);

            Assert.False(a1 < a1);
            Assert.False(a1 < a2);
            Assert.False(a2 < a1);
            Assert.True(a1 < b);

            Assert.False(a1 > a1);
            Assert.False(a1 > a2);
            Assert.False(a2 > a1);
            Assert.False(a1 > b);
        }

        [Fact]
        public void HashTest()
        {
            var a1 = new Email("a@b.com");
            var a2 = new Email("A@b.com");

            Assert.True(a1.GetHashCode() == a2.GetHashCode());
        }

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

            Assert.True(res == Metres.New(3));

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

            Assert.True(r1 == Metres.New(10));
            Assert.True(r2 == Metres.New(6));
            Assert.True(r3 == Metres.New(4));
            Assert.True(r4 == Metres.New(16));

            var h1 = new Hours(1);

            // Mixing types - won't now compile!  success!
            // Assert.Throws<Exception>(() => m1 + h1);
            // Assert.Throws<Exception>(() => m1 - h1);
            // Assert.Throws<Exception>(() => m1 / h1);
            // Assert.Throws<Exception>(() => m1 * h1);
        }

        [Fact]
        public void SerialisationTest()
        {
            var m1 = Metres.New(10);

            var str = JsonConvert.SerializeObject(m1);

            var m2 = JsonConvert.DeserializeObject<Metres>(str);

            Assert.True(m2 == m1);
        }
    }
}
