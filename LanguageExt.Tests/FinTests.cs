using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class FinTests
    {
        [Fact]
        public void Equality()
        {
            var fin1 = FinSucc(1);
            Assert.Equal(1, fin1);
        }

        [Fact]
        public void Enumerability()
        {
            var fin1 = FinSucc(1);
            Assert.True(fin1.Single() is 1, fin1.ToString());
        }

        [Fact]
        public void LinqEnumerability()
        {
            Assert.Equal(Array(6), from x in FinSucc(2) from y in Some(3) select x * y);
            Assert.Equal(Array(6), from x in FinSucc(2) from y in Set(3) select x * y);
            Assert.Equal(Array(6), from x in Some(2) from y in FinSucc(3) select x * y);
            Assert.Equal(Array(6), from x in Some(2) from y in FinSucc(3) select x * y);
        }
    }
}
