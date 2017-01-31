using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    public class Metres : NewType<int>
    {
        public Metres(int value) : base(value) { }
    }

    public class Hours : NewType<int>
    {
        public Hours(int value) : base(value) { }
    }

    public class NewTypeTests
    {
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
            //var r3 = m1 == h1;    // won't compile
        }
        [Fact]
        public void EqTestWithNull()
        {
            var m1 = new Metres(1);
            var m2 = default(Metres);

            Assert.False(m1 == m2);
            Assert.False(m1 == null);
            Assert.False(null == m1);

            Assert.True(null == m2);
            Assert.True(m2 == null);
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

            // Mixing types
            Assert.Throws<Exception>(() => h1 > m2);
            Assert.Throws<Exception>(() => h1 < m2);
            Assert.Throws<Exception>(() => h1 >= m2);
            Assert.Throws<Exception>(() => h1 <= m2);
        }

#if !COREFX
        [Fact]
        public void LinqTest()
        {
            var m1 = new Metres(1);
            var m2 = new Metres(2);

            var res = from x in m1
                      from y in m2
                      select x + y;

            Assert.True(res.Value == 3);
            Assert.True(res is Metres);

            var h1 = new Hours(1);

            // Mixing types
            Assert.Throws<Exception>(() =>
                from x in m1
                from y in h1
                select x + y
                  );
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

            // Mixing types
            Assert.Throws<Exception>(() => m1 + h1);
            Assert.Throws<Exception>(() => m1 - h1);
            Assert.Throws<Exception>(() => m1 / h1);
            Assert.Throws<Exception>(() => m1 * h1);
        }
#endif
    }
}
