using LanguageExt;
using Xunit;

namespace LanguageExtTests
{
    public class UnionTests
    {
        [Fact]
        public void UnionTest1()
        {
            var x = new Union<string, int>("Test");

            object value = null;

            x.Match()
                (a => value = a)
                (b => value = b);

            Assert.IsType<string>(value);
        }

        [Fact]
        public void UnionTest2()
        {
            var x = new Union<string, int>(100);

            object value = null;

            x.Match()
                (v => value = v)
                (v => value = v);

            Assert.IsType<int>(value);
        }

        [Fact]
        public void UnionTest3()
        {
            var x = new Union<string, int>("Test");

            string value = x.Match<string, int, string>()
                (a => a)
                (b => b.ToString());

            Assert.Equal("Test", value);
        }

        [Fact]
        public void UnionTest4()
        {
            var x = new Union<string, int>(100);

            string value = x.Match<string, int, string>()
                (a => a)
                (b => b.ToString());

            Assert.Equal("100", value);
        }

        [Fact]
        public void UnionTest5()
        {
            var x = new Union<string, int>(100);

            string value = x.Match<string, int, string>()
                (a => a)
                (b => b == 100 ? "Keeping It 100." : "Tea?");

            Assert.Equal("Keeping It 100.", value);
        }
    }
}