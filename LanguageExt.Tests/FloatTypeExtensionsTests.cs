using LanguageExt;
using LanguageExt.ClassInstances;
using Xunit;
using System.Linq;

namespace LanguageExt.Tests
{
    public class Euros : FloatType<Euros, TDecimal, decimal>
    {
        public Euros(decimal value) : base(value) { }
    }

    public class FloatTypeExtensionsTests
    {
        [Fact]
        public void SumTest()
        {
            var xs = new[] { Euros.New(10), Euros.New(20), Euros.New(30) };

            var res = xs.Sum();

            Assert.True(res == Euros.New(60));
        }
    }
}
