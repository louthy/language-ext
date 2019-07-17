using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class TupleTests
    {
        [Fact] public void TupleGeneratorTests()
        {
            var t2 = Tuple("a", "b");
            var t3 = Tuple("a", "b", "c");
            var t4 = Tuple("a", "b", "c", "d");
            var t5 = Tuple("a", "b", "c", "d", "e");
            var t6 = Tuple("a", "b", "c", "d", "e", "f");
            var t7 = Tuple("a", "b", "c", "d", "e", "f", "g");

            Assert.True(t2.Item1 == "a" && t2.Item2 == "b");
            Assert.True(t3.Item1 == "a" && t3.Item2 == "b" && t3.Item3 == "c");
            Assert.True(t4.Item1 == "a" && t4.Item2 == "b" && t4.Item3 == "c" && t4.Item4 == "d");
            Assert.True(t5.Item1 == "a" && t5.Item2 == "b" && t5.Item3 == "c" && t5.Item4 == "d" && t5.Item5 == "e");
            Assert.True(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.True(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.True(t7.Item1 == "a" && t7.Item2 == "b" && t7.Item3 == "c" && t7.Item4 == "d" && t7.Item5 == "e" && t7.Item6 == "f" && t7.Item7 == "g");
        }

        [Fact] public void WithApplicationTests1()
        {
            Tuple("a", "b").Iter((a, b) => Assert.True(a == "a" && b == "b"));
            Tuple("a", "b", "c").Iter((a, b, c) => Assert.True(a == "a" && b == "b" && c == "c"));
            Tuple("a", "b", "c", "d").Iter((a, b, c, d) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d"));
            Tuple("a", "b", "c", "d", "e").Iter((a, b, c, d, e) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            Tuple("a", "b", "c", "d", "e", "f").Iter((a, b, c, d, e, f) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            Tuple("a", "b", "c", "d", "e", "f", "g").Iter((a, b, c, d, e, f, g) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

        [Fact] public void WithApplicationTests2()
        {
            iter( Tuple("a", "b"), (a, b) => Assert.True(a == "a" && b == "b"));
            iter( Tuple("a", "b", "c"), (a, b, c) => Assert.True(a == "a" && b == "b" && c == "c"));
            iter( Tuple("a", "b", "c", "d"), (a, b, c, d) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d"));
            iter( Tuple("a", "b", "c", "d", "e"), (a, b, c, d, e) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            iter( Tuple("a", "b", "c", "d", "e", "f"), (a, b, c, d, e, f) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            iter( Tuple("a", "b", "c", "d", "e", "f", "g"), (a, b, c, d, e, f, g) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

        [Fact]
        public void ValueTupleGeneratorTests()
        {
            var t2 = Tuple("a", "b");
            var t3 = Tuple("a", "b", "c");
            var t4 = Tuple("a", "b", "c", "d");
            var t5 = Tuple("a", "b", "c", "d", "e");
            var t6 = Tuple("a", "b", "c", "d", "e", "f");
            var t7 = Tuple("a", "b", "c", "d", "e", "f", "g");

            Assert.True(t2.Item1 == "a" && t2.Item2 == "b");
            Assert.True(t3.Item1 == "a" && t3.Item2 == "b" && t3.Item3 == "c");
            Assert.True(t4.Item1 == "a" && t4.Item2 == "b" && t4.Item3 == "c" && t4.Item4 == "d");
            Assert.True(t5.Item1 == "a" && t5.Item2 == "b" && t5.Item3 == "c" && t5.Item4 == "d" && t5.Item5 == "e");
            Assert.True(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.True(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.True(t7.Item1 == "a" && t7.Item2 == "b" && t7.Item3 == "c" && t7.Item4 == "d" && t7.Item5 == "e" && t7.Item6 == "f" && t7.Item7 == "g");
        }

        [Fact]
        public void ValueWithApplicationTests1()
        {
            ("a", "b").Iter((a, b) => Assert.True(a == "a" && b == "b"));
            ("a", "b", "c").Iter((a, b, c) => Assert.True(a == "a" && b == "b" && c == "c"));
            ("a", "b", "c", "d").Iter((a, b, c, d) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d"));
            ("a", "b", "c", "d", "e").Iter((a, b, c, d, e) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            ("a", "b", "c", "d", "e", "f").Iter((a, b, c, d, e, f) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            ("a", "b", "c", "d", "e", "f", "g").Iter((a, b, c, d, e, f, g) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

        [Fact]
        public void ValueWithApplicationTests2()
        {
            iter(("a", "b"), (a, b) => Assert.True(a == "a" && b == "b"));
            iter(("a", "b", "c"), (a, b, c) => Assert.True(a == "a" && b == "b" && c == "c"));
            iter(("a", "b", "c", "d"), (a, b, c, d) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d"));
            iter(("a", "b", "c", "d", "e"), (a, b, c, d, e) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            iter(("a", "b", "c", "d", "e", "f"), (a, b, c, d, e, f) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            iter(("a", "b", "c", "d", "e", "f", "g"), (a, b, c, d, e, f, g) => Assert.True(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

    }
}
